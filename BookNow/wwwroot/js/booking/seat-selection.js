
$(document).ready(function () {
    const selectedSeats = new Map(); 
    const currencySymbol = $('#currency-symbol').text();
    const payButton = $('#btn-pay');
    const seatsCountSpan = $('#num-seats');
    const totalPriceSpan = $('#total-price');
    const btnPriceDisplaySpan = $('#btn-price-display');
    const ticketsCountBadge = $('#tickets-count');
    const showId = payButton.data('show-id');
    const statusModal = new bootstrap.Modal(document.getElementById('statusModal'));


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

    // --- SEAT CLICK HANDLER ---

    $('#seat-map').on('click', '.clickable-seat', function () {
        const seatElement = $(this);
        // Use data-seat-instance-id which is the unique identifier for locking
        const seatInstanceId = parseInt(seatElement.data('seat-instance-id'));
        const price = parseFloat(seatElement.data('price'));
        const rowVersion = seatElement.data('row-version');
        console.log("seatinstanceid", seatInstanceId);
        console.log("price", price);
        console.log("rowver", rowVersion);
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

    // --- PAYMENT / HOLD TRIGGER ---

    payButton.on('click', function () {
        if (selectedSeats.size === 0) return;

        // Disable button and show loading spinner
        payButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
        payButton.prop('disabled', true);

        // Construct the command DTO
        const seatInstanceIds = Array.from(selectedSeats.keys());
        const seatVersions = {};
        selectedSeats.forEach((value, key) => {
            // Key is SeatInstanceId, Value is Base64 RowVersion string
            seatVersions[key] = value.rowVersion;
        });

        const command = {
            ShowId: showId,
            SeatInstanceIds: seatInstanceIds,
            SeatVersions: seatVersions
        };

        // POST to the CreateHoldAndRedirect action
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
                } else if (jqXHR.status === 409 || (errorData && errorData.error && errorData.error.includes("race condition"))) {
                    // 409: Concurrency Conflict or known service conflict error
                    handleError(errorData.error || "The selected seats are no longer available.", true);
                } else {
                    // Other errors (500, validation, etc.)
                    handleError(errorData && errorData.error ? errorData.error : "An unexpected server error occurred. Please try refreshing.", false);
                }
            }
        });
    });

    // --- ERROR & LOGIN HANDLERS ---

    function showLoginPrompt() {
        // Construct return URL to send user back here after successful login
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
});
