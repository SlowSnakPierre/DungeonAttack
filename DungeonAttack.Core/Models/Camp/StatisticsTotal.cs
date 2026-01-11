using DungeonAttack.Infrastructure;

namespace DungeonAttack.Models.Camp;

public class StatisticsTotal
{
    private const string PATH = "Saves/statistics_total.json";

    private static readonly string[] BOSSES = ["bandit_leader", "zombie_knight", "ancient_snail"];

    private static readonly Dictionary<string, (int Kill, string Get)> DESCRIPTIONS = new()
    {
        // Bandits
        { "rabble", (30, "Permanent weapon \"Stick\"") },
        { "rabid_dog", (30, "+2 HP") },
        { "poacher", (30, "+1 accuracy") },
        { "thug", (30, "+5 HP") },
        { "deserter", (30, "+1 stat point") },
        { "bandit_leader", (5, "+1 skill point") },
        // Undead
        { "zombie", (30, "Permanent \"Worn gloves\"") },
        { "skeleton", (30, "+3 MP") },
        { "ghost", (30, "+1 accuracy") },
        { "fat_ghoul", (30, "+7 HP") },
        { "skeleton_soldier", (30, "+3 block chance") },
        { "zombie_knight", (5, "+1 MP-regen") },
        // Swamp
        { "leech", (30, "+3 MP") },
        { "goblin", (30, "Permanent \"Holey wicker buckler\"") },
        { "sworm", (30, "+3 HP") },
        { "spider", (30, "+1 accuracy") },
        { "orc", (30, "+1 max damage") },
        { "ancient_snail", (5, "+1 armor") },
        // MONOLITH
        { "stage_1_mimic", (0, "---") },
        { "stage_2_thing", (0, "---") },
        { "stage_3_dog", (0, "---") }
    };

    public Dictionary<string, Dictionary<string, int>> Data { get; set; } = [];

    public static StatisticsTotal Load()
    {
        if (!File.Exists(PATH))
        {
            StatisticsTotal stats = new()
            {
                Data = new Dictionary<string, Dictionary<string, int>>
                {
                    { "bandits", new Dictionary<string, int>
                        {
                            { "rabble", 0 }, { "rabid_dog", 0 }, { "poacher", 0 },
                            { "thug", 0 }, { "deserter", 0 }, { "bandit_leader", 0 }
                        }
                    },
                    { "undeads", new Dictionary<string, int>
                        {
                            { "zombie", 0 }, { "skeleton", 0 }, { "ghost", 0 },
                            { "fat_ghoul", 0 }, { "skeleton_soldier", 0 }, { "zombie_knight", 0 }
                        }
                    },
                    { "swamp", new Dictionary<string, int>
                        {
                            { "leech", 0 }, { "goblin", 0 }, { "sworm", 0 },
                            { "spider", 0 }, { "orc", 0 }, { "ancient_snail", 0 }
                        }
                    },
                    { "monolith", new Dictionary<string, int>
                        {
                            { "stage_1_mimic", 0 }, { "stage_2_thing", 0 }, { "stage_3_dog", 0 }
                        }
                    }
                }
            };
            stats.Save();
            return stats;
        }

        StatisticsTotal loadedStats = JsonLoader.LoadOrDefault<StatisticsTotal>(PATH);

        if (loadedStats.Data.Count == 0)
        {
            loadedStats.Data = new Dictionary<string, Dictionary<string, int>>
            {
                { "bandits", new Dictionary<string, int>
                    {
                        { "rabble", 0 }, { "rabid_dog", 0 }, { "poacher", 0 },
                        { "thug", 0 }, { "deserter", 0 }, { "bandit_leader", 0 }
                    }
                },
                { "undeads", new Dictionary<string, int>
                    {
                        { "zombie", 0 }, { "skeleton", 0 }, { "ghost", 0 },
                        { "fat_ghoul", 0 }, { "skeleton_soldier", 0 }, { "zombie_knight", 0 }
                    }
                },
                { "swamp", new Dictionary<string, int>
                    {
                        { "leech", 0 }, { "goblin", 0 }, { "sworm", 0 },
                        { "spider", 0 }, { "orc", 0 }, { "ancient_snail", 0 }
                    }
                },
                { "monolith", new Dictionary<string, int>
                    {
                        { "stage_1_mimic", 0 }, { "stage_2_thing", 0 }, { "stage_3_dog", 0 }
                    }
                }
            };
        }

        return loadedStats;
    }

    public void Save()
    {
        JsonLoader.Save(PATH, this);
    }

    public void AddFromRun(Dictionary<string, Dictionary<string, int>> statisticsRun)
    {
        foreach (KeyValuePair<string, Dictionary<string, int>> section in statisticsRun)
        {
            if (!Data.ContainsKey(section.Key))
            {
                Data[section.Key] = [];
            }

            foreach (KeyValuePair<string, int> enemy in section.Value)
            {
                if (Data[section.Key].ContainsKey(enemy.Key))
                {
                    Data[section.Key][enemy.Key] += enemy.Value;
                }
                else
                {
                    Data[section.Key][enemy.Key] = enemy.Value;
                }
            }
        }
        Save();
    }

    public string GetEnemyName(string dungeonCode, int index)
    {
        if (!Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value)) return string.Empty;

        KeyValuePair<string, int>[] enemies = [.. value];
        if (index >= enemies.Length) return string.Empty;

        return enemies[index].Key.Replace('_', ' ');
    }

    public int GetEnemyCount(string dungeonCode, int index)
    {
        if (!Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value)) return 0;

        KeyValuePair<string, int>[] enemies = [.. value];
        if (index >= enemies.Length) return 0;

        return enemies[index].Value;
    }

    public string GetEnemyDone(string dungeonCode, int index)
    {
        if (!Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value)) return string.Empty;

        KeyValuePair<string, int>[] enemies = [.. value];
        if (index >= enemies.Length) return string.Empty;

        string enemyCode = enemies[index].Key;
        int count = enemies[index].Value;

        bool isDone = (BOSSES.Contains(enemyCode) && count >= 5) || count >= 30;
        return isDone ? "DONE" : string.Empty;
    }

    public int GetEnemyKillRequirement(string dungeonCode, int index)
    {
        if (!Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value)) return 0;

        KeyValuePair<string, int>[] enemies = [.. value];
        if (index >= enemies.Length) return 0;

        string enemyCode = enemies[index].Key;
        return DESCRIPTIONS.TryGetValue(enemyCode, out (int Kill, string Get) value1) ? value1.Kill : 0;
    }

    public string GetEnemyReward(string dungeonCode, int index)
    {
        if (!Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value)) return string.Empty;

        KeyValuePair<string, int>[] enemies = [.. value];
        if (index >= enemies.Length) return string.Empty;

        string enemyCode = enemies[index].Key;
        return DESCRIPTIONS.TryGetValue(enemyCode, out (int Kill, string Get) value1) ? value1.Get : string.Empty;
    }

    public Dictionary<string, int>? Subdatas { get; private set; }

    public void CreateSubdatas(string dungeonCode)
    {
        if (Data.TryGetValue(dungeonCode, out Dictionary<string, int>? value))
        {
            Subdatas = value;
        }
        else
        {
            Subdatas = [];
        }
    }
}
