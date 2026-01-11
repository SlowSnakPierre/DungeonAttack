using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DungeonAttack.Models.Camp;

/// <summary>
/// Statistiques d'un run en cours (ennemis tués)
/// </summary>
public class StatisticsRun
{
    private const string PATH = "Saves/statistics_run.json";

    public Dictionary<string, Dictionary<string, int>> Data { get; set; } = [];

    private readonly string _dungeonCode;
    private List<KeyValuePair<string, int>>? _dataEnemyes;

    public StatisticsRun(string dungeonCode, bool newObj = false)
    {
        _dungeonCode = dungeonCode;
        Create(newObj);
        Load();
    }

    /// <summary>
    /// Ajoute un ennemi tué aux stats
    /// </summary>
    public void AddEnemyToData(string enemyCode)
    {
        if (!Data.ContainsKey(_dungeonCode))
            Data[_dungeonCode] = [];

        if (Data[_dungeonCode].TryGetValue(enemyCode, out int value))
            Data[_dungeonCode][enemyCode] = ++value;
        else
            Data[_dungeonCode][enemyCode] = 1;
    }

    /// <summary>
    /// Sauvegarde les stats
    /// </summary>
    public void Update()
    {
        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        Directory.CreateDirectory("Saves");
        File.WriteAllText(PATH, serializer.Serialize(Data));
    }

    /// <summary>
    /// Supprime le fichier de stats
    /// </summary>
    public static void Delete()
    {
        if (File.Exists(PATH))
            File.Delete(PATH);
    }

    /// <summary>
    /// Properties dynamiques pour le rendering
    /// </summary>
    public string Name => _dungeonCode.Replace('_', ' ').ToUpper();

    public string EnemyName__0 => GetEnemyName(0);
    public string EnemyName__1 => GetEnemyName(1);
    public string EnemyName__2 => GetEnemyName(2);
    public string EnemyName__3 => GetEnemyName(3);
    public string EnemyName__4 => GetEnemyName(4);
    public string EnemyName__5 => GetEnemyName(5);

    public int EnemyCount__0 => GetEnemyCount(0);
    public int EnemyCount__1 => GetEnemyCount(1);
    public int EnemyCount__2 => GetEnemyCount(2);
    public int EnemyCount__3 => GetEnemyCount(3);
    public int EnemyCount__4 => GetEnemyCount(4);
    public int EnemyCount__5 => GetEnemyCount(5);

    private string GetEnemyName(int index)
    {
        CreateSubdatas();
        if (_dataEnemyes == null || index >= _dataEnemyes.Count)
            return "";

        return _dataEnemyes[index].Key.Replace('_', ' ').ToUpper();
    }

    private int GetEnemyCount(int index)
    {
        CreateSubdatas();
        if (_dataEnemyes == null || index >= _dataEnemyes.Count)
            return 0;

        return _dataEnemyes[index].Value;
    }

    private void CreateSubdatas()
    {
        if (_dataEnemyes == null && Data.TryGetValue(_dungeonCode, out Dictionary<string, int>? value))
        {
            _dataEnemyes = [.. value];
        }
    }

    private void Load()
    {
        if (!File.Exists(PATH))
            return;

        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            Data = deserializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(
                File.ReadAllText(PATH));
        }
        catch
        {
            Data = NewFileData();
        }
    }

    private void Create(bool newObj)
    {
        if (!File.Exists(PATH) || newObj)
        {
            Data = NewFileData();
            Update();
        }
    }

    private static Dictionary<string, Dictionary<string, int>> NewFileData()
    {
        return new Dictionary<string, Dictionary<string, int>>
        {
            ["bandits"] = new Dictionary<string, int>
            {
                ["rabble"] = 0,
                ["rabid_dog"] = 0,
                ["poacher"] = 0,
                ["thug"] = 0,
                ["deserter"] = 0,
                ["bandit_leader"] = 0
            },
            ["undeads"] = new Dictionary<string, int>
            {
                ["zombie"] = 0,
                ["skeleton"] = 0,
                ["ghost"] = 0,
                ["fat_ghoul"] = 0,
                ["skeleton_soldier"] = 0,
                ["zombie_knight"] = 0
            },
            ["swamp"] = new Dictionary<string, int>
            {
                ["leech"] = 0,
                ["goblin"] = 0,
                ["sworm"] = 0,
                ["spider"] = 0,
                ["orc"] = 0,
                ["ancient_snail"] = 0
            },
            ["monolith"] = new Dictionary<string, int>
            {
                ["stage_1_mimic"] = 0,
                ["stage_2_thing"] = 0,
                ["stage_3_dog"] = 0
            }
        };
    }
}
