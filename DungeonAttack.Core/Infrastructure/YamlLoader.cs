using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DungeonAttack.Infrastructure;

public static class YamlLoader
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    /// <summary>
    /// Charge un fichier YAML complet et le désérialise en type T
    /// </summary>
    public static T Load<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"YAML file not found: {filePath}");
        }

        string content = File.ReadAllText(filePath);
        return Deserializer.Deserialize<T>(content);
    }

    /// <summary>
    /// Charge un fichier YAML en tant que dictionnaire et retourne l'élément avec la clé spécifiée
    /// </summary>
    public static T LoadWithKey<T>(string filePath, string key) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"YAML file not found: {filePath}");
        }

        string content = File.ReadAllText(filePath);
        Dictionary<string, T> dict = Deserializer.Deserialize<Dictionary<string, T>>(content);

        if (!dict.TryGetValue(key, out T? value))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in YAML file: {filePath}");
        }

        return value;
    }

    /// <summary>
    /// Charge un fichier YAML en tant que dictionnaire complet
    /// </summary>
    public static Dictionary<string, T> LoadDictionary<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"YAML file not found: {filePath}");
        }

        string content = File.ReadAllText(filePath);
        return Deserializer.Deserialize<Dictionary<string, T>>(content);
    }

    /// <summary>
    /// Sauvegarde un objet dans un fichier YAML
    /// </summary>
    public static void Save<T>(string filePath, T obj) where T : class
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string yaml = Serializer.Serialize(obj);
        File.WriteAllText(filePath, yaml);
    }

    /// <summary>
    /// Tente de charger un fichier YAML, retourne une nouvelle instance si le fichier n'existe pas
    /// </summary>
    public static T LoadOrDefault<T>(string filePath) where T : class, new()
    {
        if (!File.Exists(filePath))
        {
            return new T();
        }

        try
        {
            string content = File.ReadAllText(filePath);
            return Deserializer.Deserialize<T>(content) ?? new T();
        }
        catch
        {
            return new T();
        }
    }
}
