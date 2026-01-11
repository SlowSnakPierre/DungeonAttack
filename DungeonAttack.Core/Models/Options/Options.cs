using DungeonAttack.Infrastructure;

namespace DungeonAttack.Models.Options;

public class Options
{
    private const string PATH = "Saves/options.json";

    public static readonly double[] SPEEDS_FOR_ANIMATIONS = [0.1, 0.4, 0.7, 1.0, 1.5];
    public static readonly string[] SCREEN_REPLACEMENT_TYPES = ["console_clear", "\x1bc", "\u001b[H\u001b[2J", "\u001b[2J"];

    public double EnemyActionsAnimationSpeed { get; set; } = 0.7;
    public string ScreenReplacementType { get; set; } = "console_clear";

    /// <summary>
    /// Charge ou crée les options
    /// Sauvegardées en JSON
    /// </summary>
    public static Options Load()
    {
        if (!File.Exists(PATH))
        {
            Options options = new();
            options.Save();
            return options;
        }

        return JsonLoader.LoadOrDefault<Options>(PATH);
    }

    /// <summary>
    /// Sauvegarde les options en JSON
    /// </summary>
    public void Save()
    {
        JsonLoader.Save(PATH, this);
    }

    /// <summary>
    /// Définit la vitesse d'animation par index
    /// </summary>
    public void SetEnemyActionsAnimationSpeedTo(int index)
    {
        if (index >= 0 && index < SPEEDS_FOR_ANIMATIONS.Length)
        {
            EnemyActionsAnimationSpeed = SPEEDS_FOR_ANIMATIONS[index];
            Save();
        }
    }

    /// <summary>
    /// Définit le type de remplacement d'écran par index
    /// </summary>
    public void SetScreenReplacementTypeTo(int index)
    {
        if (index >= 0 && index < SCREEN_REPLACEMENT_TYPES.Length)
        {
            ScreenReplacementType = SCREEN_REPLACEMENT_TYPES[index];
            Save();
        }
    }

    /// <summary>
    /// Vérifie si l'index correspond à la vitesse actuelle
    /// </summary>
    public string ShowEnemyActionsAnimationSpeed(int index)
    {
        if (index >= 0 && index < SPEEDS_FOR_ANIMATIONS.Length)
        {
            return Math.Abs(EnemyActionsAnimationSpeed - SPEEDS_FOR_ANIMATIONS[index]) < 0.01
                ? "   (+)   "
                : $"[Enter {index + 1}]";
        }
        return string.Empty;
    }

    /// <summary>
    /// Vérifie si l'index correspond au type d'écran actuel
    /// </summary>
    public string ShowScreenReplacementType(int index)
    {
        if (index >= 0 && index < SCREEN_REPLACEMENT_TYPES.Length)
        {
            return ScreenReplacementType == SCREEN_REPLACEMENT_TYPES[index]
                ? "   (+)   "
                : $"[Enter {index + 1}]";
        }
        return string.Empty;
    }
}
