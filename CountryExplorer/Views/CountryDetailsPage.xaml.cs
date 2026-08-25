using CountryExplorer.Models;
using CountryExplorer.Services;

namespace CountryExplorer.Views;

public partial class CountryDetailsPage : ContentPage
{
    private readonly Country _country;

    public CountryDetailsPage(Country country)
    {
        InitializeComponent();
        _country = country;
        BindingContext = _country;
    }

    private void OnToggleFavoriteDetailsClicked(object sender, EventArgs e)
    {
        bool isFav = FavoritesService.ToggleFavorite(_country.CommonName);
        FavButton.Text = isFav ? "❤️ Fav" : "🤍 Fav";
    }

    private async void OnOpenMapClicked(object sender, EventArgs e)
    {
        string locationQuery = Uri.EscapeDataString($"{_country.CommonName} {_country.CapitalDisplay}");
        string mapUrl = $"https://www.google.com/maps/search/?api=1&query={locationQuery}";
        await Launcher.OpenAsync(new Uri(mapUrl));
    }

    private async void OnShareCountryClicked(object sender, EventArgs e)
    {
        string shareText = $"Explore {_country.CommonName}!\n" +
                           $"Capital: {_country.CapitalDisplay}\n" +
                           $"Region: {_country.Region} ({_country.Subregion})\n" +
                           $"Population: {_country.FormattedPopulation}";

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = shareText,
            Title = $"Share {_country.CommonName}"
        });
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}