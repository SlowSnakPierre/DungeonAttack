using DungeonAttack.Factories;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Controllers.Characters;

/// <summary>
/// Créateur d'ennemis avec logique de leveling
/// </summary>
public class EnemyCreator
{
    private const int BOSS_LEVEL = 25;
    private const int CHANCE_STEP = 4;

    private readonly string _dungeonName;
    private readonly int _standardEnemiesCount;
    private readonly bool _boss;
    private readonly int _standardChance;

    public EnemyCreator(int leveling, string dungeonName)
    {
        _dungeonName = dungeonName;

        Dictionary<string, object> enemiesData = YamlLoader.Load<Dictionary<string, object>>(
            $"Data/characters/enemyes/{dungeonName}.yml");

        _standardEnemiesCount = enemiesData.Count - (enemiesData.ContainsKey("boss") ? 1 : 0);

        _boss = leveling >= BOSS_LEVEL;

        _standardChance = Random.Shared.Next(1, 10) + Random.Shared.Next(0, leveling + 1);
    }

    /// <summary>
    /// Crée un nouvel ennemi (boss ou standard selon le leveling)
    /// </summary>
    public Enemy CreateNewEnemy()
    {
        return _boss ? CreateBossEnemy() : CreateStandardEnemy();
    }

    private Enemy CreateBossEnemy()
    {
        return EnemyFactory.Create("boss", _dungeonName);
    }

    /// <summary>
    /// Sélectionne un ennemi standard selon la chance calculée
    /// </summary>
    private Enemy CreateStandardEnemy()
    {
        for (int n = 1; n <= _standardEnemiesCount; n++)
        {
            if (_standardChance <= n * CHANCE_STEP)
            {
                return EnemyFactory.Create($"e{n}", _dungeonName);
            }
        }

        return EnemyFactory.Create($"e{_standardEnemiesCount}", _dungeonName);
    }
}
