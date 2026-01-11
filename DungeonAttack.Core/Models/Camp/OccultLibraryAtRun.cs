using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Camp;

/// <summary>
/// Gère les recettes accessibles pendant une run
/// </summary>
public class OccultLibraryAtRun(Hero hero)
{
    private readonly Hero _hero = hero;
    private readonly List<KeyValuePair<string, OccultLibraryRecipeData>> _accessibleRecipes = LoadAccessibleRecipes();

    public List<KeyValuePair<string, OccultLibraryRecipeData>> AccessibleRecipes => _accessibleRecipes;

    public KeyValuePair<string, OccultLibraryRecipeData>? FindRecipe(int index)
    {
        if (index >= 0 && index < _accessibleRecipes.Count)
            return _accessibleRecipes[index];
        return null;
    }

    /// <summary>
    /// Vérifie si le héros a tous les ingrédients pour une recette
    /// </summary>
    public bool HasIngredients(int index)
    {
        if (index < 0 || index >= _accessibleRecipes.Count)
            return false;

        OccultLibraryRecipeData recipe = _accessibleRecipes[index].Value;
        return recipe.Recipe.All(ingredient =>
            _hero.Ingredients.TryGetValue(ingredient.Key, out int count) && count >= ingredient.Value);
    }

    /// <summary>
    /// Charge les recettes accessibles depuis les sauvegardes
    /// </summary>
    private static List<KeyValuePair<string, OccultLibraryRecipeData>> LoadAccessibleRecipes()
    {
        try
        {
            Dictionary<string, OccultLibraryRecipeData> libraryData = YamlLoader.Load<Dictionary<string, OccultLibraryRecipeData>>("Data/camp/occult_library.yml");

            OccultLibrarySave savedRecipes = YamlLoader.LoadOrDefault<OccultLibrarySave>("Saves/occult_library.yml");

            return [.. libraryData
                .Where(kvp => savedRecipes.Recipes.GetValueOrDefault(kvp.Key, false))
                .OrderBy(kvp => kvp.Key)];
        }
        catch
        {
            return [];
        }
    }

    #region View Methods (pour le template YAML)

    /// <summary>
    /// Retourne "[Enter X]" pour afficher/améliorer une recette
    /// </summary>
    public string GetShowButton(int index)
    {
        if (index <= 0 || index > _accessibleRecipes.Count)
            return "";
        return $"[Enter {(char)(index + 64)}]";
    }

    /// <summary>
    /// Retourne le nom de la recette
    /// </summary>
    public string GetName(int index)
    {
        if (index <= 0 || index > _accessibleRecipes.Count)
            return "";
        return _accessibleRecipes[index - 1].Value.Name;
    }

    /// <summary>
    /// Retourne "YES" ou "NO" selon si le héros a les ingrédients
    /// </summary>
    public string GetHasIngredients(int index)
    {
        if (index <= 0 || index > _accessibleRecipes.Count)
            return "";
        return HasIngredients(index - 1) ? "YES" : "NO";
    }

    #endregion
}

/// <summary>
/// Structure de sauvegarde pour les recettes achetées
/// </summary>
public class OccultLibrarySave
{
    public Dictionary<string, bool> Recipes { get; set; } = [];
}
