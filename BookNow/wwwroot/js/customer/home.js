const LOCATION_API = "/api/location";
const MOVIE_API = "/api/movies/bycity";

$(document).ready(function () {
    const $country = $('#CountryId');
    const $city = $('#CityId');
    const $movieList = $('#movieList');
    const $movieError = $('#movieError');

    // --- Load Cities on Country change ---
    $country.on('change', function () {
        const countryId = $(this).val();
        if (!countryId) {
            $city.prop('disabled', true).html('<option value="">-- Select City --</option>');
            $movieList.empty();
            return;
        }

        $city.prop('disabled', true).html('<option>Loading cities...</option>');

        fetch(`${LOCATION_API}/cities/${countryId}`)
            .then(res => res.json())
            .then(cities => {
                $city.prop('disabled', false).html('<option value="">-- Select City --</option>');
                cities.forEach(c => {
                    $city.append(`<option value="${c.id}">${c.name}</option>`);
                });
            })
            .catch(() => {
                $city.prop('disabled', true).html('<option>Error loading cities</option>');
            });

        $movieList.empty();
        $movieError.hide();
    });

    // --- Load Movies on City change ---
    $city.on('change', function () {
        const cityId = $(this).val();
        $movieList.empty();
        $movieError.hide();

        if (!cityId) return;

        fetch(`${MOVIE_API}/${cityId}`)
            .then(res => res.json())
            .then(movies => {
                if (!movies.length) {
                    $movieError.text('No movies found in selected city.').show();
                    return;
                }
               
                movies.forEach(m => {
                    $movieList.append(`
                        <div class="col-md-3 mb-4">
                            <div class="card shadow-sm h-100">
                                <img src="${m.posterUrl}" class="card-img-top" alt="${m.title}" style="height:350px; object-fit:cover;">
                                <div class="card-body">
                                    <h5 class="card-title">${m.title}</h5>
                                    <p class="card-text">${m.language} | ${m.genre} | ${m.duration} min</p>
                                    <a href="/Customer/ShowSearch/SelectTheatre/${m.movieId}" class="btn btn-success w-100">View Shows</a>
                                </div>
                            </div>
                        </div>
                    `);
                });
            })
            .catch(() => {
                $movieError.text('Error loading movies.').show();
            });
    });
});

