using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using CountryExplorer.Models;

namespace CountryExplorer.Services;

public class CountryService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://restcountries.com/v3.1/all?fields=name,capital,region,subregion,population,flags";

    public CountryService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CountryExplorerApp");
    }

    public async Task<List<Country>> GetCountriesAsync()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var parsedList = ParseCountriesFromJson(json);
                if (parsedList.Count > 0) return parsedList;
            }

            Debug.WriteLine($"API Request returned status {response.StatusCode}. Loading fallback dataset.");
            return GetExpandedCountryDataset();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Network or API exception: {ex.Message}. Loading fallback dataset.");
            return GetExpandedCountryDataset();
        }
    }

    private List<Country> ParseCountriesFromJson(string json)
    {
        var countryList = new List<Country>();
        try
        {
            var jsonArray = JsonNode.Parse(json)?.AsArray();
            if (jsonArray == null) return countryList;

            foreach (var node in jsonArray)
            {
                if (node == null) continue;

                var country = new Country();

                // Name
                var nameNode = node["name"];
                country.NameData = new CountryName
                {
                    Common = nameNode?["common"]?.ToString() ?? "Unknown",
                    Official = nameNode?["official"]?.ToString() ?? "N/A"
                };

                // Region & Subregion
                country.Region = node["region"]?.ToString() ?? "Unknown";
                country.Subregion = node["subregion"]?.ToString() ?? "N/A";

                // Population
                if (long.TryParse(node["population"]?.ToString(), out long pop))
                {
                    country.Population = pop;
                }

                // Flags
                var flagsNode = node["flags"];
                country.Flags = new CountryFlags
                {
                    Png = flagsNode?["png"]?.ToString() ?? string.Empty,
                    Svg = flagsNode?["svg"]?.ToString() ?? string.Empty
                };

                // Capital
                var capitalArray = node["capital"]?.AsArray();
                if (capitalArray != null)
                {
                    country.CapitalList = capitalArray
                        .Select(c => c?.ToString() ?? string.Empty)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                }

                countryList.Add(country);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during JSON parsing: {ex.Message}");
        }

        return countryList;
    }

    // Comprehensive multi-region dataset for testing search, sorting, and region filters
    private List<Country> GetExpandedCountryDataset()
    {
        return new List<Country>
        {
            // --- OCEANIA ---
            new Country
            {
                NameData = new CountryName { Common = "New Zealand", Official = "New Zealand" },
                CapitalList = new List<string> { "Wellington" },
                Region = "Oceania", Subregion = "Polynesia", Population = 5124100,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/nz.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Australia", Official = "Commonwealth of Australia" },
                CapitalList = new List<string> { "Canberra" },
                Region = "Oceania", Subregion = "Australia and New Zealand", Population = 25687041,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/au.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Fiji", Official = "Republic of Fiji" },
                CapitalList = new List<string> { "Suva" },
                Region = "Oceania", Subregion = "Melanesia", Population = 896444,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/fj.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Samoa", Official = "Independent State of Samoa" },
                CapitalList = new List<string> { "Apia" },
                Region = "Oceania", Subregion = "Polynesia", Population = 202506,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/ws.png" }
            },

            // --- ASIA ---
            new Country
            {
                NameData = new CountryName { Common = "Japan", Official = "Japan" },
                CapitalList = new List<string> { "Tokyo" },
                Region = "Asia", Subregion = "Eastern Asia", Population = 125836021,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/jp.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Vietnam", Official = "Socialist Republic of Vietnam" },
                CapitalList = new List<string> { "Hanoi" },
                Region = "Asia", Subregion = "South-Eastern Asia", Population = 97338583,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/vn.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "South Korea", Official = "Republic of Korea" },
                CapitalList = new List<string> { "Seoul" },
                Region = "Asia", Subregion = "Eastern Asia", Population = 51780579,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/kr.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Singapore", Official = "Republic of Singapore" },
                CapitalList = new List<string> { "Singapore" },
                Region = "Asia", Subregion = "South-Eastern Asia", Population = 5685807,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/sg.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Thailand", Official = "Kingdom of Thailand" },
                CapitalList = new List<string> { "Bangkok" },
                Region = "Asia", Subregion = "South-Eastern Asia", Population = 69799978,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/th.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "India", Official = "Republic of India" },
                CapitalList = new List<string> { "New Delhi" },
                Region = "Asia", Subregion = "Southern Asia", Population = 1380004385,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/in.png" }
            },

            // --- EUROPE ---
            new Country
            {
                NameData = new CountryName { Common = "France", Official = "French Republic" },
                CapitalList = new List<string> { "Paris" },
                Region = "Europe", Subregion = "Western Europe", Population = 67391582,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/fr.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Germany", Official = "Federal Republic of Germany" },
                CapitalList = new List<string> { "Berlin" },
                Region = "Europe", Subregion = "Western Europe", Population = 83240525,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/de.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Italy", Official = "Italian Republic" },
                CapitalList = new List<string> { "Rome" },
                Region = "Europe", Subregion = "Southern Europe", Population = 59554023,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/it.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Spain", Official = "Kingdom of Spain" },
                CapitalList = new List<string> { "Madrid" },
                Region = "Europe", Subregion = "Southern Europe", Population = 47351567,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/es.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "United Kingdom", Official = "United Kingdom of Great Britain and Northern Ireland" },
                CapitalList = new List<string> { "London" },
                Region = "Europe", Subregion = "Northern Europe", Population = 67215293,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/gb.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Norway", Official = "Kingdom of Norway" },
                CapitalList = new List<string> { "Oslo" },
                Region = "Europe", Subregion = "Northern Europe", Population = 5379475,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/no.png" }
            },

            // --- AMERICAS ---
            new Country
            {
                NameData = new CountryName { Common = "Canada", Official = "Canada" },
                CapitalList = new List<string> { "Ottawa" },
                Region = "Americas", Subregion = "North America", Population = 38005238,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/ca.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "United States", Official = "United States of America" },
                CapitalList = new List<string> { "Washington, D.C." },
                Region = "Americas", Subregion = "North America", Population = 329484123,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/us.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Brazil", Official = "Federative Republic of Brazil" },
                CapitalList = new List<string> { "Brasília" },
                Region = "Americas", Subregion = "South America", Population = 212559409,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/br.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Mexico", Official = "United Mexican States" },
                CapitalList = new List<string> { "Mexico City" },
                Region = "Americas", Subregion = "North America", Population = 128932753,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/mx.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Argentina", Official = "Argentine Republic" },
                CapitalList = new List<string> { "Buenos Aires" },
                Region = "Americas", Subregion = "South America", Population = 45376763,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/ar.png" }
            },

            // --- AFRICA ---
            new Country
            {
                NameData = new CountryName { Common = "Egypt", Official = "Arab Republic of Egypt" },
                CapitalList = new List<string> { "Cairo" },
                Region = "Africa", Subregion = "Northern Africa", Population = 102334403,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/eg.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "South Africa", Official = "Republic of South Africa" },
                CapitalList = new List<string> { "Pretoria" },
                Region = "Africa", Subregion = "Southern Africa", Population = 59308690,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/za.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Kenya", Official = "Republic of Kenya" },
                CapitalList = new List<string> { "Nairobi" },
                Region = "Africa", Subregion = "Eastern Africa", Population = 53771300,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/ke.png" }
            },
            new Country
            {
                NameData = new CountryName { Common = "Morocco", Official = "Kingdom of Morocco" },
                CapitalList = new List<string> { "Rabat" },
                Region = "Africa", Subregion = "Northern Africa", Population = 36910558,
                Flags = new CountryFlags { Png = "https://flagcdn.com/w320/ma.png" }
            }
        };
    }
}