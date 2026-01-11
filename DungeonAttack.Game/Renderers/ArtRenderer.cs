using DungeonAttack.Infrastructure;
using System.Reflection;

namespace DungeonAttack.Renderers;

/// <summary>
/// Renderer pour les arts ASCII
/// </summary>
public class ArtRenderer
{
    private static readonly List<string> ALT_ART =
    [
        "==================",
        "   NO IMAGE YET   ",
        "=================="
    ];

    public static List<string> Load(object? entity, string artName)
    {
        if (entity == null)
            return ALT_ART;

        string path = GetArtPath(entity);
        string fullPath = $"Views/arts/{path}.yml";

        try
        {
            if (!File.Exists(fullPath))
                return ALT_ART;

            Dictionary<string, List<string>> arts = YamlLoader.LoadDictionary<List<string>>(fullPath);
            return arts.TryGetValue(artName, out List<string>? value) ? value : ALT_ART;
        }
        catch
        {
            return ALT_ART;
        }
    }

    private static string GetArtPath(object entity)
    {
        Type type = entity.GetType();
        PropertyInfo? entityTypeProp = type.GetProperty("EntityType");

        if (entityTypeProp != null)
        {
            string? entityType = entityTypeProp.GetValue(entity)?.ToString();

            if (entityType == "enemyes")
            {
                PropertyInfo? dungeonProp = type.GetProperty("DungeonName");
                PropertyInfo? codeProp = type.GetProperty("CodeName");
                string dungeon = dungeonProp?.GetValue(entity)?.ToString() ?? "";
                string code = codeProp?.GetValue(entity)?.ToString() ?? "";
                return $"enemyes/{dungeon}/_{code}";
            }
            else if (entityType == "ammunition")
            {
                PropertyInfo? ammTypeProp = type.GetProperty("AmmunitionType");
                PropertyInfo? codeProp = type.GetProperty("Code");
                string ammType = ammTypeProp?.GetValue(entity)?.ToString() ?? "";
                string code = codeProp?.GetValue(entity)?.ToString() ?? "";
                return $"ammunition/{ammType}/_{code}";
            }
            else if (entityType == "skills")
            {
                PropertyInfo? codeProp = type.GetProperty("Code");
                string code = codeProp?.GetValue(entity)?.ToString() ?? "";
                return $"skills/_{code}";
            }
            else if (entityType == "events")
            {
                PropertyInfo? pathProp = type.GetProperty("PathArt");
                return pathProp?.GetValue(entity)?.ToString() ?? "";
            }
        }

        string entityStr = entity.ToString() ?? "";
        if (entityStr.Contains('/'))
            return entityStr;

        return $"_{entityStr}";
    }
}
