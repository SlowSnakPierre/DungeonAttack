using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Services.Saves;

/// <summary>
/// Service de sauvegarde simplifié pour les héros en cours de run
/// </summary>
public static class SaveService
{
    private const string PATH = "Saves/hero_in_run.json";

    /// <summary>
    /// Sauvegarde le héros actuel en JSON
    /// </summary>
    public static bool Save(Hero hero)
    {
        try
        {
            // Pour simplifier, on sauvegarde directement l'objet Hero
            // Dans une version complète, on créerait un DTO spécifique
            HeroSaveData saveData = new()
            {
                Name = hero.Name,
                Background = hero.Background,
                DungeonName = hero.DungeonName,
                DungeonPartNumber = hero.DungeonPartNumber,
                Leveling = hero.Leveling,

                // Stats
                Hp = hero.Hp,
                HpMax = hero.HpMax,
                Mp = hero.Mp,
                MpMax = hero.MpMax,
                MinDmgBase = hero.MinDmgBase,
                MaxDmgBase = hero.MaxDmgBase,
                AccuracyBase = hero.AccuracyBase,
                ArmorBase = hero.ArmorBase,
                BlockChanceBase = hero.BlockChanceBase,
                ArmorPenetrationBase = hero.ArmorPenetrationBase,
                RegenHpBase = hero.RegenHpBase,
                RegenMpBase = hero.RegenMpBase,

                // Progression
                Exp = hero.Exp,
                Level = hero.Level,
                StatPoints = hero.StatPoints,
                SkillPoints = hero.SkillPoints,

                // Resources
                Coins = hero.Coins,
                MonolithPoints = hero.MonolithPoints,
                Ingredients = hero.Ingredients,

                // Skills (codes seulement)
                ActiveSkillCode = hero.ActiveSkill?.Code ?? string.Empty,
                ActiveSkillLevel = hero.ActiveSkill?.Level ?? 0,
                PassiveSkillCode = hero.PassiveSkill?.Code ?? string.Empty,
                PassiveSkillLevel = hero.PassiveSkill?.Level ?? 0,
                CampSkillCode = hero.CampSkill?.Code ?? string.Empty,
                CampSkillLevel = hero.CampSkill?.Level ?? 0,

                // Equipment (codes seulement)
                WeaponCode = hero.Weapon.Code,
                BodyArmorCode = hero.BodyArmor.Code,
                HeadArmorCode = hero.HeadArmor.Code,
                ArmsArmorCode = hero.ArmsArmor.Code,
                ShieldCode = hero.Shield.Code,

                // Events data
                EventsData = hero.EventsData
            };

            JsonLoader.Save(PATH, saveData);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Vérifie si une sauvegarde existe
    /// </summary>
    public static bool SaveExists()
    {
        return File.Exists(PATH);
    }
}

/// <summary>
/// DTO pour la sauvegarde du héros
/// </summary>
public class HeroSaveData
{
    public string Name { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string DungeonName { get; set; } = string.Empty;
    public int DungeonPartNumber { get; set; }
    public int Leveling { get; set; }

    public int Hp { get; set; }
    public int HpMax { get; set; }
    public int Mp { get; set; }
    public int MpMax { get; set; }
    public int MinDmgBase { get; set; }
    public int MaxDmgBase { get; set; }
    public int AccuracyBase { get; set; }
    public int ArmorBase { get; set; }
    public int BlockChanceBase { get; set; }
    public int ArmorPenetrationBase { get; set; }
    public int RegenHpBase { get; set; }
    public int RegenMpBase { get; set; }

    public int Exp { get; set; }
    public int Level { get; set; }
    public int StatPoints { get; set; }
    public int SkillPoints { get; set; }

    public int Coins { get; set; }
    public int MonolithPoints { get; set; }
    public Dictionary<string, int> Ingredients { get; set; } = [];

    public string ActiveSkillCode { get; set; } = string.Empty;
    public int ActiveSkillLevel { get; set; }
    public string PassiveSkillCode { get; set; } = string.Empty;
    public int PassiveSkillLevel { get; set; }
    public string CampSkillCode { get; set; } = string.Empty;
    public int CampSkillLevel { get; set; }

    public string WeaponCode { get; set; } = string.Empty;
    public string BodyArmorCode { get; set; } = string.Empty;
    public string HeadArmorCode { get; set; } = string.Empty;
    public string ArmsArmorCode { get; set; } = string.Empty;
    public string ShieldCode { get; set; } = string.Empty;

    public Dictionary<string, object> EventsData { get; set; } = [];
}
