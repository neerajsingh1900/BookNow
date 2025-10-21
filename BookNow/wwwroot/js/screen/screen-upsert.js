$(document).ready(function () {
    const $form = $('#screenUpsertForm');
    const $submitButton = $('#submitButton');
    const $serverSummary = $('#serverValidationSummary');
    const $rowsInput = $('#NumberOfRows');
    const $seatsInput = $('#SeatsPerRow');
    const $calculatedSeats = $('#calculatedSeats');
    const $seatsSummary = $('#totalSeatsSummary');
        

    // --- Seat Calculation Logic (UX Improvement) ---
    const calculateSeats = () => {
        const rows = parseInt($rowsInput.val()) || 0;
        const seats = parseInt($seatsInput.val()) || 0;
        const total = rows * seats;

        $calculatedSeats.text(total);
        if (total > 0) {
            $seatsSummary.removeClass('d-none');
        } else {
            $seatsSummary.addClass('d-none');
        }
    };
    $rowsInput.on('input', calculateSeats);
    $seatsInput.on('input', calculateSeats);
    calculateSeats(); // Initial calculation

    // --- Form Submission (Robust API Interaction) ---
    $form.on('submit', function (e) {
        e.preventDefault();

        if (!$form.valid()) return;

        $submitButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Saving...');
        $serverSummary.addClass('d-none').text('');

        const formData = {
            screenId: $('#ScreenId').val() || null,
            theatreId: parseInt($('#TheatreId').val()),
            screenNumber: $('#ScreenNumber').val(),
            numberOfRows: parseInt($rowsInput.val()),
            seatsPerRow: parseInt($seatsInput.val()),
            defaultSeatPrice: parseFloat($('#DefaultSeatPrice').val())
        };

        const url = '/TheatreOwner/api/screen/add';
        // NOTE: For simplicity, we use POST for both Add and Edit in this example
        // In a true PUT/Edit, you'd use a separate PUT endpoint.

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Include CSRF token if enabled
                // 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(formData)
        })
            .then(async response => {
                $submitButton.prop('disabled', false).html('<i class="fas fa-save me-1"></i> @actionType Screen');
                if (response.ok) {
                    return response.json();
                } else {
                    const errorData = await response.json();
                    // ValidationException from service comes here
                    throw new Error(errorData.message || 'Error occurred while creating the screen.');
                }
            })
            .then(data => {
                console.log('@actionType successful:', data);
                // Redirect to the screen list after successful creation
                window.location.href = `/TheatreOwner/Screen/Index/${formData.theatreId}`;
            })
            .catch(error => {
                console.error('API Error:', error);
                $serverSummary.text(error.message).removeClass('d-none');
            });
    });

    // Omitted: Client-side data loading for Edit mode (fetch screen details).
});