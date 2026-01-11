using DungeonAttack.Models.Ammunition;

namespace DungeonAttack.Factories;

/// <summary>
/// Factory pour créer des instances d'équipement selon le type
/// </summary>
public static class AmmunitionFactory
{
    /// <summary>
    /// Crée une instance d'équipement basée sur le type et le code
    /// </summary>
    /// <param name="ammunitionType">Type d'équipement (weapon, body_armor, head_armor, arms_armor, shield)</param>
    /// <param name="ammunitionCode">Code unique de l'équipement</param>
    /// <returns>Instance d'Ammunition du type approprié</returns>
    /// <exception cref="ArgumentException">Si le type est inconnu</exception>
    public static Ammunition Create(string ammunitionType, string ammunitionCode)
    {
        return ammunitionType switch
        {
            "weapon" => new Weapon(ammunitionCode),
            "body_armor" => new BodyArmor(ammunitionCode),
            "head_armor" => new HeadArmor(ammunitionCode),
            "arms_armor" => new ArmsArmor(ammunitionCode),
            "shield" => new Shield(ammunitionCode),
            _ => throw new ArgumentException($"Type d'équipement inconnu: {ammunitionType}")
        };
    }
}
