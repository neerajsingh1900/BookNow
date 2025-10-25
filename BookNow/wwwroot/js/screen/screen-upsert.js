$(document).ready(function () {
    const $form = $('#screenUpsertForm');
    const $submitButton = $('#submitButton');
    const $serverSummary = $('#serverValidationSummary');
    const $rowsInput = $('#NumberOfRows');
    const $seatsInput = $('#SeatsPerRow');
    const $calculatedSeats = $('#calculatedSeats');
    const $seatsSummary = $('#totalSeatsSummary');
  
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
    calculateSeats(); 

});