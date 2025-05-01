using Newtonsoft.Json;

namespace CoworkingApp.Services;

public record GeocodeResult(decimal Latitude, decimal Longitude);

public record ReverseGeocodeResult(
    string? Street,      // road + house number
    string? District,    // suburb, county
    string? City,        // city/town
    string? PostalCode,  // postcode
    string? Country      // country
);

file class NominatimAddressDto
{
    [JsonProperty("house_number")]
    public string? HouseNumber { get; set; }

    [JsonProperty("road")]
    public string? Road { get; set; }

    [JsonProperty("suburb")]
    public string? Suburb { get; set; }

    [JsonProperty("county")]
    public string? County { get; set; }

    [JsonProperty("city")]
    public string? City { get; set; }

    [JsonProperty("town")]
    public string? Town { get; set; }

    [JsonProperty("postcode")]
    public string? Postcode { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }
}

file class NominatimReverseDto
{
    [JsonProperty("address")]
    public NominatimAddressDto? Address { get; set; }
}

public interface IGeocodingService
{
    Task<GeocodeResult?> Geocode
        (
            string street,
            string district,
            string city,
            string postalCode,
            string country
        );

    Task<ReverseGeocodeResult?> ReverseGeocode(decimal latitude, decimal longitude);
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

    public async Task<ReverseGeocodeResult?> ReverseGeocode(decimal latitude, decimal longitude)
    {
        var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}&addressdetails=1";

        using var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var reverseDto = await response.Content.ReadFromJsonAsync<NominatimReverseDto>();

        if (reverseDto?.Address == null)
        {
            return null;
        }

        return new ReverseGeocodeResult(
            Street:     string.Join(" ", 
                            from s in (new[] { reverseDto.Address.Road, reverseDto.Address.HouseNumber })  
                            where !string.IsNullOrWhiteSpace(s) select s),
            District:   reverseDto.Address.Suburb ?? reverseDto.Address.County,
            City:       reverseDto.Address.City ?? reverseDto.Address.Town,
            PostalCode: reverseDto.Address.Postcode,
            Country:    reverseDto.Address.Country
        );
    }


    public async Task<GeocodeResult?> Geocode
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

