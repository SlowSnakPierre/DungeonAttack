using DungeonAttack.Infrastructure;

namespace DungeonAttack.Models.Camp;

/// <summary>
/// Système de méta-progression permanent
/// </summary>
public class Monolith
{
    private const string PATH = "Saves/monolith.json";

    private static readonly Dictionary<string, int> PRICES = new()
    {
        { "hp", 1 },
        { "mp", 1 },
        { "accuracy", 5 },
        { "damage", 10 },
        { "stat_points", 7 },
        { "skill_points", 15 },
        { "armor", 40 },
        { "regen_hp", 70 },
        { "regen_mp", 60 },
        { "armor_penetration", 30 },
        { "block_chance", 5 }
    };

    private static readonly Dictionary<string, double> PRICE_MULTIPLIER = new()
    {
        { "hp", 1.04 },
        { "mp", 1.04 },
        { "accuracy", 1.4 },
        { "damage", 1.4 },
        { "stat_points", 1.3 },
        { "skill_points", 1.3 },
        { "armor", 1.7 },
        { "regen_hp", 2.0 },
        { "regen_mp", 2.0 },
        { "armor_penetration", 1.4 },
        { "block_chance", 1.5 }
    };

    public static readonly string[] STATS_LIST =
    [
        "hp", "mp", "accuracy", "damage", "stat_points", "skill_points",
        "armor", "regen_hp", "regen_mp", "armor_penetration", "block_chance"
    ];

    public int Points { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int Accuracy { get; set; }
    public int Damage { get; set; }
    public int StatPoints { get; set; }
    public int SkillPoints { get; set; }
    public int Armor { get; set; }
    public int RegenHp { get; set; }
    public int RegenMp { get; set; }
    public int ArmorPenetration { get; set; }
    public int BlockChance { get; set; }

    public static Monolith Load()
    {
        if (!File.Exists(PATH))
        {
            Monolith monolith = new();
            monolith.Save();
            return monolith;
        }

        return JsonLoader.LoadOrDefault<Monolith>(PATH);
    }

    public void Save()
    {
        JsonLoader.Save(PATH, this);
    }

    public void AddPoints(int n)
    {
        Points += n;
        Save();
    }

    public void TakePointsTo(int index)
    {
        if (index < 0 || index >= STATS_LIST.Length) return;

        string characteristic = STATS_LIST[index];
        int price = RealPriceWithMultiplier(characteristic);

        if (Points >= price)
        {
            Points -= price;
            IncrementStat(characteristic);
            Save();
        }
    }

    public int RealPriceWithMultiplier(string characteristic)
    {
        if (!PRICES.TryGetValue(characteristic, out int value) || !PRICE_MULTIPLIER.TryGetValue(characteristic, out double value1))
            return 0;

        int currentValue = GetStatValue(characteristic);
        return (int)Math.Floor(value * Math.Pow(value1, currentValue));
    }

    private int GetStatValue(string characteristic)
    {
        return characteristic switch
        {
            "hp" => Hp,
            "mp" => Mp,
            "accuracy" => Accuracy,
            "damage" => Damage,
            "stat_points" => StatPoints,
            "skill_points" => SkillPoints,
            "armor" => Armor,
            "regen_hp" => RegenHp,
            "regen_mp" => RegenMp,
            "armor_penetration" => ArmorPenetration,
            "block_chance" => BlockChance,
            _ => 0
        };
    }

    private void IncrementStat(string characteristic)
    {
        switch (characteristic)
        {
            case "hp": Hp++; break;
            case "mp": Mp++; break;
            case "accuracy": Accuracy++; break;
            case "damage": Damage++; break;
            case "stat_points": StatPoints++; break;
            case "skill_points": SkillPoints++; break;
            case "armor": Armor++; break;
            case "regen_hp": RegenHp++; break;
            case "regen_mp": RegenMp++; break;
            case "armor_penetration": ArmorPenetration++; break;
            case "block_chance": BlockChance++; break;
        }
    }
}
