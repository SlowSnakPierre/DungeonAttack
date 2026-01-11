using DungeonAttack.Factories;
using DungeonAttack.Models.Characters;
using DungeonAttack.Renderers;

namespace DungeonAttack.Controllers.Ammunition;

/// <summary>
/// Affichage détaillé des pièces d'équipement
/// </summary>
public static class AmmunitionShow
{
    public static void ShowWeaponButtonsActions(string distribution, object character)
    {
        if (!"ABCDE".Contains(distribution, StringComparison.CurrentCultureIgnoreCase))
            return;

        string ammunitionType = distribution.ToUpper() switch
        {
            "A" => "weapon",
            "B" => "head_armor",
            "C" => "body_armor",
            "D" => "arms_armor",
            "E" => "shield",
            _ => ""
        };

        if (string.IsNullOrEmpty(ammunitionType))
            return;

        Models.Ammunition.Ammunition? ammunitionObj = null;

        if (character is Hero hero)
        {
            ammunitionObj = ammunitionType switch
            {
                "weapon" => hero.Weapon,
                "head_armor" => hero.HeadArmor,
                "body_armor" => hero.BodyArmor,
                "arms_armor" => hero.ArmsArmor,
                "shield" => hero.Shield,
                _ => null
            };
        }
        else if (character is Enemy enemy)
        {
            ammunitionObj = ammunitionType switch
            {
                "weapon" => enemy.Weapon,
                "head_armor" => enemy.HeadArmor,
                "body_armor" => enemy.BodyArmor,
                "arms_armor" => enemy.ArmsArmor,
                "shield" => enemy.Shield,
                _ => null
            };
        }

        if (ammunitionObj != null && ammunitionObj.Code != "without")
        {
            Display(ammunitionObj, ammunitionType, artObj: ammunitionObj);
        }
    }

    public static void Display(Models.Ammunition.Ammunition? obj = null, string? type = null,
        string? code = null, object? artObj = null)
    {
        if (type == null) return;

        Models.Ammunition.Ammunition ammunitionObj;

        if (obj != null)
        {
            ammunitionObj = obj;
        }
        else if (code != null)
        {
            ammunitionObj = AmmunitionFactory.Create(type, code);
        }
        else
        {
            return;
        }

        string menuName = $"ammunition_{type}_screen";
        MainRenderer renderer = new(menuName, entity: ammunitionObj);
        renderer.AddArt("normal", artObj ?? ammunitionObj);
        renderer.RenderScreen();
        Console.ReadLine();
    }
}
