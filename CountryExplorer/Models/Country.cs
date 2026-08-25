using System.Text.Json.Serialization;

namespace CountryExplorer.Models;

public class Country
{
    [JsonPropertyName("name")]
    public CountryName? NameData { get; set; }

    [JsonPropertyName("capital")]
    public List<string>? CapitalList { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("subregion")]
    public string Subregion { get; set; } = string.Empty;

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("flags")]
    public CountryFlags? Flags { get; set; }

    [JsonPropertyName("currencies")]
    public Dictionary<string, CurrencyDetail>? Currencies { get; set; }

    [JsonPropertyName("languages")]
    public Dictionary<string, string>? Languages { get; set; }

    // Display helpers for UI bindings
    public string CommonName => NameData?.Common ?? "Unknown Country";
    public string OfficialName => NameData?.Official ?? "N/A";
    public string CapitalDisplay => (CapitalList != null && CapitalList.Count > 0) ? string.Join(", ", CapitalList) : "N/A";
    public string FlagUrl => Flags?.Png ?? Flags?.Svg ?? string.Empty;
    public string FormattedPopulation => Population.ToString("N0");

    public string CurrenciesDisplay
    {
        get
        {
            if (Currencies == null || Currencies.Count == 0) return "N/A";
            var currencyItems = Currencies.Values
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => string.IsNullOrEmpty(c.Symbol) ? c.Name : $"{c.Name} ({c.Symbol})");
            return string.Join(", ", currencyItems);
        }
    }

    public string LanguagesDisplay
    {
        get
        {
            if (Languages == null || Languages.Count == 0) return "N/A";
            return string.Join(", ", Languages.Values);
        }
    }
}

public class CountryName
{
    [JsonPropertyName("common")]
    public string Common { get; set; } = string.Empty;

    [JsonPropertyName("official")]
    public string Official { get; set; } = string.Empty;
}

public class CountryFlags
{
    [JsonPropertyName("png")]
    public string Png { get; set; } = string.Empty;

    [JsonPropertyName("svg")]
    public string Svg { get; set; } = string.Empty;
}

public class CurrencyDetail
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
}