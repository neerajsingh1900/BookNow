const API_ROOT = '/TheatreOwner/api/theatre';
const LOCATION_API = '/api/location';


function loadTheatreDashboard() {
    const $loadingIndicator = $('#loadingIndicator');
    const $noDataMessage = $('#noDataMessage');
    const $tableBody = $('#tblTheatres tbody');

    $loadingIndicator.removeClass('d-none');
    $noDataMessage.addClass('d-none');
    $tableBody.empty();

    let totalScreens = 0;
    let pendingCount = 0;

    fetch(API_ROOT)
        .then(response => {
            $loadingIndicator.addClass('d-none');
            if (!response.ok) {
                
                return response.json().then(err => { throw new Error(err.message || 'Failed to load data.'); });
            }
            return response.json();
        })
        .then(data => {
            if (data && data.length > 0) {
                data.forEach(theatre => {
                    const screens = theatre.screenCount || 0;
                    totalScreens += screens;

                    if (theatre.status === 'PendingApproval') {
                        pendingCount++;
                    }

                   
                    const getStatusBadge = (status) => {
                        let badgeClass = 'bg-secondary';
                        if (status === 'PendingApproval') badgeClass = 'bg-warning text-dark';
                        else if (status === 'Active') badgeClass = 'bg-success';
                        else if (status === 'Maintenance') badgeClass = 'bg-danger';

                        return `<span class="badge rounded-pill px-3 py-2 ${badgeClass}">${status}</span>`;
                    };

                    const row = `
                        <tr>
                            <td>
                                <div class="fw-bold">${theatre.theatreName}</div>
                                <div class="text-muted small">${theatre.phoneNumber}</div>
                            </td>
                            <td>${theatre.cityName}, ${theatre.countryName}</td>
                            <td>${getStatusBadge(theatre.status)}</td>
                            <td>${screens}</td>
                            <td>
                                <a href="/TheatreOwner/Screen/Index/${theatre.theatreId}" class="btn btn-sm btn-outline-primary me-2">Screens</a>
                                <a href="/TheatreOwner/Theatre/Upsert/${theatre.theatreId}" class="btn btn-sm btn-outline-secondary">Edit</a>
                            </td>
                        </tr>
                    `;
                    $tableBody.append(row);
                });

               
                $('#totalTheatreCount').text(data.length);
                $('#pendingCount').text(pendingCount);
                $('#totalScreenCount').text(totalScreens);

                if (!$.fn.DataTable.isDataTable('#tblTheatres')) {
                    $('#tblTheatres').DataTable({
                        responsive: true,
                        pageLength: 10,
                        lengthMenu: [5, 10, 25, 50],
                        columnDefs: [
                            { orderable: false, targets: [4] } 
                        ]
                    });
                }
                else {
                    $('#tblTheatres').DataTable().clear().destroy();
                    $('#tblTheatres').DataTable({
                        responsive: true,
                        pageLength: 10,
                        lengthMenu: [5, 10, 25, 50],
                        columnDefs: [
                            { orderable: false, targets: [4] }
                        ]
                    });
                }

            }
            else {
                $noDataMessage.removeClass('d-none');
            }
        })
        .catch(error => {
            console.error('Error fetching theatres:', error);
            $noDataMessage.html(`<p class="text-danger">Failed to load theatre data: ${error.message}</p>`).removeClass('d-none');
        });
}


function initializeTheatreUpsert() {
    const $citySelect = $('#CityId');
    const $countrySelect = $('#CountryId');
    const $form = $('#theatreUpsertForm');
    const $submitButton = $('#submitButton');
    const $validationSummary = $('#serverValidationSummary');


    function loadCountries() {

   const selectedCountryId = window.theatreEditData ? window.theatreEditData.countryId : null;

        fetch(`${LOCATION_API}/countries`)
            .then(response => response.json())
            .then(countries => {
                $countrySelect.empty().append('<option value="">-- Select Country --</option>');
                countries.forEach(c => {
                    $countrySelect.append($('<option>', {
                        value: c.countryId,
                        text: c.name,
                        selected: c.countryId === selectedCountryId
                    }));
                });

                if (selectedCountryId && window.theatreEditData) {
                    loadCities(selectedCountryId, window.theatreEditData.CityId);
                }
            })
            .catch(error => {
                console.error('Error loading countries:', error);
                $validationSummary.text('Failed to load countries list.').removeClass('d-none');
            });
    }

    function loadCities(countryId, selectedCityId = null) {
      
        $citySelect.prop('disabled', true).empty().append('<option value="">Loading Cities...</option>');

        if (!countryId) {
            $citySelect.empty().append('<option value="">-- Select City --</option>').prop('disabled', true);
            return;
        }

        fetch(`${LOCATION_API}/cities/${countryId}`)
            .then(response => response.json())
            .then(cities => {
                $citySelect.empty().append('<option value="">-- Select City --</option>');
                cities.forEach(c => {
                    $citySelect.append($('<option>', {
                        value: c.cityId,
                        text: c.name,
                        selected: c.cityId === selectedCityId
                    }));
                });
                $citySelect.prop('disabled', false);
            })
            .catch(error => {
                console.error('Error loading cities:', error);
                $citySelect.empty().append('<option value="">Error loading cities</option>').prop('disabled', true);
            });
    }

   
    $countrySelect.on('change', function () {
        const countryId = $(this).val();
    
        if (countryId) {
            loadCities(countryId);
        } else {
            loadCities(null); 
        }
    });

    $('#theatreUpsertForm').on('submit', function () {
        const $submitButton = $('#submitButton');

       
        let valid = true;
        if (!$('#CountryId').val()) { $('#CountryId').addClass('is-invalid'); valid = false; }
        if (!$('#CityId').val()) { $('#CityId').addClass('is-invalid'); valid = false; }
        if (!valid) return false;

       
        $submitButton.prop('disabled', true)
            .html('<i class="fas fa-spinner fa-spin me-1"></i> Saving...');

        
    });
  
    loadCountries();
}




$(document).ready(function () {
    if (document.getElementById('tblTheatres')) {
        loadTheatreDashboard();
    }
    if (document.getElementById('theatreUpsertForm')) {
        initializeTheatreUpsert();
    }
});
