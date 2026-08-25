using CountryExplorer.Models;
using CountryExplorer.Services;

namespace CountryExplorer.Views;

public partial class CountriesPage : ContentPage
{
    private readonly CountryService _countryService;
    private List<Country> _allCountries = new();
    private bool _showOnlyFavorites = false;

    public CountriesPage()
    {
        InitializeComponent();
        _countryService = new CountryService();
        RegionPicker.SelectedIndex = 0;
        SortPicker.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_allCountries.Count == 0)
        {
            await LoadCountryDataAsync();
        }
        else
        {
            ApplyFilters();
        }
    }

    private async Task LoadCountryDataAsync()
    {
        ShowLoadingState(true);
        StatusMessageView.IsVisible = false;

        try
        {
            _allCountries = await _countryService.GetCountriesAsync();

            if (_allCountries.Count == 0)
            {
                ShowErrorMessage("No country records were returned from the API.");
            }
            else
            {
                ApplyFilters();
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
        finally
        {
            ShowLoadingState(false);
            CountryRefreshView.IsRefreshing = false;
        }
    }

    private void ApplyFilters()
    {
        if (_allCountries == null || _allCountries.Count == 0) return;

        string searchText = CountrySearchBar.Text?.Trim().ToLower() ?? string.Empty;
        string selectedRegion = RegionPicker.SelectedItem?.ToString() ?? "All Regions";
        string sortOption = SortPicker.SelectedItem?.ToString() ?? "Name (A-Z)";

        var filtered = _allCountries.AsEnumerable();

        if (_showOnlyFavorites)
        {
            var favList = FavoritesService.GetFavorites();
            filtered = filtered.Where(c => favList.Contains(c.CommonName));
        }

        if (!string.IsNullOrEmpty(selectedRegion) && selectedRegion != "All Regions")
        {
            filtered = filtered.Where(c => c.Region.Equals(selectedRegion, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            filtered = filtered.Where(c =>
                c.CommonName.ToLower().Contains(searchText) ||
                c.OfficialName.ToLower().Contains(searchText));
        }

        filtered = sortOption switch
        {
            "Name (Z-A)" => filtered.OrderByDescending(c => c.CommonName),
            "Population (High-Low)" => filtered.OrderByDescending(c => c.Population),
            "Population (Low-High)" => filtered.OrderBy(c => c.Population),
            _ => filtered.OrderBy(c => c.CommonName)
        };

        var resultList = filtered.ToList();
        CountriesCollectionView.ItemsSource = resultList;

        // Dynamic stats calculations
        long totalPop = resultList.Sum(c => c.Population);
        ResultCountLabel.Text = $"Showing {resultList.Count} of {_allCountries.Count} countries";
        TotalPopulationLabel.Text = $"Total Pop: {totalPop:N0}";

        if (resultList.Count == 0)
        {
            StatusMessageLabel.Text = _showOnlyFavorites ? "No favorite countries saved yet." : "No countries match your search criteria.";
            StatusMessageView.IsVisible = true;
            CountriesCollectionView.IsVisible = false;
        }
        else
        {
            StatusMessageView.IsVisible = false;
            CountriesCollectionView.IsVisible = true;
        }
    }

    private void OnFavoriteButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Country country)
        {
            FavoritesService.ToggleFavorite(country.CommonName);
            ApplyFilters();
        }
    }

    private void OnToggleFavoritesFilterClicked(object sender, EventArgs e)
    {
        _showOnlyFavorites = !_showOnlyFavorites;
        FavFilterToolbarItem.Text = _showOnlyFavorites ? "🌐 Show All" : "❤️ Favorites Only";
        ApplyFilters();
    }

    private void OnSearchOrFilterChanged(object sender, EventArgs e) => ApplyFilters();
    private async void OnReloadClicked(object sender, EventArgs e) => await LoadCountryDataAsync();
    private async void OnRefreshRequested(object sender, EventArgs e) => await LoadCountryDataAsync();

    private async void OnRandomCountryClicked(object sender, EventArgs e)
    {
        if (_allCountries.Count == 0) return;
        var randomCountry = _allCountries[new Random().Next(_allCountries.Count)];
        await Navigation.PushAsync(new CountryDetailsPage(randomCountry));
    }

    private async void OnCountrySelected(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Country selectedCountry)
        {
            await Navigation.PushAsync(new CountryDetailsPage(selectedCountry));
        }
    }

    private void ShowLoadingState(bool isLoading)
    {
        LoadingView.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        CountriesCollectionView.IsVisible = !isLoading;
    }

    private void ShowErrorMessage(string message)
    {
        StatusMessageLabel.Text = message;
        StatusMessageView.IsVisible = true;
        CountriesCollectionView.IsVisible = false;
        ResultCountLabel.Text = "Unable to display countries";
    }
}