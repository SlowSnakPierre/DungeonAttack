using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;

namespace DungeonAttack.Actions;

/// <summary>
/// Actions statiques pour le héros
/// </summary>
public static class HeroActions
{
    /// <summary>
    /// Repos entre les combats (restaure HP et MP)
    /// </summary>
    public static void Rest(Hero hero, MainMessage messages)
    {
        if (hero.Hp < hero.HpMax)
        {
            int recovered = (int)Math.Min(hero.RecoveryHp, hero.HpMax - hero.Hp);
            hero.Hp += recovered;
            messages.Log.Add($"After resting, you restore {Math.Round(hero.RecoveryHp)} HP, now you have {hero.Hp}/{hero.HpMax} HP");
        }

        if (hero.Mp < hero.MpMax)
        {
            int recovered = (int)Math.Min(hero.RecoveryMp, hero.MpMax - hero.Mp);
            hero.Mp += recovered;
            messages.Log.Add($"After resting, you restore {Math.Round(hero.RecoveryMp)} MP, now you have {hero.Mp}/{hero.MpMax} MP");
        }
    }

    /// <summary>
    /// Régénération pendant le combat
    /// </summary>
    public static void RegenerationHpMp(Hero hero, MainMessage messages)
    {
        if (hero.RegenHp > 0 && hero.Hp < hero.HpMax)
        {
            int regen = Math.Min(hero.RegenHp, hero.HpMax - hero.Hp);
            hero.Hp += regen;
            messages.Log.Add($"You regenerating {hero.RegenHp} HP");
        }

        if (hero.RegenMp > 0 && hero.Mp < hero.MpMax)
        {
            int regen = Math.Min(hero.RegenMp, hero.MpMax - hero.Mp);
            hero.Mp += regen;

            if (messages.Log.Count > 0 && messages.Log[^1].Contains("regenerating"))
            {
                messages.Log[^1] += $". You regenerating {hero.RegenMp} MP";
            }
            else
            {
                messages.Log.Add($"You regenerating {hero.RegenMp} MP");
            }
        }
    }

    /// <summary>
    /// Ajoute de l'expérience et gère les montées de niveau
    /// </summary>
    public static void AddExpAndHeroLevelUp(Hero hero, int addedExp, MainMessage messages)
    {
        hero.Exp += addedExp;
        messages.Log.Add($"You have gained {addedExp} exp, now you have {hero.Exp} exp");

        int sumNewLevels = 0;

        for (int i = 0; i < hero.ExpLvl.Length; i++)
        {
            if (hero.Exp >= hero.ExpLvl[i] && hero.Level < i)
            {
                int newLevels = i - hero.Level;
                hero.StatPoints += newLevels;
                hero.SkillPoints += newLevels;
                hero.Level += newLevels;
                sumNewLevels += newLevels;
            }
        }

        if (sumNewLevels > 0)
        {
            string s = sumNewLevels > 1 ? "s" : "";
            messages.Log.Add($"You have gained {sumNewLevels} new level{s}, now your level is {hero.Level}");
            messages.Log.Add($"You have gained {sumNewLevels} stat point{s} and {sumNewLevels} skill point{s}");
            messages.Log.Add($"Now you have {hero.StatPoints} stat point{s} and {hero.SkillPoints} skill point{s}");
        }
    }
}
