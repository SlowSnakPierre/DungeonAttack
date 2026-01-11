using DungeonAttack.Infrastructure;

namespace DungeonAttack.Models.Camp;

public class OccultLibrary
{
    private const string SAVE_PATH = "Saves/occult_library.yml";
    private const string DATA_PATH = "Data/camp/occult_library.yml";

    public Dictionary<string, bool> Recipes { get; set; } = [];

    private Dictionary<string, OccultLibraryRecipeData>? _libraryData;
    private Warehouse? _warehouse;

    public static OccultLibrary Load(Warehouse warehouse)
    {
        bool needsSave = !File.Exists(SAVE_PATH);

        OccultLibrary library = YamlLoader.LoadOrDefault<OccultLibrary>(SAVE_PATH);
        library._warehouse = warehouse;

        try
        {
            library._libraryData = YamlLoader.Load<Dictionary<string, OccultLibraryRecipeData>>(DATA_PATH);

            foreach (string key in library._libraryData.Keys)
            {
                if (!library.Recipes.ContainsKey(key))
                {
                    library.Recipes[key] = false;
                    needsSave = true;
                }
            }

            if (needsSave)
                library.Save();
        }
        catch
        {
            library._libraryData = [];
            if (needsSave)
                library.Save();
        }

        return library;
    }

    public void Save()
    {
        YamlLoader.Save(SAVE_PATH, this);
    }

    public bool CanSellThisRecipe(int n)
    {
        KeyValuePair<string, OccultLibraryRecipeData>? recipe = FindRecipeByViewCode(n);
        return recipe != null && !Recipes.GetValueOrDefault(recipe.Value.Key, true);
    }

    public bool CanShowThisRecipe(char c)
    {
        int index = c - 64;
        return _libraryData != null && index <= _libraryData.Count;
    }

    public void Sell(int n)
    {
        KeyValuePair<string, OccultLibraryRecipeData>? recipe = FindRecipeByViewCode(n);
        if (recipe == null) return;

        int price = recipe.Value.Value.Price;
        bool hasMoney = _warehouse?.TakeCoinsFromWarehouse(price) ?? false;

        if (hasMoney)
        {
            Recipes[recipe.Value.Key] = true;
            Save();
        }
    }

    public int GetCoins()
    {
        return _warehouse?.Coins ?? 0;
    }

    public string GetStatus(int i)
    {
        KeyValuePair<string, OccultLibraryRecipeData>? recipe = FindRecipeByViewCode(i);
        if (recipe == null) return string.Empty;

        return Recipes.GetValueOrDefault(recipe.Value.Key, false)
            ? "IN YOUR WAREHOUSE"
            : $"[Enter {i}]";
    }

    public static string GetShowButton(int i)
    {
        return $"[Enter {(char)(i + 64)}]";
    }

    public string GetPriceDisplay(int i)
    {
        KeyValuePair<string, OccultLibraryRecipeData>? recipe = FindRecipeByViewCode(i);
        if (recipe == null) return string.Empty;

        return Recipes.GetValueOrDefault(recipe.Value.Key, false)
            ? "SOLD"
            : recipe.Value.Value.Price.ToString();
    }

    public string GetName(int i)
    {
        KeyValuePair<string, OccultLibraryRecipeData>? recipe = FindRecipeByViewCode(i);
        if (recipe == null) return string.Empty;
        return recipe.Value.Value.Name;
    }

    public KeyValuePair<string, OccultLibraryRecipeData>? FindRecipeByViewCode(int n)
    {
        if (_libraryData == null) return null;

        foreach (KeyValuePair<string, OccultLibraryRecipeData> kvp in _libraryData)
        {
            if (kvp.Value.ViewCode == n)
            {
                return kvp;
            }
        }

        return null;
    }

    /// <summary>
    /// Retourne les recettes accessibles (achetées) triées par code
    /// </summary>
    public List<KeyValuePair<string, OccultLibraryRecipeData>> GetAccessibleRecipes()
    {
        if (_libraryData == null) return [];

        return _libraryData
            .Where(kvp => Recipes.GetValueOrDefault(kvp.Key, false))
            .OrderBy(kvp => kvp.Key)
            .ToList();
    }

    public Dictionary<string, OccultLibraryRecipeData>? LibraryData => _libraryData;
}

/// <summary>
/// Données d'une recette de l'OccultLibrary (chargées depuis YAML)
/// </summary>
public class OccultLibraryRecipeData
{
    public int ViewCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public Dictionary<string, int> Recipe { get; set; } = [];
    public Dictionary<string, Dictionary<string, int>> Effect { get; set; } = [];
}
