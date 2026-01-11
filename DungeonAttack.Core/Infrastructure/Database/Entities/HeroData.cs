namespace DungeonAttack.Infrastructure.Database.Entities;

/// <summary>
/// Entité pour les données de héros (backgrounds)
/// Virtual properties pour lazy loading avec proxies
/// </summary>
public class HeroData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int N { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int MinDmg { get; set; }
    public int MaxDmg { get; set; }
    public int Accuracy { get; set; }
    public int Armor { get; set; }
    public int ArmorPenetration { get; set; }
    public int SkillPoints { get; set; }

    // Equipment (stored as comma-separated strings)
    public string Weapon { get; set; } = string.Empty;
    public string BodyArmor { get; set; } = string.Empty;
    public string HeadArmor { get; set; } = string.Empty;
    public string ArmsArmor { get; set; } = string.Empty;
    public string Shield { get; set; } = string.Empty;
}
