namespace DungeonAttack.Services.Saves;

/// <summary>
/// Service de suppression de sauvegarde
/// </summary>
public static class DeleteService
{
    private const string PATH = "Saves/hero_in_run.json";

    /// <summary>
    /// Supprime la sauvegarde du héros
    /// </summary>
    public static void Delete()
    {
        if (File.Exists(PATH))
        {
            File.Delete(PATH);
        }
    }
}
