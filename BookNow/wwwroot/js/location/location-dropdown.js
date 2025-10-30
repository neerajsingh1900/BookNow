const LOCATION_API = "/api/location";
const LOCATION_SAVE_URL = "/api/locationdata/set";

document.addEventListener("DOMContentLoaded", function () {
    const $country = $('#CountryIdHeader');
    const $city = $('#CityIdHeader');
    const $applyBtn = $('#applyLocationBtn');
    const $cityText = $('#selectedCityText');

   
    $country.on('change', function () {
        const countryId = $(this).val();
        $city.prop('disabled', true).html('<option>Loading cities...</option>');
        $applyBtn.prop('disabled', true);

        if (!countryId) {
            $city.html('<option value="">-- Select City --</option>');
            return;
        }

        fetch(`${LOCATION_API}/cities/${countryId}`)
            .then(res => res.json())
            .then(cities => {
                $city.prop('disabled', false).html('<option value="">-- Select City --</option>');
                cities.forEach(c => {
                    $city.append(`<option value="${c.cityId}">${c.name}</option>`);
                });
            })
            .catch(() => {
                $city.html('<option>Error loading cities</option>');
            });
    });

   
    $city.on('change', function () {
        $applyBtn.prop('disabled', !$(this).val());
    });

    // --- Save location ---
    $applyBtn.on('click', function () {
        const cityId = $city.val();
        const cityName = $city.find('option:selected').text();

        if (!cityId) return;

        const formData = new FormData();
        formData.append("cityId", cityId);
        formData.append("cityName", cityName);

        fetch(LOCATION_SAVE_URL, { method: "POST", body: formData })
            .then(res => {
                if (res.ok) {
                    window.location.reload();
                } else {
                    console.error("Failed to save location");
                }
            })
            .catch(err => console.error("Error:", err));
    });
});

