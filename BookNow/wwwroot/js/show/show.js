$(document).ready(function () {

    const showsRow = $('#showsRow');
    const loadingIndicator = $('#loadingIndicator');
    const noDataMessage = $('#noDataMessage');

    $.ajax({
        url: `/TheatreOwner/api/show/GetShows?screenId=${screenId}`,
        type: 'GET',
        success: function (result) {
            loadingIndicator.addClass('d-none');

            const shows = result.data;

            if (!shows || shows.length === 0) {
                noDataMessage.removeClass('d-none');
                return;
            }

            showsRow.removeClass('d-none');

            shows.forEach(function (show) {
                const now = new Date();
                const start = new Date(show.startTime);
                const end = new Date(show.endTime);
                const status = end < now ? "Past" : (start > now ? "Upcoming" : "In Progress");
                const statusBadgeClass = status === "Past" ? "bg-secondary" : (status === "Upcoming" ? "bg-success" : "bg-warning text-dark");
                const movieDuration = show.movieDurationMinutes;
                const showDuration = Math.floor((end - start) / 60000);

                const cardHtml = `
                            <div class="col">
                                <div class="card shadow-sm h-100">
                                    <div class="row g-0">
                                        <div class="col-md-2 d-none d-md-block">
                                            <img src="${show.moviePosterUrl}" class="img-fluid rounded-start h-100 object-fit-cover" alt="${show.movieTitle} Poster" style="width: 100%; min-height: 200px;">
                                        </div>
                                        <div class="col-md-10">
                                            <div class="card-body d-flex flex-column justify-content-between h-100">
                                                <div>
                                                    <div class="d-flex justify-content-between align-items-center mb-2">
                                                        <h5 class="card-title mb-0 fw-bold text-dark">${show.movieTitle}</h5>
                                                        <span class="badge ${statusBadgeClass}">${status}</span>
                                                    </div>
                                                    <p class="card-subtitle text-muted mb-3">
                                                        <i class="fas fa-film me-1"></i> ${show.movieGenre}
                                                        <span class="mx-2 text-dark">|</span>
                                                        <i class="fas fa-clock me-1"></i> ${movieDuration} mins
                                                    </p>
                                                    <div class="row g-2">
                                                        <div class="col-12 col-sm-6">
                                                            <div class="p-2 border rounded bg-light">
                                                                <small class="text-muted">Start Time</small><br />
                                                                <span class="fw-bold text-success">${start.toLocaleDateString()}</span>
                                                                <span class="fw-bold"> ${start.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-6">
                                                            <div class="p-2 border rounded bg-light">
                                                                <small class="text-muted">End Time (Slot)</small><br />
                                                                <span class="fw-bold text-danger">${end.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                                                                <small class="text-muted">(${showDuration} mins)</small>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="mt-3">
                                                    ${status === "Upcoming" ?
                        `<a href="#" class="btn btn-sm btn-info me-2"><i class="fas fa-wrench me-1"></i> Manage Show</a>
                                                         <a href="#" class="btn btn-sm btn-outline-secondary"><i class="fas fa-ticket-alt me-1"></i> View Bookings</a>` :
                        status === "In Progress" ? `<span class="badge bg-danger">Currently Running</span>` : ''
                    }
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;

                showsRow.append(cardHtml);
            });
        },
        error: function () {
            loadingIndicator.addClass('d-none');
            noDataMessage.removeClass('d-none').text("Failed to load shows.");
        }
    });
});