using Newtonsoft.Json;

namespace CoworkingApp.Services;

public record GeocodeResult(decimal Latitude, decimal Longitude);

public interface IGeocodingService
{
    Task<GeocodeResult?> GeocodeAsync
        (
            string street,
            string district,
            string city,
            string postalCode,
            string country
        );
}

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;

    public NominatimGeocodingService(HttpClient http)
    {
        _http = http;

        // Required by Nominatim’s usage policy:
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0 (admin@myapp.com)");
    }

    public async Task<GeocodeResult?> GeocodeAsync
        (
            string street,
            string district,
            string city,
            string postalCode,
            string country
        )
    {
        var q = Uri.EscapeDataString($"{street}, {district}, {city}, {postalCode}, {country}");
        var response = await _http.GetStringAsync($"https://nominatim.openstreetmap.org/search?format=json&q={q}");
        var geoDtos = JsonConvert.DeserializeObject<List<GeoDto>>(response);

        if (geoDtos == null)
        {
            throw new Exception("Failed to deserialize geocoding response.");
        }

        if (geoDtos.Count > 0)
        {
            return new GeocodeResult(Latitude: decimal.Parse(geoDtos[0].Latitude), Longitude: decimal.Parse(geoDtos[0].Longitude));
        }

        return null;
    }

    private class GeoDto 
    { 
        public string Latitude { get; set; } = string.Empty; 
        public string Longitude { get; set; } = string.Empty; 
    }
}

