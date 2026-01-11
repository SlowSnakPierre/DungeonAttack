using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Factories;

/// <summary>
/// Factory pour créer des héros à partir de la base de données
/// </summary>
public static class HeroFactory
{
    /// <summary>
    /// Crée un héros de base à partir de son background
    /// NOTE: Cette factory crée un héros basique. Les bonus (monolith, shop, skills)
    /// doivent être appliqués après création via les services/controllers appropriés.
    /// </summary>
    /// <param name="name">Nom du héros</param>
    /// <param name="background">Code du background (drunk, passerby, watchman, etc.)</param>
    /// <param name="dungeonName">Nom du donjon (optionnel)</param>
    /// <returns>Instance de Hero avec stats de base</returns>
    public static Hero Create(string name, string background, string? dungeonName = null)
    {
        using GameDbContext context = new();
        HeroData? heroData = context.Heroes.Find(background) ?? throw new InvalidOperationException($"Hero background '{background}' not found in database");

        List<string> bodyArmorList = !string.IsNullOrEmpty(heroData.BodyArmor)
            ? [.. heroData.BodyArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> headArmorList = !string.IsNullOrEmpty(heroData.HeadArmor)
            ? [.. heroData.HeadArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> armsArmorList = !string.IsNullOrEmpty(heroData.ArmsArmor)
            ? [.. heroData.ArmsArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> shieldList = !string.IsNullOrEmpty(heroData.Shield)
            ? [.. heroData.Shield.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];

        Hero hero = new()
        {
            Name = name,
            Background = background,
            BackgroundName = heroData.Name,

            DungeonName = dungeonName ?? string.Empty,
            DungeonPartNumber = 1,
            Leveling = 0,
            GameStatus = "normal",

            Hp = heroData.Hp,
            HpMax = heroData.Hp,
            RegenHpBase = 0,

            Mp = heroData.Mp,
            MpMax = heroData.Mp,
            RegenMpBase = 0,

            MinDmgBase = heroData.MinDmg,
            MaxDmgBase = heroData.MaxDmg,
            ArmorPenetrationBase = heroData.ArmorPenetration,

            AccuracyBase = heroData.Accuracy,
            ArmorBase = heroData.Armor,
            BlockChanceBase = 0,

            Exp = 0,
            Level = 0,
            StatPoints = 5,
            SkillPoints = heroData.SkillPoints,

            MonolithPoints = 0,
            Coins = 0,
            Ingredients = [],
            EventsData = [],
            Weapon = new Weapon(heroData.Weapon),
            BodyArmor = new BodyArmor(bodyArmorList.Count > 0
                ? bodyArmorList[Random.Shared.Next(bodyArmorList.Count)]
                : "without"),
            HeadArmor = new HeadArmor(headArmorList.Count > 0
                ? headArmorList[Random.Shared.Next(headArmorList.Count)]
                : "without"),
            ArmsArmor = new ArmsArmor(armsArmorList.Count > 0
                ? armsArmorList[Random.Shared.Next(armsArmorList.Count)]
                : "without"),
            Shield = new Shield(shieldList.Count > 0
                ? shieldList[Random.Shared.Next(shieldList.Count)]
                : "without")
        };

        return hero;
    }
}
