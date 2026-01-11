namespace DungeonAttack.Models.Messages;

/// <summary>
/// DTO pour les messages de combat (similaire à MainMessage)
/// </summary>
public class AttacksRoundMessage
{
    public string Main { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public string Additional1 { get; set; } = string.Empty;
    public string Additional2 { get; set; } = string.Empty;
    public string Additional3 { get; set; } = string.Empty;
    public List<string> Log { get; set; } = [];

    public string GetLog(int index)
    {
        return index >= 0 && index < Log.Count ? Log[index] : string.Empty;
    }

    public void ClearLog()
    {
        Log.Clear();
    }
}
