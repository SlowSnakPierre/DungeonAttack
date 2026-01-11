namespace DungeonAttack.Models.Messages;

/// <summary>
/// DTO pour passer les données d'affichage aux renderers
/// </summary>
public class MainMessage
{
    public string Main { get; set; } = string.Empty;
    public string Additional1 { get; set; } = string.Empty;
    public string Additional2 { get; set; } = string.Empty;
    public string Additional3 { get; set; } = string.Empty;
    public List<string> Log { get; set; } = [];

    /// <summary>
    /// Accès aux éléments du log par index (log_0, log_1, etc.)
    /// </summary>
    public string GetLog(int index)
    {
        return index >= 0 && index < Log.Count ? Log[index] : string.Empty;
    }

    public void ClearLog()
    {
        Log.Clear();
    }
}
