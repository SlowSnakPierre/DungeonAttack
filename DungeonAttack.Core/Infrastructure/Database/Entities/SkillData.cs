namespace DungeonAttack.Infrastructure.Database.Entities;

/// <summary>
/// Entité pour les données de compétences (active, passive, camp)
/// </summary>
public class SkillData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SkillType { get; set; } = string.Empty; // active, passive, camp
    public string Description { get; set; } = string.Empty;

    public int MpCost { get; set; }
    public double DamageMod { get; set; }
    public double AccuracyMod { get; set; }
    public double LvlMod { get; set; }

    public int HpCost { get; set; }
    public double RestoreEffect { get; set; }
    public int BasicMod { get; set; }
}
