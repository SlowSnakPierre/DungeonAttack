using DungeonAttack.Infrastructure.Database.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DungeonAttack.Infrastructure.Database;

/// <summary>
/// Migre les données YAML vers SQLite au premier lancement
/// </summary>
public static class DataMigrator
{
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Initialise la base de données et migre les données YAML si nécessaire
    /// </summary>
    public static void EnsureDatabaseInitialized()
    {
        using GameDbContext context = new();

        context.Database.EnsureCreated();

        if (context.Heroes.Any())
        {
            return;
        }

        Console.WriteLine("Première initialisation: migration des données YAML vers SQLite...");

        try
        {
            MigrateHeroes(context);
            MigrateWeapons(context);
            MigrateArmors(context);
            MigrateShields(context);
            MigrateEnemies(context);
            MigrateSkills();

            context.SaveChanges();
            Console.WriteLine("✓ Migration terminée avec succès");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Erreur lors de la migration: {ex.Message}");
            throw;
        }
    }

    private static void MigrateHeroes(GameDbContext context)
    {
        string yamlPath = "Data/Characters/heroes.yml";
        if (!File.Exists(yamlPath)) return;

        string yaml = File.ReadAllText(yamlPath);
        Dictionary<string, Dictionary<string, object>> heroesDict = YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

        foreach ((string? code, Dictionary<string, object>? data) in heroesDict)
        {
            if (code == "cheater") continue;

            List<string> bodyArmorList = data.TryGetValue("body_armor", out object? value1) && value1 is List<object> ba
                ? [.. ba.Select(x => x?.ToString() ?? "")]
                : [];
            List<string> headArmorList = data.TryGetValue("head_armor", out object? value2) && value2 is List<object> ha
                ? [.. ha.Select(x => x?.ToString() ?? "")]
                : [];
            List<string> armsArmorList = data.TryGetValue("arms_armor", out object? value3) && value3 is List<object> aa
                ? [.. aa.Select(x => x?.ToString() ?? "")]
                : [];
            List<string> shieldList = data.TryGetValue("shield", out object? value4) && value4 is List<object> sh
                ? [.. sh.Select(x => x?.ToString() ?? "")]
                : [];

            context.Heroes.Add(new HeroData
            {
                Code = code,
                Name = data["name"]?.ToString() ?? "",
                N = Convert.ToInt32(data["n"]),
                Hp = Convert.ToInt32(data["hp"]),
                Mp = Convert.ToInt32(data["mp"]),
                MinDmg = Convert.ToInt32(data["min_dmg"]),
                MaxDmg = Convert.ToInt32(data["max_dmg"]),
                Accuracy = Convert.ToInt32(data["accuracy"]),
                Armor = Convert.ToInt32(data["armor"]),
                ArmorPenetration = data.TryGetValue("armor_penetration", out object? value5) ? Convert.ToInt32(value5) : 0,
                SkillPoints = Convert.ToInt32(data["skill_points"]),

                // Equipment
                Weapon = data.TryGetValue("weapon", out object? value) ? value?.ToString() ?? "" : "",
                BodyArmor = string.Join(",", bodyArmorList),
                HeadArmor = string.Join(",", headArmorList),
                ArmsArmor = string.Join(",", armsArmorList),
                Shield = string.Join(",", shieldList)
            });
        }

        Console.WriteLine($"  → {heroesDict.Count - 1} héros migrés");
    }

    private static void MigrateWeapons(GameDbContext context)
    {
        string yamlPath = "Data/Ammunition/weapon.yml";
        if (!File.Exists(yamlPath)) return;

        string yaml = File.ReadAllText(yamlPath);
        Dictionary<string, Dictionary<string, object>> weaponsDict = YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

        foreach ((string? code, Dictionary<string, object>? data) in weaponsDict)
        {
            context.Weapons.Add(new WeaponData
            {
                Code = code,
                Name = data.TryGetValue("name", out object? value) ? value?.ToString() ?? "" : "",
                Price = data.TryGetValue("price", out object? value1) ? Convert.ToInt32(value1) : 0,
                MinDmg = data.TryGetValue("min_dmg", out object? value2) ? Convert.ToInt32(value2) : 0,
                MaxDmg = data.TryGetValue("max_dmg", out object? value3) ? Convert.ToInt32(value3) : 0,
                Accuracy = data.TryGetValue("accuracy", out object? value4) ? Convert.ToInt32(value4) : 0,
                Armor = data.TryGetValue("armor", out object? value15) ? Convert.ToInt32(value15) : 0,
                Mp = data.TryGetValue("mp", out object? value5) ? Convert.ToInt32(value5) : 0,
                BlockChance = data.TryGetValue("block_chance", out object? value6) ? Convert.ToInt32(value6) : 0,
                ArmorPenetration = data.TryGetValue("armor_penetration", out object? value14) ? Convert.ToInt32(value14) : 0,
                EnhanceMinDmg = data.TryGetValue("enhance_min_dmg", out object? value7) ? Convert.ToInt32(value7) : 0,
                EnhanceMaxDmg = data.TryGetValue("enhance_max_dmg", out object? value13) ? Convert.ToInt32(value13) : 0,
                EnhanceAccuracy = data.TryGetValue("enhance_accuracy", out object? value8) ? Convert.ToInt32(value8) : 0,
                EnhanceArmor = data.TryGetValue("enhance_armor", out object? value9) ? Convert.ToInt32(value9) : 0,
                EnhanceMp = data.TryGetValue("enhance_mp", out object? value10) ? Convert.ToInt32(value10) : 0,
                EnhanceBlockChance = data.TryGetValue("enhance_block_chance", out object? value11) ? Convert.ToInt32(value11) : 0,
                EnhanceArmorPenetration = data.TryGetValue("enhance_armor_penetration", out object? value12) ? Convert.ToInt32(value12) : 0
            });
        }

        Console.WriteLine($"  → {weaponsDict.Count} armes migrées");
    }

    private static void MigrateArmors(GameDbContext context)
    {
        string[] armorTypes = ["body_armor", "head_armor", "arms_armor"];

        foreach (string armorType in armorTypes)
        {
            string yamlPath = $"Data/Ammunition/{armorType}.yml";
            if (!File.Exists(yamlPath)) continue;

            string yaml = File.ReadAllText(yamlPath);
            Dictionary<string, Dictionary<string, object>> armorsDict = YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

            foreach ((string? code, Dictionary<string, object>? data) in armorsDict)
            {
                context.Armors.Add(new ArmorData
                {
                    Code = code,
                    Name = data.TryGetValue("name", out object? value) ? value?.ToString() ?? "" : "",
                    AmmunitionType = armorType,
                    Price = data.TryGetValue("price", out object? value1) ? Convert.ToInt32(value1) : 0,
                    Accuracy = data.TryGetValue("accuracy", out object? value2) ? Convert.ToInt32(value2) : 0,
                    Armor = data.TryGetValue("armor", out object? value3) ? Convert.ToInt32(value3) : 0,
                    Mp = data.TryGetValue("mp", out object? value4) ? Convert.ToInt32(value4) : 0,
                    BlockChance = data.TryGetValue("block_chance", out object? value5) ? Convert.ToInt32(value5) : 0,
                    ArmorPenetration = data.TryGetValue("armor_penetration", out object? value6) ? Convert.ToInt32(value6) : 0,
                    EnhanceAccuracy = data.TryGetValue("enhance_accuracy", out object? value7) ? Convert.ToInt32(value7) : 0,
                    EnhanceArmor = data.TryGetValue("enhance_armor", out object? value8) ? Convert.ToInt32(value8) : 0,
                    EnhanceMp = data.TryGetValue("enhance_mp", out object? value9) ? Convert.ToInt32(value9) : 0,
                    EnhanceBlockChance = data.TryGetValue("enhance_block_chance", out object? value10) ? Convert.ToInt32(value10) : 0,
                    EnhanceArmorPenetration = data.TryGetValue("enhance_armor_penetration", out object? value11) ? Convert.ToInt32(value11) : 0
                });
            }

            Console.WriteLine($"  → {armorsDict.Count} {armorType} migrés");
        }
    }

    private static void MigrateShields(GameDbContext context)
    {
        string yamlPath = "Data/Ammunition/shield.yml";
        if (!File.Exists(yamlPath)) return;

        string yaml = File.ReadAllText(yamlPath);
        Dictionary<string, Dictionary<string, object>> shieldsDict = YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

        foreach ((string? code, Dictionary<string, object>? data) in shieldsDict)
        {
            context.Shields.Add(new ShieldData
            {
                Code = code,
                Name = data.TryGetValue("name", out object? value) ? value?.ToString() ?? "" : "",
                Price = data.TryGetValue("price", out object? value1) ? Convert.ToInt32(value1) : 0,
                MinDmg = data.TryGetValue("min_dmg", out object? value2) ? Convert.ToInt32(value2) : 0,
                MaxDmg = data.TryGetValue("max_dmg", out object? value3) ? Convert.ToInt32(value3) : 0,
                Accuracy = data.TryGetValue("accuracy", out object? value4) ? Convert.ToInt32(value4) : 0,
                Armor = data.TryGetValue("armor", out object? value5) ? Convert.ToInt32(value5) : 0,
                Mp = data.TryGetValue("mp", out object? value6) ? Convert.ToInt32(value6) : 0,
                BlockChance = data.TryGetValue("block_chance", out object? value7) ? Convert.ToInt32(value7) : 0,
                ArmorPenetration = data.TryGetValue("armor_penetration", out object? value8) ? Convert.ToInt32(value8) : 0,
                EnhanceMinDmg = data.TryGetValue("enhance_min_dmg", out object? value9) ? Convert.ToInt32(value9) : 0,
                EnhanceMaxDmg = data.TryGetValue("enhance_max_dmg", out object? value10) ? Convert.ToInt32(value10) : 0,
                EnhanceAccuracy = data.TryGetValue("enhance_accuracy", out object? value11) ? Convert.ToInt32(value11) : 0,
                EnhanceArmor = data.TryGetValue("enhance_armor", out object? value12) ? Convert.ToInt32(value12) : 0,
                EnhanceMp = data.TryGetValue("enhance_mp", out object? value13) ? Convert.ToInt32(value13) : 0,
                EnhanceBlockChance = data.TryGetValue("enhance_block_chance", out object? value14) ? Convert.ToInt32(value14) : 0,
                EnhanceArmorPenetration = data.TryGetValue("enhance_armor_penetration", out object? value15) ? Convert.ToInt32(value15) : 0
            });
        }

        Console.WriteLine($"  → {shieldsDict.Count} boucliers migrés");
    }

    private static void MigrateEnemies(GameDbContext context)
    {
        string[] dungeons = ["bandits", "undeads", "swamp", "monolith"];

        foreach (string dungeon in dungeons)
        {
            string yamlPath = $"Data/Characters/Enemyes/{dungeon}.yml";
            if (!File.Exists(yamlPath)) continue;

            string yaml = File.ReadAllText(yamlPath);
            Dictionary<string, Dictionary<string, object>> enemiesDict = YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

            foreach ((string? code, Dictionary<string, object>? data) in enemiesDict)
            {
                // Parse equipment and ingredient lists
                List<string> weaponList = data.TryGetValue("weapon", out object? value) && value is List<object> w
                    ? [.. w.Select(x => x?.ToString() ?? "")]
                    : [];
                List<string> bodyArmorList = data.TryGetValue("body_armor", out object? value1) && value1 is List<object> ba
                    ? [.. ba.Select(x => x?.ToString() ?? "")]
                    : [];
                List<string> headArmorList = data.TryGetValue("head_armor", out object? value2) && value2 is List<object> ha
                    ? [.. ha.Select(x => x?.ToString() ?? "")]
                    : [];
                List<string> armsArmorList = data.TryGetValue("arms_armor", out object? value3) && value3 is List<object> aa
                    ? [.. aa.Select(x => x?.ToString() ?? "")]
                    : [];
                List<string> shieldList = data.TryGetValue("shield", out object? value4) && value4 is List<object> sh
                    ? [.. sh.Select(x => x?.ToString() ?? "")]
                    : [];
                List<string> ingredientsList = data.TryGetValue("ingredients", out object? value5) && value5 is List<object> ing
                    ? [.. ing.Select(x => x?.ToString() ?? "")]
                    : [];

                context.Enemies.Add(new EnemyData
                {
                    DungeonName = dungeon,
                    Code = code,
                    CodeName = data.TryGetValue("code_name", out object? value6) ? value6?.ToString() ?? "" : "",
                    Name = data["name"]?.ToString() ?? "",
                    N = data.TryGetValue("n", out object? value7) ? Convert.ToInt32(value7) : 0,
                    Hp = Convert.ToInt32(data["hp"]),
                    Mp = data.TryGetValue("mp", out object? value8) ? Convert.ToInt32(value8) : 0,
                    MinDmg = Convert.ToInt32(data["min_dmg"]),
                    MaxDmg = Convert.ToInt32(data["max_dmg"]),
                    Accuracy = Convert.ToInt32(data["accuracy"]),
                    Armor = Convert.ToInt32(data["armor"]),
                    BlockChance = data.TryGetValue("block_chance", out object? value9) ? Convert.ToInt32(value9) : 0,
                    ArmorPenetration = data.TryGetValue("armor_penetration", out object? value10) ? Convert.ToInt32(value10) : 0,
                    RegenHp = data.TryGetValue("regen_hp_base", out object? value11) ? Convert.ToInt32(value11) : 0,
                    RegenMp = data.TryGetValue("regen_mp_base", out object? value12) ? Convert.ToInt32(value12) : 0,
                    Exp = Convert.ToInt32(data["exp_gived"]),
                    Coins = Convert.ToInt32(data["coins_gived"]),

                    // Equipment and loot
                    Weapon = string.Join(",", weaponList),
                    BodyArmor = string.Join(",", bodyArmorList),
                    HeadArmor = string.Join(",", headArmorList),
                    ArmsArmor = string.Join(",", armsArmorList),
                    Shield = string.Join(",", shieldList),
                    Ingredients = string.Join(",", ingredientsList)
                });
            }

            Console.WriteLine($"  → {enemiesDict.Count} ennemis {dungeon} migrés");
        }
    }

    private static void MigrateSkills()
    {
        // Note: Les skills sont complexes et nécessitent une logique spécifique
        // Pour simplifier, on ne les migre pas encore
        // Ils peuvent rester chargés dynamiquement via les factories
        Console.WriteLine("  → Skills: utilisation des factories dynamiques (pas de migration)");
    }
}
