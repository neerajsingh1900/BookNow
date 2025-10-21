$(document).ready(function () {
    const theatreId = THEATRE_ID;
    const table = $('#tblScreens');
    console.log('Theatre ID:', theatreId, typeof theatreId);
    const tableBody = $('#screensTableBody');
    const loadingIndicator = $('#loadingIndicator');
    const noDataMessage = $('#noDataMessage');
    let dataTable = null;

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

                if (dataTable) {
                    dataTable.destroy();
                    dataTable = null;
                }
                
                if (data && data.length > 0) {
              

                    data.forEach(screen => {
                        // Action links pointing to the MVC controllers
                        const scheduleUrl = `/TheatreOwner/Screen/ScheduleShow?screenId=${screen.screenId}`;
                        const editUrl = `/TheatreOwner/Screen/Upsert?theatreId=${theatreId}&id=${screen.screenId}`;
                        const row = `
                                    <tr>
                                        <td><div class="fw-bold">${screen.screenNumber}</div></td>
                                           <td>${screen.totalSeats}</td>
                                        <td>${screen.defaultSeatPrice.toFixed(2)}</td>
                                        <td>
                                            <a href="#" class="text-primary small">View Shows ${screen.currentShowCount}</a>
                                        </td>
                                        <td>
                                          <div class="d-flex">
                <a href="${scheduleUrl}" class="btn btn-sm btn-primary me-2 d-flex align-items-center">
                    <i class="fas fa-calendar-plus me-1"></i> Add Show
                </a>
             <a href="${editUrl}" class="btn btn-sm btn-outline-secondary d-flex align-items-center">
                    <i class="fas fa-edit me-1"></i> Edit
                </a>
            </div>
                                            </td>
                                    </tr>
                                `;

                        tableBody.append(row);
                    }); 
                   dataTable = table.DataTable({
                        paging: true,
                        searching: true,
                        ordering: true,
                        info: true,
                        lengthChange: true,
                        pageLength: 5, // optional: default 10
                        language: {
                            search: "Search screens:",
                            lengthMenu: "Show _MENU_ entries per page",
                            info: "Showing _START_ to _END_ of _TOTAL_ screens"
                        }
                    });

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
});