using System.Text.Json;
using System.Text.Json.Serialization;

namespace DungeonAttack.Infrastructure;

/// <summary>
/// Utilitaire pour charger/sauvegarder des fichiers JSON
/// Utilisé pour les sauvegardes de jeu (héros, options, warehouse, etc.)
/// </summary>
public static class JsonLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
        }
    };

    /// <summary>
    /// Charge un fichier JSON complet et le désérialise en type T
    /// </summary>
    public static T Load<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"JSON file not found: {filePath}");
        }

        string content = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(content, Options)
            ?? throw new InvalidOperationException($"Failed to deserialize JSON from {filePath}");
    }

    /// <summary>
    /// Sauvegarde un objet dans un fichier JSON
    /// </summary>
    public static void Save<T>(string filePath, T obj) where T : class
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(obj, Options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Tente de charger un fichier JSON, retourne une nouvelle instance si le fichier n'existe pas
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
            return JsonSerializer.Deserialize<T>(content, Options) ?? new T();
        }
        catch
        {
            return new T();
        }
    }
}
