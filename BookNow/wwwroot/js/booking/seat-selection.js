$(document).ready(function () {
    
    const selectedSeats = new Map();
    const currencySymbol = $('#currency-symbol').text();
    const payButton = $('#btn-pay');
    const seatsCountSpan = $('#num-seats');
    const totalPriceSpan = $('#total-price');
    const btnPriceDisplaySpan = $('#btn-price-display');
    const ticketsCountBadge = $('#tickets-count');
    const showId = parseInt(payButton.data('show-id'));
    const statusModal = new bootstrap.Modal(document.getElementById('statusModal'));

   
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/seatMapHub") 
        .withAutomaticReconnect()
        .build();

  
    connection.start().then(() => {
        console.log("SignalR Connected. Joining group:", showId);
       
        connection.invoke("JoinShowGroup", showId).catch(err => {
            console.error("Failed to join show group:", err);
        });
    }).catch(err => console.error("SignalR Connection Error:", err));


   
    connection.on("ReceiveSeatUpdate", function (jsonUpdate) {
        try {
            const updates = JSON.parse(jsonUpdate);

            updates.forEach(update => {
                const seatInstanceId = update.seatInstanceId;
                const newState = update.state;

                const seatElement = $(`[data-seat-instance-id="${seatInstanceId}"]`);

                if (seatElement.length === 0) return; // Ignore if element doesn't exist

                seatElement.removeClass('seat-available seat-held seat-sold seat-selected clickable-seat');

                if (newState === "Held") {
                    seatElement.addClass('seat-held');
                    selectedSeats.delete(seatInstanceId); 
                    seatElement.removeClass('clickable-seat');
                } else if (newState === "Booked") {
                    seatElement.addClass('seat-sold');
                    selectedSeats.delete(seatInstanceId);
                    seatElement.removeClass('clickable-seat');
                } else if (newState === "Available") {
                    seatElement.addClass('seat-available clickable-seat');
                }
            });

            
            updateSummary();
        } catch (e) {
            console.error("Error processing real-time seat update:", e);
        }
    });


   
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

   
    $('#seat-map').on('click', '.clickable-seat', function () {
        const seatElement = $(this);
        const seatInstanceId = parseInt(seatElement.data('seat-instance-id'));
        const price = parseFloat(seatElement.data('price'));
        const rowVersion = seatElement.data('row-version');

        if (seatElement.hasClass('seat-selected')) {
            
            seatElement.removeClass('seat-selected');
            selectedSeats.delete(seatInstanceId);
        } else {
            
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

  
    payButton.on('click', function () {
        if (selectedSeats.size === 0) return;

       
        payButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
        payButton.prop('disabled', true);

        
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

     
        $.ajax({
            url: '/api/Customer/BookingApi/CreateHold',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(command),
            success: function (response) {
                
                payButton.html(`Pay ${currencySymbol}<span id="btn-price-display">${btnPriceDisplaySpan.text()}</span>`);
                payButton.prop('disabled', false);

                if (response.success && response.redirectUrl) {
                   
                    window.location.href = response.redirectUrl;
                } else {
                  
                    handleError(response.errorMessage || "An unknown error occurred.", false);
                }
            },
            error: function (jqXHR) {
               
                payButton.html(`Pay ${currencySymbol}<span id="btn-price-display">${btnPriceDisplaySpan.text()}</span>`);
                payButton.prop('disabled', false);

                let errorData = jqXHR.responseJSON;

                if (jqXHR.status === 401) {
                   
                    showLoginPrompt();
                } else if (jqXHR.status === 409 || (errorData && errorData.error && (errorData.error.includes("race condition") || errorData.error.includes("taken") || errorData.error.includes("concurrency")))) {
                    
                    handleError(errorData.error || "The selected seats are no longer available.", true);
                } else {
                  
                    handleError(errorData && errorData.error ? errorData.error : "An unexpected server error occurred. Please try refreshing.", false);
                }
            }
        });
    });

   

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
           
            if (!message.includes("refresh")) {
                message += "<br><br>Click **Refresh Map** to see the updated availability.";
            }
        } else {
            footer = `<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>`;
        }

        showModal(title, message, footer);
    }

   
    $(window).on('beforeunload', function () {
        if (connection.state === signalR.HubConnectionState.Connected && showId) {
            connection.invoke("LeaveShowGroup", showId).catch(() => { });
        }
    });
});
