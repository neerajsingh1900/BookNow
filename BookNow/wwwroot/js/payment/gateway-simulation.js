
$(document).ready(function () {
    // --- 1. DOM Elements & Constants ---
    const timerDisplay = $('#payment-timer');
    const successButton = $('#btn-simulate-success');
    const failButton = $('#btn-simulate-failure');
    const processingSpinner = '<span class="spinner-border spinner-border-sm"></span> Processing...';



    const bookingId = successButton.data('booking-id');
    const CityCookieKey = "BN_CityId";
    function getCookieValue(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return '1'; 
    }
    const cityId = getCookieValue(CityCookieKey); 

    if (!bookingId || !cityId) {
        console.error("CRITICAL: Booking ID or City ID not found. Cannot start simulation.");
        return;
    }
    function lockPaymentUI() {
        // Disable all form inputs and accordion buttons to prevent changes during processing
        $('input, select, button[data-bs-toggle="collapse"]').prop('disabled', true);
    }
    function disableButtons(statusText) {
        lockPaymentUI(); // <--- CALL THE NEW LOCK FUNCTION HERE
        // Disable both buttons and show processing text/spinner on the Success button
        successButton.prop('disabled', true).html(processingSpinner);
        failButton.prop('disabled', true).text(statusText);
    }
    // Timer setup (5 minutes hold duration, matching your backend logic)
    const holdDurationMinutes = 5;
    let timeRemaining = holdDurationMinutes * 60;
    let timerInterval;
    let isFinalized = false;
    function generateIdempotencyKey() {
       
        if (crypto && crypto.randomUUID) return crypto.randomUUID();
        
        return 'id-' + Math.random().toString(36).substr(2, 16) + Date.now();
    }



    // --- 2. Utility & Core Logic ---
    function updateTimerDisplay() {
        const minutes = Math.floor(timeRemaining / 60);
        const seconds = timeRemaining % 60;
        const timeString = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

        timerDisplay.text(timeString);

      
        if (timeRemaining <= 60 && timeRemaining > 0) {
            timerDisplay.addClass('text-danger').removeClass('text-warning');
        } else if (timeRemaining <= 0) {
            timerDisplay.removeClass('text-danger').text('TIME OUT');
        }
    }

    function disableButtons(statusText) {
      
        successButton.prop('disabled', true).html(processingSpinner);
        failButton.prop('disabled', true).text(statusText);
    }

    // Sends the status update to the backend service
    function handleResponse(status) {

        if (isFinalized) return; 
        isFinalized = true;

        clearInterval(timerInterval);
        disableButtons(`Simulating ${status}...`);

        const idempotencyKey = generateIdempotencyKey();



        const command = {
            BookingId: bookingId,
            Status: status,
            CityId: parseInt(cityId),
            IdempotencyKey: idempotencyKey
        };

        // Use the correct Area route for the AJAX call
        $.ajax({
            url: '/Customer/Payment/HandleGatewayResponse',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(command),
            success: function (response) {
                // Backend returns JSON: { success: true, redirectUrl: "/Customer/Payment/Success" }
                if (response.success && response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                } else {
                   
                    window.location.href = '/Customer/Payment/Failed';
                }
            },
            error: function (jqXHR) {
                console.error("AJAX error processing payment response:", jqXHR);
                window.location.href = '/Customer/Payment/Failed';
            }
        });
    }

    // --- 3. Timer Logic ---
    function startTimer() {
        updateTimerDisplay(); // Initial display

        timerInterval = setInterval(() => {
            timeRemaining--;
            updateTimerDisplay();

            if (timeRemaining <= 0) {
                clearInterval(timerInterval);
                // Automatically trigger the 'timeout' path when timer hits zero
                handleResponse('timeout');
            }
        }, 1000);
    }

    // --- 4. Event Handlers ---
    successButton.on('click', function () {
        handleResponse('success');
    });

    failButton.on('click', function () {
        handleResponse('failure');
    });


    window.addEventListener('beforeunload', function (e) {
        if (!isFinalized) {
            const msg = "Are you sure you want to cancel the payment?";
            e.preventDefault();
            e.returnValue = msg;
            return msg;
        }
    });
    

    window.addEventListener('unload', function () {
        if (!isFinalized) {
            const payload = JSON.stringify({ BookingId: bookingId });
            const blob = new Blob([payload], { type: 'application/json' });
            navigator.sendBeacon('/Customer/Payment/ReleaseSeats', blob);
        }
    });


    // Start the whole process upon document load
    startTimer();
});