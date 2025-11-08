    $(document).ready(function () {
            const API_BASE_URL = "/api/Customer/ShowSearchApi/showtimes";
    const $listContainer = $('#theatre-showtime-list');
    const $header = $('#showtime-header');
    const movieId = @Model.Movie.MovieId;
    let activeDate = '@activeDateString';

    function updateShowtimes(targetDateStr, targetDayName) {

        $listContainer.html('<div class="text-center py-5"><div class="spinner-border text-danger" role="status"></div><p class="mt-2 text-muted">Loading showtimes for ' + targetDayName + '...</p></div>');


    fetch(`${API_BASE_URL}/${movieId}?date=${targetDateStr}`)
                    .then(res => {
                        if (!res.ok) throw new Error('Failed to fetch showtimes.');
    return res.text();
                    })
                    .then(htmlFragment => {

        $listContainer.html(htmlFragment);
    $header.text(`Showtimes for ${targetDayName}`);


    $('.date-selector').removeClass('bg-danger text-white shadow-sm');
    $(`.date-selector[data-date="${targetDateStr}"]`).addClass('bg-danger text-white shadow-sm');
    activeDate = targetDateStr;
                    })
                    .catch(error => {
        console.error("Error loading dynamic showtimes:", error);
    $listContainer.html('<div class="alert alert-danger">Error loading data. Please try refreshing the page.</div>');
                    });
            }


    $('#date-filter-strip').on('click', '.date-selector.clickable', function () {
                const $clickedDiv = $(this);
    const targetDateStr = $clickedDiv.data('date');
    const targetDayName = $clickedDiv.find('.fw-bold.small').text().trim() + ' ' + $clickedDiv.find('.small').text().trim();


    if (targetDateStr !== activeDate) {
        updateShowtimes(targetDateStr, targetDayName);
                }
            });
        });
