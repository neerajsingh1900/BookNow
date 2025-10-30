$(document).ready(function () {
    const CLEANUP_BUFFER = 15;


    let allMovies = [];
    $.getJSON('/TheatreOwner/api/show/GetAllMovies', function (data) {
        allMovies = data.map(m => ({
            id: m.movieId,
            text: m.title,
            duration: parseInt(m.duration),
            poster: m.posterUrl,
            genre: m.genre,
            language: m.language
        }));

        $('#MovieSelect2').select2({
            placeholder: "Select a movie...",
            allowClear: true,
            data: allMovies,
            templateResult: formatMovieOption,
            templateSelection: formatMovieSelection,
            width: '100%'
        });

        function formatMovieOption(movie) {
            if (!movie.id) return movie.text;
            const poster = movie.poster ? `<img src="${movie.poster}" style="width:50px; height:70px; object-fit:cover; margin-right:10px;">` : '';
            return $(`
                        <div style="display:flex; align-items:center;">
                            ${poster}
                            <div>
                                <strong>${movie.text}</strong><br>
                                <small>${movie.duration} min | ${movie.genre || ''} | ${movie.language || ''}</small>
                            </div>
                        </div>
                    `);
        }

        function formatMovieSelection(movie) {
            return movie.text || movie.id;
        }
    });


    function updateEndTime() {
        const startVal = $('#StartTimeInput').val();
        const movieData = $('#MovieSelect2').select2('data')[0];
        if (!startVal || !movieData) return;

        const durationWithBuffer = parseInt(movieData.duration || 0) + CLEANUP_BUFFER;
        const start = new Date(startVal);
        const duration = movieData.duration || 0;
        const end = new Date(start.getTime() + (duration + CLEANUP_BUFFER) * 60000);

        $('#EndTimeDisplay').text('End Time: ' + end.toLocaleString());
        $('#DurationMinutesInput').val(durationWithBuffer);
    }

    $('#MovieSelect2, #StartTimeInput').on('change', function () {
        updateEndTime();
        validateFutureTime();
        updateSubmitButtonState();
    });


    function updateSubmitButtonState() {
        const movieSelected = $('#MovieSelect2').val() && $('#MovieSelect2').val() !== "";
        const startVal = $('#StartTimeInput').val();
        const startValid = startVal && new Date(startVal) >= new Date(new Date().getTime() + MIN_FUTURE_MINUTES * 60000);

        if (movieSelected && startValid) {
            $('#submitButton').prop('disabled', false).removeClass('btn-secondary').addClass('btn-primary');
        } else {
            $('#submitButton').prop('disabled', true).removeClass('btn-primary').addClass('btn-secondary');
        }
    }


    const MIN_FUTURE_MINUTES = 30;
    const $clientFutureTimeError = $('#clientFutureTimeError');

    function validateFutureTime() {
        const val = $('#StartTimeInput').val();
        if (!val) return;

        const selectedTime = new Date(val);
        const minValidTime = new Date(new Date().getTime() + MIN_FUTURE_MINUTES * 60000);
        if (selectedTime < minValidTime) {
            $clientFutureTimeError.text(`Show must start at least ${MIN_FUTURE_MINUTES} minutes from now.`).show();
        } else {
            $clientFutureTimeError.hide();
        }
    }
});