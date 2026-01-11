using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;

namespace DungeonAttack.Actions;

/// <summary>
/// Actions statiques pour les ennemis
/// </summary>
public static class EnemyActions
{
    /// <summary>
    /// Régénération HP/MP de l'ennemi pendant le combat
    /// </summary>
    public static void RegenerationHpMp(Enemy enemy, MainMessage messages)
    {
        if (enemy.RegenHp > 0 && enemy.Hp < enemy.HpMax)
        {
            int regen = Math.Min(enemy.RegenHp, enemy.HpMax - enemy.Hp);
            enemy.Hp += regen;
            messages.Log.Add($"Enemy regenerating {enemy.RegenHp} HP");
        }

        if (enemy.RegenMp > 0 && enemy.Mp < enemy.MpMax)
        {
            int regen = Math.Min(enemy.RegenMp, enemy.MpMax - enemy.Mp);
            enemy.Mp += regen;

            if (messages.Log.Count > 0 && messages.Log[^1].Contains("regenerating"))
            {
                messages.Log[^1] += $". Enemy regenerating {enemy.RegenMp} MP";
            }
            else
            {
                messages.Log.Add($"Enemy regenerating {enemy.RegenMp} MP");
            }
        }
    }
}
