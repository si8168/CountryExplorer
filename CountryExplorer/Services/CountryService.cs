using System.Diagnostics;
using System.Text.Json;
using CountryExplorer.Models;

namespace CountryExplorer.Services;

public class CountryService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://restcountries.com/v3.1/all?fields=name,capital,region,subregion,population,flags,currencies,languages";

    public CountryService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<List<Country>> GetCountriesAsync()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var countries = JsonSerializer.Deserialize<List<Country>>(json);
                return countries ?? new List<Country>();
            }

            Debug.WriteLine($"API Request failed with status code: {response.StatusCode}");
            return new List<Country>();
        }
        catch (HttpRequestException netEx)
        {
            Debug.WriteLine($"Network error while fetching country data: {netEx.Message}");
            throw new Exception("Unable to reach the server. Please check your internet connection.");
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("Request timed out.");
            throw new Exception("The connection timed out while loading data. Please try again.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected error in CountryService: {ex.Message}");
            throw new Exception("Failed to process country information.");
        }
    }
}