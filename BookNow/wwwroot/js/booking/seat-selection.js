$(document).ready(function () {
    // --- 1. DOM Elements and Constants ---
    const selectedSeats = new Map();
    const currencySymbol = $('#currency-symbol').text();
    const payButton = $('#btn-pay');
    const seatsCountSpan = $('#num-seats');
    const totalPriceSpan = $('#total-price');
    const btnPriceDisplaySpan = $('#btn-price-display');
    const ticketsCountBadge = $('#tickets-count');
    // Ensure showId is parsed as an integer for SignalR grouping
    const showId = parseInt(payButton.data('show-id'));
    const statusModal = new bootstrap.Modal(document.getElementById('statusModal'));

    // --- 2. SignalR Setup ---
    // Ensure the SignalR client script is included in your _Layout_SeatMap.cshtml
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/seatMapHub") // Connects to the endpoint mapped in Program.cs
        .withAutomaticReconnect()
        .build();

    // Start the connection and join the show's group
    connection.start().then(() => {
        console.log("SignalR Connected. Joining group:", showId);
        // Invoke the server method to join the show's group
        connection.invoke("JoinShowGroup", showId).catch(err => {
            console.error("Failed to join show group:", err);
        });
    }).catch(err => console.error("SignalR Connection Error:", err));


    // --- 3. SignalR Receive Handler ---
    connection.on("ReceiveSeatUpdate", function (jsonUpdate) {
        // jsonUpdate is a JSON string: [{"seatInstanceId": 123, "state": "Held"}, ...]
        try {
            const updates = JSON.parse(jsonUpdate);

            updates.forEach(update => {
                const seatInstanceId = update.seatInstanceId;
                const newState = update.state;

                const seatElement = $(`[data-seat-instance-id="${seatInstanceId}"]`);

                if (seatElement.length === 0) return; // Ignore if element doesn't exist

                // Remove existing state classes
                seatElement.removeClass('seat-available seat-held seat-sold seat-selected clickable-seat');

                // Apply new state class
                if (newState === "Held") {
                    // Seat is now held by a competitor or the current user's hold succeeded
                    seatElement.addClass('seat-held');
                    selectedSeats.delete(seatInstanceId); // Remove from client's selection map
                    seatElement.removeClass('clickable-seat');
                } else if (newState === "Booked") {
                    // Payment confirmed (Post-Payment success)
                    seatElement.addClass('seat-sold');
                    selectedSeats.delete(seatInstanceId);
                    seatElement.removeClass('clickable-seat');
                } else if (newState === "Available") {
                    // Hold expired via TTL or DB transaction failed/rolled back
                    seatElement.addClass('seat-available clickable-seat');
                }
            });

            // Refresh summary if any seat was removed from selection due to real-time update
            updateSummary();
        } catch (e) {
            console.error("Error processing real-time seat update:", e);
        }
    });


    // --- 4. Utility Functions ---
    function formatPrice(price) {
        return price.toFixed(2);
    }

    function updateSummary() {
        let total = 0;
        selectedSeats.forEach(seat => {
            total += Number(seat.price);
        });

        const count = selectedSeats.size;

        totalPriceSpan.text(formatPrice(total));
        seatsCountSpan.text(count);
        btnPriceDisplaySpan.text(formatPrice(total));
        ticketsCountBadge.text(`${count} Tickets`);
        payButton.prop('disabled', count === 0);
    }

    function showModal(title, message, footerContent) {
        $('#statusModalLabel').text(title);
        $('#modal-message').html(message);
        $('#modal-footer-content').empty().html(footerContent);
        statusModal.show();
    }

    // --- 5. SEAT CLICK HANDLER (Original Logic) ---
    $('#seat-map').on('click', '.clickable-seat', function () {
        const seatElement = $(this);
        const seatInstanceId = parseInt(seatElement.data('seat-instance-id'));
        const price = parseFloat(seatElement.data('price'));
        const rowVersion = seatElement.data('row-version');

        if (seatElement.hasClass('seat-selected')) {
            // Deselect seat
            seatElement.removeClass('seat-selected');
            selectedSeats.delete(seatInstanceId);
        } else {
            // Select seat (Enforce max 10 seats)
            if (selectedSeats.size >= 10) {
                showModal("Selection Limit Reached", "You can select a maximum of 10 seats per booking.",
                    `<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>`);
                return;
            }

            seatElement.addClass('seat-selected');
            selectedSeats.set(seatInstanceId, { price: price, rowVersion: rowVersion });
        }
        updateSummary();
    });

    // --- 6. PAYMENT / HOLD TRIGGER (Using Confirmed API Path) ---
    payButton.on('click', function () {
        if (selectedSeats.size === 0) return;

        // Disable button and show loading spinner
        payButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
        payButton.prop('disabled', true);

        // Construct the command DTO
        const seatInstanceIds = Array.from(selectedSeats.keys());
        const seatVersions = {};
        selectedSeats.forEach((value, key) => {
            seatVersions[key] = value.rowVersion;
        });

        const command = {
            ShowId: showId,
            SeatInstanceIds: seatInstanceIds,
            SeatVersions: seatVersions
        };

        // CRITICAL: Use the confirmed API route
        $.ajax({
            url: '/api/Customer/BookingApi/CreateHold',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(command),
            success: function (response) {
                // Restore button state (in case redirect fails)
                payButton.html(`Pay ${currencySymbol}<span id="btn-price-display">${btnPriceDisplaySpan.text()}</span>`);
                payButton.prop('disabled', false);

                if (response.success && response.redirectUrl) {
                    // Success: Redirect to Payment Gateway
                    window.location.href = response.redirectUrl;
                } else {
                    // Handle Validation/Service Errors (if success=false)
                    handleError(response.errorMessage || "An unknown error occurred.", false);
                }
            },
            error: function (jqXHR) {
                // Restore button state
                payButton.html(`Pay ${currencySymbol}<span id="btn-price-display">${btnPriceDisplaySpan.text()}</span>`);
                payButton.prop('disabled', false);

                let errorData = jqXHR.responseJSON;

                if (jqXHR.status === 401) {
                    // 401: Unauthorized -> Login Required
                    showLoginPrompt();
                } else if (jqXHR.status === 409 || (errorData && errorData.error && (errorData.error.includes("race condition") || errorData.error.includes("taken") || errorData.error.includes("concurrency")))) {
                    // 409: Concurrency Conflict (From Redis or DB)
                    handleError(errorData.error || "The selected seats are no longer available.", true);
                } else {
                    // Other errors (500, validation, etc.)
                    handleError(errorData && errorData.error ? errorData.error : "An unexpected server error occurred. Please try refreshing.", false);
                }
            }
        });
    });

    // --- 7. ERROR & LOGIN HANDLERS (Original Logic) ---

    function showLoginPrompt() {
        const cityChanged = sessionStorage.getItem("cityChanged");
        const returnUrl = cityChanged ? '/' : window.location.pathname + window.location.search;
        const loginUrl = `/Identity/Account/Login?ReturnUrl=${encodeURIComponent(returnUrl)}`;
        const footer = `
            <a href="${loginUrl}" class="btn btn-primary">Login / Sign Up</a>
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        `;
        showModal("Authentication Required",
            "Please log in or sign up to finalize your seat booking and proceed to payment.",
            footer);
    }

    function handleError(message, isConcurrency) {
        let footer;
        let title = "Booking Failed";

        if (isConcurrency) {
            title = "Seats Taken!";
            footer = `
                <button type="button" class="btn btn-danger" onclick="window.location.reload()">Refresh Map</button>
            `;
            // Ensure message is clear about the need to refresh
            if (!message.includes("refresh")) {
                message += "<br><br>Click **Refresh Map** to see the updated availability.";
            }
        } else {
            footer = `<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>`;
        }

        showModal(title, message, footer);
    }

    // --- 8. Clean Up (SignalR) ---
    $(window).on('beforeunload', function () {
        if (connection.state === signalR.HubConnectionState.Connected && showId) {
            connection.invoke("LeaveShowGroup", showId).catch(() => { });
        }
    });
});
