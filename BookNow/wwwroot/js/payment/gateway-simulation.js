
$(document).ready(function () {
    
    const timerDisplay = $('#payment-timer');
    const successButton = $('#btn-simulate-success');
    const failButton = $('#btn-simulate-failure');
    const processingSpinner = '<span class="spinner-border spinner-border-sm"></span> Processing...';
    const bookingId = successButton.data('booking-id');
    const CityCookieKey = "BN_CityId";


    const expiryTimestamp = parseInt($('#hold-expiry-timestamp').val());
    const serverTimestamp = parseInt($('#server-timestamp').val());
    const clientNow = Math.floor(Date.now() / 1000);
    const clockOffset = serverTimestamp - clientNow;

    let timeRemaining = expiryTimestamp - (clientNow + clockOffset);
    console.log("Time remaining (seconds):", timeRemaining, clientNow, clockOffset);
    if (timeRemaining < 0) timeRemaining = 0;
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
        $('input, select, button[data-bs-toggle="collapse"]').prop('disabled', true);
    }
    function disableButtons(statusText) {
        lockPaymentUI(); 
       
        successButton.prop('disabled', true).html(processingSpinner);
        failButton.prop('disabled', true).text(statusText);
    }
    
  
    
    let timerInterval;
    let isFinalized = false;
    function generateIdempotencyKey() {
       
        if (crypto && crypto.randomUUID) return crypto.randomUUID();
        
        return 'id-' + Math.random().toString(36).substr(2, 16) + Date.now();
    }




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

  

    startTimer();
});