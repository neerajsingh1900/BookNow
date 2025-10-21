$(document).ready(function () {
    const theatreId = THEATRE_ID;
    console.log('Theatre ID:', theatreId, typeof theatreId);
    const tableBody = $('#screensTableBody');
    const loadingIndicator = $('#loadingIndicator');
    const noDataMessage = $('#noDataMessage');

    // Function to fetch and render screens
    const loadScreens = () => {
        loadingIndicator.removeClass('d-none');
        noDataMessage.addClass('d-none');
        tableBody.empty();

        fetch(`/TheatreOwner/api/screen/list/${theatreId}`) // Assuming API endpoint for listing
            .then(response => {
                loadingIndicator.addClass('d-none');
                if (!response.ok) {
                    throw new Error('Failed to load screens.');
                }
              //  console.log("response",response.json());
                return response.json();
            })
            .then(data => {
                console.log("Received data:", data);
                loadingIndicator.addClass('d-none');
                if (data && data.length > 0) {
                    data.forEach(screen => {
                        // Action links pointing to the MVC controllers
                        const scheduleUrl = `/TheatreOwner/Screen/ScheduleShow?screenId=${screen.screenId}`;
                        const editUrl = `/TheatreOwner/Screen/Upsert?theatreId=${theatreId}&id=${screen.screenId}`;

                        const row = `
                                    <tr>
                                        <td><div class="fw-bold">${screen.screenNumber}</div></td>
                                        <td>${screen.totalSeats}</td>
                                        <td>$${screen.defaultSeatPrice.toFixed(2)}</td>
                                        <td>
                                            <a href="#" class="text-primary small">View Shows (0)</a>
                                        </td>
                                        <td>
                                            <a href="${scheduleUrl}" class="btn btn-sm btn-primary me-2">Schedule Show</a>
                                            <a href="${editUrl}" class="btn btn-sm btn-outline-secondary">Edit Screen</a>
                                        </td>
                                    </tr>
                                `;
                        tableBody.append(row);
                    }); 
                    if (!dataTable) {
                        dataTable = $('#tblScreens').DataTable({
                            responsive: true,
                            pageLength: 10,
                            lengthChange: false,
                            ordering: true,
                            searching: true,
                            language: {
                                search: "_INPUT_",
                                searchPlaceholder: "Search screens..."
                            }
                        });
                    } else {
                        dataTable.clear().rows.add($('#tblScreens tbody tr')).draw();
                    }
                }
                else {
                    noDataMessage.removeClass('d-none');
                }
            })
            .catch(error => {
                console.error('API Error:', error);
                loadingIndicator.addClass('d-none');
                noDataMessage.html('<p class="text-danger">Error loading screens. Please check network connection.</p>').removeClass('d-none');
            });
    };

    loadScreens(); // Initial load
}



);