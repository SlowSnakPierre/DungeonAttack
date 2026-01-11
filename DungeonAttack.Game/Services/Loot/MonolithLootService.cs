using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;

namespace DungeonAttack.Services.Loot;

/// <summary>
/// Service de loot du monolithe (points basés sur difficulté de l'ennemi)
/// </summary>
public class MonolithLootService(Hero hero, Enemy enemy, MainMessage messages)
{
    private readonly Hero _hero = hero;
    private readonly Enemy _enemy = enemy;
    private readonly MainMessage _messages = messages;
    private double _probability = 0;
    private int _points = 0;

    /// <summary>
    /// Effectue le loot du monolithe (calcule et ajoute les points)
    /// </summary>
    public void Looting()
    {
        CountPoints();

        if (_points > 0)
        {
            _hero.MonolithPoints += _points;
            DisplayScreen();
        }
    }

    /// <summary>
    /// Calcule les points du monolithe basés sur le ratio stats ennemi/héros
    /// Plus l'ennemi est fort relativement au héros, plus il donne de points
    /// </summary>
    private void CountPoints()
    {
        double sum = 0;

        (string, double, double)[] stats =
        [
            ("hp_max", (double)_enemy.HpMax, (double)_hero.HpMax),
            ("mp_max", (double)_enemy.MpMax, (double)_hero.MpMax),
            ("min_dmg", (double)_enemy.MinDmg, (double)_hero.MinDmg),
            ("max_dmg", (double)_enemy.MaxDmg, (double)_hero.MaxDmg),
            ("regen_hp", (double)_enemy.RegenHp, (double)_hero.RegenHp),
            ("regen_mp", (double)_enemy.RegenMp, (double)_hero.RegenMp),
            ("armor", (double)_enemy.Armor, (double)_hero.Armor),
            ("accuracy", (double)_enemy.Accuracy, (double)_hero.Accuracy)
        ];

        foreach ((string? name, double enemyStat, double heroStat) in stats)
        {
            double ratio = heroStat <= 1 ? enemyStat : enemyStat / heroStat;
            sum += ratio;
        }

        _probability = sum / stats.Length;

        int basePoints = (int)Math.Floor(_probability);
        double fractionalPart = _probability - basePoints;
        int bonusPoint = fractionalPart > Random.Shared.NextDouble() ? 1 : 0;

        _points = basePoints + bonusPoint;
    }

    /// <summary>
    /// Affiche le message de loot du monolithe
    /// </summary>
    private void DisplayScreen()
    {
        _messages.Main = "You hear Monolith. Press Enter to continue";
        _messages.Log.Add($"probability is {_probability:F2}");
        _messages.Log.Add($"The death of the {_enemy.Name} filled the monolith on {_points}");
        _messages.Log.Add($"You now have {_hero.MonolithPoints} Monolith points");

        MainRenderer renderer = new("messages_screen", entity: _messages);
        renderer.RenderScreen();
        Console.ReadLine();
    }

    /// <summary>
    /// Retourne le nombre de points gagnés (utile pour les tests)
    /// </summary>
    public int GetPoints() => _points;

    /// <summary>
    /// Retourne la probabilité calculée (utile pour les tests)
    /// </summary>
    public double GetProbability() => _probability;
}
