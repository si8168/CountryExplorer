using CountryExplorer.Models;

namespace CountryExplorer.Views;

public partial class CountryDetailsPage : ContentPage
{
    public CountryDetailsPage(Country country)
    {
        InitializeComponent();
        BindingContext = country;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}