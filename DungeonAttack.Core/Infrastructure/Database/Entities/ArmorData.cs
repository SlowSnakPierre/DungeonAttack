namespace DungeonAttack.Infrastructure.Database.Entities;

/// <summary>
/// Entité pour les données d'armure (body, head, arms)
/// </summary>
public class ArmorData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AmmunitionType { get; set; } = string.Empty;

    public int Price { get; set; }
    public int Accuracy { get; set; }
    public int Armor { get; set; }
    public int Mp { get; set; }
    public int BlockChance { get; set; }
    public int ArmorPenetration { get; set; }

    public int EnhanceAccuracy { get; set; }
    public int EnhanceArmor { get; set; }
    public int EnhanceMp { get; set; }
    public int EnhanceBlockChance { get; set; }
    public int EnhanceArmorPenetration { get; set; }
}
