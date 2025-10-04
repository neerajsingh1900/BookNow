(function () {
    let dataTable;

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {
        // Destroy previous instance if exists
        if ($.fn.DataTable.isDataTable('#tblData')) {
            $('#tblData').DataTable().destroy();
        }

        dataTable = $('#tblData').DataTable({
            "ajax": {
                "url": "/api/Producer/MovieApi",  // API endpoint
                "type": "GET",
                "datatype": "json",
                "dataSrc": "" // <-- IMPORTANT: your API returns an array at root
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
                { "data": "movieId", "width": "5%" },
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
                    "data": "movieId",
                    "render": function (data) {
                        return `
                        <div class="d-flex justify-content-center">
                            <a href="/Producer/Movie/Upsert?id=${data}" class="btn btn-sm btn-primary me-2">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                            <button class="btn btn-sm btn-danger" onclick="DeleteMovie(${data})">
                                <i class="fas fa-trash-alt"></i> Delete
                            </button>
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

    // Expose Delete globally
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
                        Swal.fire(
                            'Error!',
                            'Failed to delete movie: ' + xhr.responseText,
                            'error'
                        );
                    }
                });
            }
        });
    };
})();
