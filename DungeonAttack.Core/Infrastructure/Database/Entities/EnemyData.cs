namespace DungeonAttack.Infrastructure.Database.Entities;

/// <summary>
/// Entité pour les données d'ennemis
/// </summary>
public class EnemyData
{
    public string DungeonName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CodeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int N { get; set; }

    public int Hp { get; set; }
    public int Mp { get; set; }
    public int MinDmg { get; set; }
    public int MaxDmg { get; set; }
    public int Accuracy { get; set; }
    public int Armor { get; set; }
    public int BlockChance { get; set; }
    public int ArmorPenetration { get; set; }
    public int RegenHp { get; set; }
    public int RegenMp { get; set; }
    public int Exp { get; set; }
    public int Coins { get; set; }

    // Equipment and loot (stored as comma-separated strings)
    public string Ingredients { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string BodyArmor { get; set; } = string.Empty;
    public string HeadArmor { get; set; } = string.Empty;
    public string ArmsArmor { get; set; } = string.Empty;
    public string Shield { get; set; } = string.Empty;
}
