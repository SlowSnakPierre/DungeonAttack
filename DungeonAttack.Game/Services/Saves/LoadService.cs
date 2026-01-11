using DungeonAttack.Factories;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Services.Saves;

/// <summary>
/// Service de chargement de sauvegarde
/// </summary>
public static class LoadService
{
    private const string PATH = "Saves/hero_in_run.json";

    /// <summary>
    /// Charge un héros sauvegardé
    /// </summary>
    public static Hero Load()
    {
        HeroSaveData saveData = JsonLoader.Load<HeroSaveData>(PATH);

        Hero hero = new()
        {
            Name = saveData.Name,
            Background = saveData.Background,
            DungeonName = saveData.DungeonName,
            DungeonPartNumber = saveData.DungeonPartNumber,
            Leveling = saveData.Leveling,

            // Stats
            Hp = saveData.Hp,
            HpMax = saveData.HpMax,
            Mp = saveData.Mp,
            MpMax = saveData.MpMax,
            MinDmgBase = saveData.MinDmgBase,
            MaxDmgBase = saveData.MaxDmgBase,
            AccuracyBase = saveData.AccuracyBase,
            ArmorBase = saveData.ArmorBase,
            BlockChanceBase = saveData.BlockChanceBase,
            ArmorPenetrationBase = saveData.ArmorPenetrationBase,
            RegenHpBase = saveData.RegenHpBase,
            RegenMpBase = saveData.RegenMpBase,

            // Progression
            Exp = saveData.Exp,
            Level = saveData.Level,
            StatPoints = saveData.StatPoints,
            SkillPoints = saveData.SkillPoints,

            // Resources
            Coins = saveData.Coins,
            MonolithPoints = saveData.MonolithPoints,
            Ingredients = saveData.Ingredients,
            EventsData = saveData.EventsData,
            // Recréer l'équipement
            Weapon = new Weapon(saveData.WeaponCode),
            BodyArmor = new BodyArmor(saveData.BodyArmorCode),
            HeadArmor = new HeadArmor(saveData.HeadArmorCode),
            ArmsArmor = new ArmsArmor(saveData.ArmsArmorCode),
            Shield = new Shield(saveData.ShieldCode)
        };

        if (!string.IsNullOrEmpty(saveData.ActiveSkillCode))
        {
            hero.ActiveSkill = SkillFactory.CreateActive(saveData.ActiveSkillCode, hero);
            hero.ActiveSkill.Level = saveData.ActiveSkillLevel;
        }

        if (!string.IsNullOrEmpty(saveData.PassiveSkillCode))
        {
            hero.PassiveSkill = SkillFactory.CreatePassive(saveData.PassiveSkillCode, hero);
            hero.PassiveSkill.Level = saveData.PassiveSkillLevel;
        }

        if (!string.IsNullOrEmpty(saveData.CampSkillCode))
        {
            hero.CampSkill = SkillFactory.CreateCamp(saveData.CampSkillCode, hero);
            hero.CampSkill.Level = saveData.CampSkillLevel;
        }

        return hero;
    }
}
