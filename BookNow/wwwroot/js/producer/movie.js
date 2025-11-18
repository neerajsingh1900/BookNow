(function () {
    let dataTable;

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {

        if ($.fn.DataTable.isDataTable('#tblData')) {
            $('#tblData').DataTable().destroy();
        }

        dataTable = $('#tblData').DataTable({
            "ajax": {
                "url": "/api/Producer/MovieApi",
                "type": "GET",
                "datatype": "json",
                "dataSrc": ""
            },
            "columns": [
                {
                    "data": "posterUrl",
                    "render": function (data) {
                        return data
                            ? `<img src="${data}" style="width:50px;height:auto;" />`
                            : '<span class="text-muted">No Image</span>';
                    },
                    "width": "10%"
                },

                { "data": "title", "width": "20%" },
                { "data": "genre", "width": "10%" },
                { "data": "language", "width": "10%" },
                { "data": "duration", "width": "10%" },
                {
                    "data": "releaseDate",
                    "render": function (data) {
                        return data
                            ? new Date(data).toLocaleDateString('en-US', {
                                year: 'numeric',
                                month: 'short',
                                day: 'numeric'
                            })
                            : "";
                    },
                    "width": "15%"
                },
                {
                    "data": null, 
                    "render": function (data, type, row) {
                        const movieId = row.movieId;
                        
                        const canEditAndDelete = row.canDeleteOrEditCriticalFields;

                      
                        let editButton;

                        if (canEditAndDelete) {
                           
                            editButton = `
                                <a href="/Producer/Movie/Upsert?id=${movieId}" class="btn btn-sm btn-primary me-2">
                                    <i class="fas fa-edit"></i> 
                                </a>`;
                        } else {
                           
                            editButton = `
                                <button class="btn btn-sm btn-primary me-2" 
                                        title="Editing is restricted due to existing shows." 
                                        onclick="ShowRestrictionPopUp('edit')">
                                    <i class="fas fa-lock"></i> 
                                </button>`;
                        }

                     
                        let deleteButton;

                        if (canEditAndDelete) {
                           
                            deleteButton = `
                                <button class="btn btn-sm btn-danger" onclick="DeleteMovie(${movieId})">
                                    <i class="fas fa-trash-alt"></i> 
                                </button>`;
                        } else {
                            
                            deleteButton = `
                                <button class="btn btn-sm btn-danger" 
                                        title="Deletion is restricted due to existing shows." 
                                        onclick="ShowRestrictionPopUp('delete')">
                                    <i class="fas fa-lock"></i> 
                                </button>`;
                        }

                        return `
                            <div class="d-flex justify-content-center">
                                ${editButton}
                                ${deleteButton}
                            </div>`;
                    },
                    "width": "20%"
                }
            ],
            "responsive": true,
            "pagingType": "simple_numbers",
            "language": {
                "emptyTable": "No movies found for this Producer."
            }
        });
    }

   
    window.ShowRestrictionPopUp = function (actionType) {
        let title, text, icon;

        if (actionType === 'delete') {
            title = 'Deletion Blocked 🛑';
            text = "This movie cannot be deleted because it has been listed for shows. You must retain the record for auditing and analytics.";
            icon = 'info';
        } else if (actionType === 'edit') {
            title = 'Editing Restricted 🔒';
            text = "This movie has been listed for a show and its details cannot be modified. Access to the edit page is blocked.";
            icon = 'warning';
        }

        Swal.fire({
            title: title,
            text: text,
            icon: icon,
            confirmButtonColor: '#007bff',
            confirmButtonText: 'Understood'
        });
    };


    window.DeleteMovie = function (movieId) {
        if (!movieId) return;

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#E53935',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "DELETE",
                    url: `/api/Producer/MovieApi/${movieId}`,
                    success: function () {
                        dataTable.ajax.reload();
                        Swal.fire(
                            'Deleted!',
                            'Movie has been deleted successfully.',
                            'success'
                        );
                    },
                    error: function (xhr) {
                        let errorMessage = 'Failed to delete movie: ' + xhr.responseText;

                         if (xhr.status === 403 || xhr.status === 409) {
                            errorMessage = 'Deletion blocked: This movie has historical show data.';
                        }

                        Swal.fire(
                            'Error!',
                            errorMessage,
                            'error'
                        );
                    }
                });
            }
        });
    };
})();