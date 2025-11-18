    //$(document).ready(function () {
    //const theatreId = THEATRE_ID;
    //const table = $('#tblScreens');
    //console.log('Theatre ID:', theatreId, typeof theatreId);
    //const tableBody = $('#screensTableBody');
    //const loadingIndicator = $('#loadingIndicator');
    //const noDataMessage = $('#noDataMessage');
    //let dataTable = null;


    //    window.reloadScreens = function () {
    //        if (dataTable) {
    //            dataTable.ajax.reload(null, false);
    //        } else {

    //            loadScreens();
    //        }
    //    }

    //const loadScreens = () => {
    //    loadingIndicator.removeClass('d-none');
    //    noDataMessage.addClass('d-none');
    //    tableBody.empty();


    //    fetch(`/TheatreOwner/api/screen/list/${theatreId}`)
    //        .then(response => {
    //            loadingIndicator.addClass('d-none');
    //            if (!response.ok) {
    //                throw new Error('Failed to load screens.');
    //            }
    //            return response.json();
    //        })
    //        .then(data => {
    //            console.log("Received data:", data);
    //            loadingIndicator.addClass('d-none');

    //            if (dataTable) {
    //                dataTable.destroy();
    //                dataTable = null;
    //            }

    //            if (data && data.length > 0) {


    //                data.forEach(screen => {

    //                    const scheduleUrl = `/TheatreOwner/Show/Upsert?screenId=${screen.screenId}`;
    //                    const editUrl = `/TheatreOwner/Screen/Upsert?theatreId=${theatreId}&id=${screen.screenId}`;
    //                    const viewshowsUrl = `/TheatreOwner/Show/Index?screenId=${screen.screenId}`;
    //                    const row = `
    //                                <tr>
    //                                    <td><div class="fw-bold">${screen.screenNumber}</div></td>
    //                                       <td>${screen.totalSeats}</td>
    //                                    <td>${screen.defaultSeatPrice.toFixed(2)}</td>
    //                                    <td>
    //                                        <a href="${viewshowsUrl}" class="text-primary small">View Shows ${screen.currentShowCount}</a>
    //                                    </td>
    //                                    <td>
    //                                      <div class="d-flex">
    //            <a href="${scheduleUrl}" class="btn btn-sm btn-primary me-2 d-flex align-items-center">
    //                <i class="fas fa-calendar-plus me-1"></i>Show
    //            </a>
    //         <a href="${editUrl}" class="btn btn-sm btn-outline-secondary d-flex align-items-center">
    //                <i class="fas fa-edit me-1"></i>
    //            </a>
    //        </div>
    //                                        </td>
    //                                </tr>
    //                            `;

    //                    tableBody.append(row);
    //                });
    //               dataTable = table.DataTable({
    //                    paging: true,
    //                    searching: true,
    //                    ordering: true,
    //                    info: true,
    //                    lengthChange: true,
    //                    pageLength: 10,
    //                    language: {
    //                        search: "Search screens:",
    //                        lengthMenu: "Show _MENU_ entries per page",
    //                        info: "Showing _START_ to _END_ of _TOTAL_ screens"
    //                    }
    //                });

    //            }
    //            else {
    //                noDataMessage.removeClass('d-none');
    //            }
    //        })
    //        .catch(error => {
    //            console.error('API Error:', error);
    //            loadingIndicator.addClass('d-none');
    //            noDataMessage.html('<p class="text-danger">Error loading screens. Please check network connection.</p>').removeClass('d-none');
    //        });
    //};


    //loadScreens();
//});



const API_ROOT_SCREEN = '/TheatreOwner/api/screen';
let dataTable = null;

window.reloadScreens = function () {
    if (dataTable) {
       
        dataTable.ajax.reload(null, false);
    } else {
       
        loadScreens();
    }
}


window.handleSoftDeleteClickScreen = function (screenId, screenNumber, hasActiveBookings) {
    if (hasActiveBookings) {
        
        Swal.fire({
            title: 'Deletion Blocked 🛑',
            html: `Screen **${screenNumber}** cannot be deleted because there are **active, uncompleted customer bookings** tied to its future shows.`,
            icon: 'info',
            confirmButtonText: 'Understood',
            confirmButtonColor: '#007bff'
        });
    } else {
       
        confirmSoftDeleteScreen(screenId, screenNumber);
    }
};

window.confirmSoftDeleteScreen = function (screenId, screenNumber) {
    Swal.fire({
        title: 'Confirm Delete',
        text: `Are you sure you want to delete Screen ${screenNumber}? This will cancel all future shows.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#E53935',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, soft-delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`${API_ROOT_SCREEN}/${screenId}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' }
            })
                .then(response => {
                    if (response.ok) return response.json();
                    return response.json().then(errorData => {
                        throw { message: errorData.message || 'An unknown error occurred.', status: response.status };
                    });
                })
                .then(data => {
                    window.reloadScreens();
                    Swal.fire('Soft Deleted!', data.message, 'success');
                })
                .catch(error => {
                    console.error('Soft Delete Error:', error);
                    Swal.fire('Error!', error.message || 'The server returned an error.', 'error');
                });
        }
    });
};

function getStatusBadge(status) {
    let badgeClass = 'bg-secondary';
    if (status === 'PendingApproval') badgeClass = 'bg-warning text-dark';
    else if (status === 'Active') badgeClass = 'bg-success';
    else if (status === 'Maintenance') badgeClass = 'bg-danger';
    return `<span class="badge rounded-pill px-3 py-2 ${badgeClass}">${status}</span>`;
}


const loadScreens = () => {
    const table = $('#tblScreens');
    const loadingIndicator = $('#loadingIndicator');
    const noDataMessage = $('#noDataMessage');

  
    loadingIndicator.removeClass('d-none');
    noDataMessage.addClass('d-none');

    if (dataTable) {
       
        dataTable.ajax.reload();
        return;
    }

   
    dataTable = table.DataTable({
       
        ajax: {
            url: `${API_ROOT_SCREEN}/list/${THEATRE_ID}`,
            dataSrc: '',
            type: 'GET',
            error: function (xhr, error, thrown) {
                console.error("DataTables AJAX Error:", error, thrown);
                loadingIndicator.addClass('d-none');
                noDataMessage.html('<p class="text-danger">Error loading screens. Please try again.</p>').removeClass('d-none');
            },
            dataFilter: function (data) {
                // Hide loading indicator only after fetch completes (can be in success callback too)
                loadingIndicator.addClass('d-none');
                return data;
            }
        },

        // Define columns based on DTO properties
        columns: [
            { data: "screenNumber", render: (data) => `<div class="fw-bold">${data}</div>` },
            { data: "totalSeats" },
            { data: "defaultSeatPrice", render: (data) => data.toFixed(2) },
            {
                data: "currentShowCount",
                render: function (data, type, row) {
                    const viewshowsUrl = `/TheatreOwner/Show/Index?screenId=${row.screenId}`;
                    return `<a href="${viewshowsUrl}" class="text-primary small">View Shows (${data})</a>`;
                }
            },
            {
                data: "screenId",
                orderable: false,
                className: "text-center",
                render: function (data, type, row) {
                    const scheduleUrl = `/TheatreOwner/Show/Upsert?screenId=${data}`;
                   
                    const hasActiveBookings = row.hasActiveBookings || false;
                    
                    const disabledTitle = hasActiveBookings ? 'title="Active bookings prevent modification/deletion."' : '';

                    return `
                        <div class="d-flex justify-content-center">
                            <a href="${scheduleUrl}" class="btn btn-sm btn-primary me-2 d-flex align-items-center">
                                <i class="fas fa-calendar-plus me-1"></i>Show
                            </a>
                          
                            <button onclick="handleSoftDeleteClickScreen(${data}, '${row.screenNumber}', ${hasActiveBookings})" 
                                    class="btn btn-sm btn-danger d-flex align-items-center"
                                    ${disabledTitle}>
                                <i class="fas fa-trash-alt me-1"></i>
                            </button>
                        </div>
                    `;
                }
            }
        ],
       
        destroy: true,
        responsive: true,
        pageLength: 10,
       
        language: {
            search: "Search screens:",
            emptyTable: "No screens found for this theatre. Click 'Add New Screen' to configure seating."
        },
        initComplete: function (settings, json) {
           
            if (json && json.length > 0) {
                noDataMessage.addClass('d-none');
            } else {
                noDataMessage.removeClass('d-none');
            }
        }
    });
};

$(document).ready(function () {
    // NOTE: This runs once when the page loads
    if (document.getElementById('tblScreens')) {
        loadScreens();
    }
});
