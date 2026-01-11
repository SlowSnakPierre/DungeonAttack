namespace DungeonAttack.Renderers;

/// <summary>
/// Structure d'un template de menu YAML
/// </summary>
public class MenuTemplate
{
    public List<string> View { get; set; } = [];
    public Dictionary<int, Dictionary<string, FieldOptions>>? InsertOptions { get; set; }
    public List<PartialConfig>? Partials { get; set; }
    public List<ArtConfig>? Arts { get; set; }
    public List<MenuConfig>? Menu { get; set; }
}

public class MenuConfig
{
    public bool ShowArrow { get; set; }
    public string Orientation { get; set; } = "vertical";
    public int ItemsCount { get; set; } = 0;
    public Dictionary<string, MenuButton> Buttons { get; set; } = [];
}

public class MenuButton
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class FieldOptions
{
    public List<string> Methods { get; set; } = [];
    public string Modifier { get; set; } = "m"; // m=middle, s=start, e=end
}

public class PartialConfig
{
    public string PartialName { get; set; } = string.Empty;
    public List<int> Y { get; set; } = [];
    public List<int> X { get; set; } = [];
}

public class ArtConfig
{
    public List<int> Y { get; set; } = [];
    public List<int> X { get; set; } = [];
}
