/**
 * BookNow.Web/wwwroot/js/theatre/theatreOwner.js
 * Consolidated JavaScript logic for the Theatre Owner Area.
 * Handles dashboard loading, API calls, and cascading dropdowns.
 */

const API_ROOT = '/TheatreOwner/api/theatre';
const LOCATION_API = '/api/location';

/**
 * --- Dashboard Logic (Theatre/Index.cshtml) ---
 */

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
                // Throw error with server message if available
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

                    // Function to dynamically render the status badge
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

                // Update Summary Cards
                $('#totalTheatreCount').text(data.length);
                $('#pendingCount').text(pendingCount);
                $('#totalScreenCount').text(totalScreens);
            } else {
                $noDataMessage.removeClass('d-none');
            }
        })
        .catch(error => {
            console.error('Error fetching theatres:', error);
            $noDataMessage.html(`<p class="text-danger">Failed to load theatre data: ${error.message}</p>`).removeClass('d-none');
        });
}

/**
 * --- Upsert Logic (Theatre/Upsert.cshtml) ---
 */

function initializeTheatreUpsert() {
    const $citySelect = $('#CityId');
    const $countrySelect = $('#CountryId');
    const $form = $('#theatreUpsertForm');
    const $submitButton = $('#submitButton');
    const $validationSummary = $('#serverValidationSummary');

    // --- Cascading Dropdown Functions ---

    function loadCountries() {
     //   console.log('Loading countries...');

        fetch(`${LOCATION_API}/countries`)
            .then(response => response.json())
            .then(countries => {
                $countrySelect.empty().append('<option value="">-- Select Country --</option>');
                countries.forEach(c => {
                    $countrySelect.append($('<option>', {
                        value: c.id,
                        text: c.name
                    }));
                });
            })
            .catch(error => {
                console.error('Error loading countries:', error);
                $validationSummary.text('Failed to load countries list.').removeClass('d-none');
            });
    }

    function loadCities(countryId, selectedCityId = null) {
      //  console.log('Loading cities for countryId:', countryId);
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
                        value: c.id,
                        text: c.name,
                        selected: c.id === selectedCityId
                    }));
                });
                $citySelect.prop('disabled', false);
            })
            .catch(error => {
                console.error('Error loading cities:', error);
                $citySelect.empty().append('<option value="">Error loading cities</option>').prop('disabled', true);
            });
    }

    // --- Event Handlers ---

    // Country Change Event
    $countrySelect.on('change', function () {
        const countryId = $(this).val();
        //loadCities(countryId ? parseInt(countryId) : null);
        if (countryId) {
            loadCities(countryId); // pass string, let API handle parsing
        } else {
            loadCities(null); // resets city dropdown
        }
    });

    // Form Submission Handler
    $form.on('submit', function (e) {
        e.preventDefault();

        // Custom validation for mandatory CityId
        if (!$citySelect.val()) {
            $validationSummary.text('Please select both a Country and a City.').removeClass('d-none');
            return;
        }

        if (!$form.valid()) {
            return;
        }

        $submitButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Saving...');
        $validationSummary.addClass('d-none').text('');

        const isUpdate = $('#TheatreId').val();

        const formData = {
            theatreId: isUpdate ? parseInt(isUpdate) : null,
            theatreName: $('#TheatreName').val(),
            email: $('#Email').val(),
            phoneNumber: $('#PhoneNumber').val(),
            cityId: parseInt($citySelect.val()),
            address: $('#Address').val()
        };
        console.log("formdata:",formData);
        const url = API_ROOT;
        const method = isUpdate ? 'PUT' : 'POST';

        fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                // Always include anti-forgery token for POST/PUT requests
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(formData)
        })
            .then(async response => {
                $submitButton.prop('disabled', false).html('<i class="fas fa-save me-1"></i> ' + (isUpdate ? 'Update' : 'Register') + ' Theatre');
                if (response.ok) {
                    // Success: Redirect to dashboard
                    console.log('Operation successful.');
                    window.location.href = '/TheatreOwner/Theatre/Index';
                } else {
                    // Error handling
                    const errorData = await response.json();
                    throw new Error(errorData.message || 'An unexpected error occurred. Check server logs.');
                }
            })
            .catch(error => {
                console.error('API Error:', error);
                $validationSummary.text(error.message).removeClass('d-none');
            });
    });

    // --- Initialization ---
    loadCountries();
}

// Map the initialization functions to the correct pages when the document is ready
$(document).ready(function () {
    if (document.getElementById('tblTheatres')) {
        loadTheatreDashboard();
    }
    if (document.getElementById('theatreUpsertForm')) {
        initializeTheatreUpsert();
    }
});
