using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Services.Loot;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur de loot après un combat victorieux
/// </summary>
public class LootRoundEngine(Hero hero, Enemy enemy, bool run)
{
    private readonly Hero _hero = hero;
    private readonly Enemy _enemy = enemy;
    private readonly bool _run = run;
    private readonly MainMessage _messages = new();

    /// <summary>
    /// Exécute le loot (Monolith + Enemy)
    /// </summary>
    public void Action()
    {
        if (_run)
            return;

        MonolithLoot();

        _messages.ClearLog();

        EnemyLoot();
    }

    private void MonolithLoot()
    {
        MonolithLootService monolithLoot = new(_hero, _enemy, _messages);
        monolithLoot.Looting();
    }

    private void EnemyLoot()
    {
        EnemyLootService enemyLoot = new(_hero, _enemy, _messages);
        enemyLoot.Looting();
    }

    public MainMessage GetMessages() => _messages;
}
