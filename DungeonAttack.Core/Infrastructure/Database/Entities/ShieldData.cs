namespace DungeonAttack.Infrastructure.Database.Entities;

/// <summary>
/// Entité pour les données de boucliers
/// </summary>
public class ShieldData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AmmunitionType { get; set; } = "shield";

    public int Price { get; set; }
    public int MinDmg { get; set; }
    public int MaxDmg { get; set; }
    public int Accuracy { get; set; }
    public int Armor { get; set; }
    public int Mp { get; set; }
    public int BlockChance { get; set; }
    public int ArmorPenetration { get; set; }

    public int EnhanceMinDmg { get; set; }
    public int EnhanceMaxDmg { get; set; }
    public int EnhanceAccuracy { get; set; }
    public int EnhanceArmor { get; set; }
    public int EnhanceMp { get; set; }
    public int EnhanceBlockChance { get; set; }
    public int EnhanceArmorPenetration { get; set; }
}
