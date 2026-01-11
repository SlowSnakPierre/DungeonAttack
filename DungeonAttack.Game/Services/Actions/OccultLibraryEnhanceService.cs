using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using AmmunitionBase = DungeonAttack.Models.Ammunition.Ammunition;

namespace DungeonAttack.Services.Actions;

/// <summary>
/// Service pour améliorer l'équipement avec les recettes de l'OccultLibrary
/// </summary>
public class OccultLibraryEnhanceService(
    Hero hero,
    AmmunitionBase ammunition,
    string ammunitionType,
    OccultLibraryRecipe recipe)
{
    private readonly Hero _hero = hero;
    private readonly AmmunitionBase _ammunition = ammunition;
    private readonly string _ammunitionType = ammunitionType;
    private readonly OccultLibraryRecipe _recipe = recipe;

    /// <summary>
    /// Applique l'amélioration à l'équipement
    /// </summary>
    public void AmmunitionEnhance()
    {
        if (_ammunition == null || _ammunition.Code == "without")
            return;

        if (_ammunition.Enhance)
            return;

        Dictionary<string, int>? effects = _recipe.EffectOf(_ammunitionType);
        if (effects == null)
            return;

        _ammunition.Enhance = true;
        _ammunition.EnhanceName = _recipe.Name;

        foreach (KeyValuePair<string, int> effect in effects)
        {
            switch (effect.Key.ToLower())
            {
                case "accuracy":
                    _ammunition.EnhanceAccuracy = effect.Value;
                    break;
                case "armor":
                    _ammunition.EnhanceArmor = effect.Value;
                    break;
                case "min_dmg":
                    _ammunition.EnhanceMinDmg = effect.Value;
                    break;
                case "max_dmg":
                    _ammunition.EnhanceMaxDmg = effect.Value;
                    break;
                case "block_chance":
                    _ammunition.EnhanceBlockChance = effect.Value;
                    break;
            }
        }
    }
}
