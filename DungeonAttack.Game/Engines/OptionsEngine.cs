using DungeonAttack.Models.Options;
using DungeonAttack.Renderers;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur des options (vitesse animation, type de clear screen)
/// </summary>
public class OptionsEngine(Options options)
{
    private readonly Options _options = options;

    /// <summary>
    /// Menu principal des options
    /// </summary>
    public void Main()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("options_choose_screen");
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "animation_speed":
                    AnimationSpeed();
                    break;
                case "screen_replacement_type":
                    ScreenReplacementType();
                    break;
            }
        }
    }

    /// <summary>
    /// Sous-menu: vitesse d'animation
    /// </summary>
    private void AnimationSpeed()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("options_animation_speed_screen", entity: _options);
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "speed_1":
                    _options.SetEnemyActionsAnimationSpeedTo(0);
                    break;
                case "speed_2":
                    _options.SetEnemyActionsAnimationSpeedTo(1);
                    break;
                case "speed_3":
                    _options.SetEnemyActionsAnimationSpeedTo(2);
                    break;
                case "speed_4":
                    _options.SetEnemyActionsAnimationSpeedTo(3);
                    break;
                case "speed_5":
                    _options.SetEnemyActionsAnimationSpeedTo(4);
                    break;
            }
        }
    }

    /// <summary>
    /// Sous-menu: type de remplacement d'écran
    /// </summary>
    private void ScreenReplacementType()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("options_screen_replacement_type_screen", entity: _options);
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "type_1":
                    _options.SetScreenReplacementTypeTo(0);
                    break;
                case "type_2":
                    _options.SetScreenReplacementTypeTo(1);
                    break;
                case "type_3":
                    _options.SetScreenReplacementTypeTo(2);
                    break;
                case "type_4":
                    _options.SetScreenReplacementTypeTo(3);
                    break;
            }
        }
    }
}
