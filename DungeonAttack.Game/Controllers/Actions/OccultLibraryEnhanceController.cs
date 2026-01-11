using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Actions;
using AmmunitionBase = DungeonAttack.Models.Ammunition.Ammunition;

namespace DungeonAttack.Controllers.Actions;

/// <summary>
/// Contrôleur pour améliorer l'équipement avec les recettes
/// </summary>
public class OccultLibraryEnhanceController(Hero hero)
{
    private readonly Hero _hero = hero;
    private readonly OccultLibraryAtRun _ebr = new(hero);
    private OccultLibraryRecipe? _recipe;

    /// <summary>
    /// Affiche la liste des recettes et gère la sélection
    /// </summary>
    public void RecipesList()
    {
        string? choose = null;

        while (choose != "0" && choose != "")
        {
            MainRenderer renderer = new("enhance_by_recipe_screen", entity: _ebr);
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper();

            if (!string.IsNullOrEmpty(choose) && choose.Length == 1)
            {
                char c = choose[0];
                if (c >= 'A' && c <= 'X')
                {
                    int index = c - 'A';
                    ShowRecipe(index);
                }
            }
        }
    }

    /// <summary>
    /// Affiche une recette spécifique
    /// </summary>
    private void ShowRecipe(int index)
    {
        if (index >= _ebr.AccessibleRecipes.Count)
            return;

        KeyValuePair<string, OccultLibraryRecipeData> recipeData = _ebr.AccessibleRecipes[index];
        _recipe = new OccultLibraryRecipe(recipeData, _hero);

        if (_ebr.HasIngredients(index))
        {
            ShowOrEnhanceAmmunition();
        }
        else
        {
            MainRenderer renderer = new("camp_ol_recipe_screen", entity: _recipe);
            renderer.RenderScreen();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Affiche l'écran d'amélioration et gère la sélection
    /// </summary>
    private void ShowOrEnhanceAmmunition()
    {
        if (_recipe == null)
            return;

        string? choose = null;

        while (choose != "0" && choose != "")
        {
            MainRenderer renderer = new("camp_ol_enhance_screen", entity: _recipe);
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(choose))
                continue;

            if (choose.Length == 1 && choose[0] >= 'A' && choose[0] <= 'E')
            {
                ShowAmmunition(choose[0]);
            }
            else if (choose.Length == 1 && choose[0] >= '1' && choose[0] <= '5')
            {
                int slot = choose[0] - '1';
                EnhanceAmmunition(slot);
            }
        }
    }

    /// <summary>
    /// Affiche les détails d'un équipement
    /// </summary>
    private void ShowAmmunition(char c)
    {
        (string? ammunitionType, AmmunitionBase? ammunitionObj) = c switch
        {
            'A' => ("weapon", _hero.Weapon),
            'B' => ("head_armor", _hero.HeadArmor),
            'C' => ("body_armor", _hero.BodyArmor),
            'D' => ("arms_armor", _hero.ArmsArmor),
            'E' => ("shield", (AmmunitionBase?)_hero.Shield),
            _ => ("", null)
        };

        if (ammunitionObj != null)
        {
            AmmunitionShow.Display(ammunitionObj, ammunitionType);
        }
    }

    /// <summary>
    /// Améliore un équipement avec la recette actuelle
    /// </summary>
    private void EnhanceAmmunition(int slot)
    {
        if (_recipe == null || !_recipe.HeroHasIngredients())
            return;

        (string? ammunitionType, AmmunitionBase? ammunitionObj) = slot switch
        {
            0 => ("weapon", _hero.Weapon),
            1 => ("head_armor", _hero.HeadArmor),
            2 => ("body_armor", _hero.BodyArmor),
            3 => ("arms_armor", _hero.ArmsArmor),
            4 => ("shield", (AmmunitionBase?)_hero.Shield),
            _ => ("", null)
        };

        if (ammunitionObj == null || ammunitionObj.Code == "without")
            return;

        if (ammunitionObj.Enhance)
            return;

        OccultLibraryEnhanceService service = new(_hero, ammunitionObj, ammunitionType, _recipe);
        service.AmmunitionEnhance();

        _recipe.ConsumeIngredients();
    }
}
