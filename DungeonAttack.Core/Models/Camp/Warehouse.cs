using DungeonAttack.Factories;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Camp;

/// <summary>
/// Entrepôt pour stocker l'équipement et les pièces entre les runs
/// </summary>
public class Warehouse
{
    private const string PATH = "Saves/warehouse.json";

    public int Coins { get; set; }
    public string Weapon { get; set; } = "without";
    public string BodyArmor { get; set; } = "without";
    public string HeadArmor { get; set; } = "without";
    public string ArmsArmor { get; set; } = "without";
    public string Shield { get; set; } = "without";

    /// <summary>
    /// Charge l'entrepôt depuis le fichier JSON
    /// Crée le fichier par défaut s'il n'existe pas
    /// </summary>
    public static Warehouse Load()
    {
        if (!File.Exists(PATH))
        {
            Warehouse warehouse = new();
            warehouse.Save();
            return warehouse;
        }

        return JsonLoader.LoadOrDefault<Warehouse>(PATH);
    }

    /// <summary>
    /// Sauvegarde l'entrepôt dans le fichier JSON
    /// </summary>
    public void Save()
    {
        JsonLoader.Save(PATH, this);
    }

    /// <summary>
    /// Ajoute les pièces du héros à l'entrepôt
    /// </summary>
    public void AddCoinsFrom(Hero hero)
    {
        Coins += hero.Coins;
        Save();
    }

    /// <summary>
    /// Retire des pièces de l'entrepôt
    /// </summary>
    public bool TakeCoinsFromWarehouse(int n)
    {
        if (Coins >= n)
        {
            Coins -= n;
            Save();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Ajoute un équipement à l'entrepôt
    /// </summary>
    public void AddAmmunitionToWarehouse(string ammunitionType, string ammunitionCode)
    {
        switch (ammunitionType)
        {
            case "weapon": Weapon = ammunitionCode; break;
            case "body_armor": BodyArmor = ammunitionCode; break;
            case "head_armor": HeadArmor = ammunitionCode; break;
            case "arms_armor": ArmsArmor = ammunitionCode; break;
            case "shield": Shield = ammunitionCode; break;
        }
        Save();
    }

    /// <summary>
    /// Transfère tout l'équipement de l'entrepôt au héros
    /// </summary>
    public void TakeAmmunitionBy(Hero hero)
    {
        if (Weapon != "without")
        {
            hero.Weapon = (Weapon)AmmunitionFactory.Create("weapon", Weapon);
            Weapon = "without";
        }
        if (BodyArmor != "without")
        {
            hero.BodyArmor = (BodyArmor)AmmunitionFactory.Create("body_armor", BodyArmor);
            BodyArmor = "without";
        }
        if (HeadArmor != "without")
        {
            hero.HeadArmor = (HeadArmor)AmmunitionFactory.Create("head_armor", HeadArmor);
            HeadArmor = "without";
        }
        if (ArmsArmor != "without")
        {
            hero.ArmsArmor = (ArmsArmor)AmmunitionFactory.Create("arms_armor", ArmsArmor);
            ArmsArmor = "without";
        }
        if (Shield != "without")
        {
            hero.Shield = (Shield)AmmunitionFactory.Create("shield", Shield);
            Shield = "without";
        }
        Save();
    }

    /// <summary>
    /// Retire un équipement spécifique de l'entrepôt
    /// </summary>
    public string TakeAmmunitionFromWarehouse(string ammunitionType)
    {
        string code = ammunitionType switch
        {
            "weapon" => Weapon,
            "body_armor" => BodyArmor,
            "head_armor" => HeadArmor,
            "arms_armor" => ArmsArmor,
            "shield" => Shield,
            _ => "without"
        };

        switch (ammunitionType)
        {
            case "weapon": Weapon = "without"; break;
            case "body_armor": BodyArmor = "without"; break;
            case "head_armor": HeadArmor = "without"; break;
            case "arms_armor": ArmsArmor = "without"; break;
            case "shield": Shield = "without"; break;
        }

        Save();
        return code;
    }

    public string Show(string type)
    {
        return type switch
        {
            "coins" => Coins.ToString(),
            "weapon" => Weapon,
            "body_armor" => BodyArmor,
            "head_armor" => HeadArmor,
            "arms_armor" => ArmsArmor,
            "shield" => Shield,
            _ => string.Empty
        };
    }
}
