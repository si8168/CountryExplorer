namespace CountryExplorer.Services;

public static class FavoritesService
{
    private const string FavoritesKey = "user_favorite_countries";

    public static List<string> GetFavorites()
    {
        string saved = Preferences.Get(FavoritesKey, string.Empty);
        if (string.IsNullOrEmpty(saved)) return new List<string>();
        return saved.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static bool ToggleFavorite(string countryName)
    {
        var favorites = GetFavorites();
        bool isFav;

        if (favorites.Contains(countryName))
        {
            favorites.Remove(countryName);
            isFav = false;
        }
        else
        {
            favorites.Add(countryName);
            isFav = true;
        }

        Preferences.Set(FavoritesKey, string.Join(",", favorites));
        return isFav;
    }

    public static bool IsFavorite(string countryName)
    {
        return GetFavorites().Contains(countryName);
    }
}