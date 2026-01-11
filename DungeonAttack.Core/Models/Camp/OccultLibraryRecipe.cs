using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Camp;

/// <summary>
/// Représente une recette pour l'affichage et l'amélioration
/// </summary>
public class OccultLibraryRecipe(KeyValuePair<string, OccultLibraryRecipeData> data, Hero? hero = null)
{
    private readonly string _codeName = data.Key;
    private readonly OccultLibraryRecipeData _data = data.Value;
    private readonly Hero? _hero = hero;

    public string CodeName => _codeName;
    public int ViewCode => _data.ViewCode;
    public string Name => _data.Name;
    public int Price => _data.Price;
    public Dictionary<string, int> RecipeIngredients => _data.Recipe;

    /// <summary>
    /// Vérifie si le héros a tous les ingrédients nécessaires
    /// </summary>
    public bool HeroHasIngredients()
    {
        if (_hero == null)
            return false;

        return _data.Recipe.All(ingredient =>
            _hero.Ingredients.TryGetValue(ingredient.Key, out int count) && count >= ingredient.Value);
    }

    /// <summary>
    /// Retourne l'effet de la recette pour un type d'équipement
    /// </summary>
    public Dictionary<string, int>? EffectOf(string ammunitionType)
    {
        if (_data.Effect.TryGetValue(ammunitionType, out Dictionary<string, int>? effect))
            return effect;
        return null;
    }

    /// <summary>
    /// Consomme les ingrédients du héros
    /// </summary>
    public void ConsumeIngredients()
    {
        if (_hero == null)
            return;

        foreach (KeyValuePair<string, int> ingredient in _data.Recipe)
        {
            if (_hero.Ingredients.ContainsKey(ingredient.Key))
            {
                _hero.Ingredients[ingredient.Key] -= ingredient.Value;
                if (_hero.Ingredients[ingredient.Key] <= 0)
                    _hero.Ingredients.Remove(ingredient.Key);
            }
        }
    }

    #region View Methods (pour les templates YAML)

    /// <summary>
    /// Retourne les ingrédients du héros formatés
    /// </summary>
    public string GetHeroIngredients()
    {
        if (_hero == null || _hero.Ingredients.Count == 0)
            return "";

        return "Your ingredients:     " + FormatDictionary(_hero.Ingredients);
    }

    /// <summary>
    /// Retourne la recette formatée
    /// </summary>
    public string GetRecipeDisplay()
    {
        return FormatDictionary(_data.Recipe);
    }

    /// <summary>
    /// Retourne l'effet pour un type d'équipement formaté
    /// </summary>
    public string GetEffectDisplay(string ammunitionType)
    {
        if (!_data.Effect.TryGetValue(ammunitionType, out Dictionary<string, int>? effect))
            return "";
        return FormatDictionary(effect);
    }

    /// <summary>
    /// Méthodes pour accéder aux propriétés de l'équipement du héros
    /// </summary>
    public string GetHeroWeaponName() => _hero?.Weapon?.Name ?? "";
    public string GetHeroHeadArmorName() => _hero?.HeadArmor?.Name ?? "";
    public string GetHeroBodyArmorName() => _hero?.BodyArmor?.Name ?? "";
    public string GetHeroArmsArmorName() => _hero?.ArmsArmor?.Name ?? "";
    public string GetHeroShieldName() => _hero?.Shield?.Name ?? "";

    public bool HeroWeaponCanEnhance() => _hero?.Weapon != null && _hero.Weapon.Code != "without" && !_hero.Weapon.Enhance;
    public bool HeroHeadArmorCanEnhance() => _hero?.HeadArmor != null && _hero.HeadArmor.Code != "without" && !_hero.HeadArmor.Enhance;
    public bool HeroBodyArmorCanEnhance() => _hero?.BodyArmor != null && _hero.BodyArmor.Code != "without" && !_hero.BodyArmor.Enhance;
    public bool HeroArmsArmorCanEnhance() => _hero?.ArmsArmor != null && _hero.ArmsArmor.Code != "without" && !_hero.ArmsArmor.Enhance;
    public bool HeroShieldCanEnhance() => _hero?.Shield != null && _hero.Shield.Code != "without" && !_hero.Shield.Enhance;

    private static string FormatDictionary<T>(Dictionary<string, T> dict)
    {
        return string.Join(";   ", dict
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{FormatKey(kvp.Key)}: {kvp.Value}"));
    }

    private static string FormatKey(string key)
    {
        return string.Join(" ", key.Split('_').Select(word =>
            word.Length > 0 ? char.ToUpper(word[0]) + word[1..] : word));
    }

    #endregion
}
