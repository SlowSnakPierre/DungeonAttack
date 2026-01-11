using DungeonAttack.Actions;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Options;
using DungeonAttack.Models.Skills.ActiveSkills;
using DungeonAttack.Models.Skills.PassiveSkills;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Saves;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur de combat au tour par tour
/// </summary>
public class AttacksRoundEngine
{
    private readonly Hero _hero;
    private readonly Enemy _enemy;
    private readonly AttacksRoundMessage _messages;
    private readonly double _enemyAnimationSpeed;

    private double _heroDamage;
    private double _heroAccuracy;
    private readonly bool _heroBlockSuccessful;
    private string _heroAttackType = "";
    private bool _heroHit;
    private string _activeSkillMessage = "";

    private double _enemyDamage;
    private double _enemyAccuracy;
    private readonly bool _enemyBlockSuccessful;
    private string _enemyAttackType = "";

    public AttacksRoundEngine(Hero hero, Enemy enemy, AttacksRoundMessage messages)
    {
        _hero = hero;
        _enemy = enemy;
        _messages = messages;

        _heroDamage = Random.Shared.Next(_hero.MinDmg, _hero.MaxDmg + 1);
        _heroAccuracy = _hero.Accuracy;
        _heroBlockSuccessful = _hero.BlockChance >= Random.Shared.Next(1, 101);

        _enemyDamage = Random.Shared.Next(_enemy.MinDmg, _enemy.MaxDmg + 1);
        _enemyAccuracy = _enemy.Accuracy;
        _enemyBlockSuccessful = _enemy.BlockChance >= Random.Shared.Next(1, 101);

        _enemyAnimationSpeed = Options.Load().EnemyActionsAnimationSpeed;
    }

    /// <summary>
    /// Exécute un tour de combat complet
    /// </summary>
    public void Action()
    {
        HeroSelectTypeOfAttack();
        EnemySelectTypeOfAttack();
        CountHeroFinalDamage();
        CountEnemyFinalDamage();
        HeroAttackEffects();

        if (_enemy.Hp > 0)
            EnemyAttackEffects();

        MainMessage tempMessages = new();
        tempMessages.Log.AddRange(_messages.Log);

        HeroActions.RegenerationHpMp(_hero, tempMessages);

        if (_enemy.Hp > 0)
            EnemyActions.RegenerationHpMp(_enemy, tempMessages);

        _messages.Log.AddRange(tempMessages.Log.Skip(_messages.Log.Count));

        RoundResult();
    }

    /// <summary>
    /// Vérifie si le héros veut fuir (si HP < 15%)
    /// </summary>
    public bool HeroRun()
    {
        if (_hero.Hp < (_hero.HpMax * 0.15) && _hero.Hp > 0 && _enemy.Hp > 0)
        {
            _messages.ClearLog();
            _messages.Main = "You are on the threshold of death";

            List<MenuOption> menuOptions = [
                new("N", "Stay and fight"),
                new("Y", "Try to escape")
            ];

            MenuSelector? selector = null;

            void RenderAction()
            {
                _messages.Actions = BuildRunMenuLine(selector);
                DisplayBattleScreenWithArt("normal");
            }

            selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
            string runSelect = selector.Select();

            if (runSelect == "Y")
            {
                int runChance = Random.Shared.Next(0, 41) + Random.Shared.Next(0, 41) +
                               Random.Shared.Next(0, 41) + Random.Shared.Next(0, 41);

                _messages.Actions = "To continue press Enter";
                _messages.Log.Add($"Random run chance = {runChance}, {_enemy.Name}'s accuracy = {_enemy.Accuracy}");

                if (runChance > _enemy.Accuracy)
                {
                    _messages.Main = "Managed to escape";
                    _messages.Log.Add("The coward ran away");
                    DisplayBattleScreenWithArt("attack");
                    Console.ReadLine();
                    return true;
                }
                else
                {
                    _hero.Hp -= (int)Math.Round(_enemyDamage);
                    _messages.Main = "Failed to escape";
                    _messages.Log.Add($"{_enemy.Name} dealt {(int)Math.Round(_enemyDamage)} damage");
                    DisplayBattleScreenWithArt("attack");
                    Console.ReadLine();

                    if (_hero.Hp <= 0)
                    {
                        MainMessage messages = new()
                        {
                            Main = "You are dead - you cowardly dog!"
                        };
                        foreach (string log in _messages.Log)
                            messages.Log.Add(log);

                        new DeleteHeroInRun(_hero, "game_over", messages)
                            .AddCampLootAndDeleteHeroFile();
                    }
                }
            }
        }

        return false;
    }

    private static string BuildRunMenuLine(MenuSelector? selector)
    {
        string indicator1 = selector?.GetIndicatorForValue("N") ?? " ";
        string indicator2 = selector?.GetIndicatorForValue("Y") ?? " ";
        return $"{indicator1} Stay and fight    {indicator2} Try to escape";
    }

    public bool HeroDead() => _hero.Hp <= 0;

    #region Hero Attack Selection

    private void HeroSelectTypeOfAttack()
    {
        // Vider le buffer de la console pour éviter les inputs préenregistrés
        while (Console.KeyAvailable)
            Console.ReadKey(true);

        bool success = false;

        while (!success)
        {
            string skillName = _hero.ActiveSkill?.Name ?? "No Skill";

            List<MenuOption> menuOptions = [
                new("1", "Hit Body"),
                new("2", "Hit Head"),
                new("3", "Hit Legs"),
                new("4", $"Hit by {skillName}")
            ];

            MenuSelector? selector = null;

            void RenderAction()
            {
                _messages.Main = BuildAttackMenuLine(menuOptions, selector);
                _messages.Actions = "";
                DisplayBattleScreenWithArt("normal");
            }

            selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
            string selectedType = selector.Select();

            _messages.ClearLog();

            success = selectedType switch
            {
                "2" => HeroHeadAttackType(),
                "3" => HeroLegsAttackType(),
                "4" => HeroSkillAttackType(),
                _ => HeroBodyAttackType()
            };
        }
    }

    private static string BuildAttackMenuLine(List<MenuOption> options, MenuSelector? selector)
    {
        List<string> parts = [];
        foreach (MenuOption option in options)
        {
            string indicator = selector?.GetIndicatorForValue(option.Value) ?? " ";
            parts.Add($"{indicator} {option.Label}");
        }
        return string.Join("    ", parts);
    }

    private bool HeroBodyAttackType()
    {
        _heroAttackType = "to the body";
        return true;
    }

    private bool HeroHeadAttackType()
    {
        _heroDamage *= 1.5;
        _heroAccuracy *= 0.7;
        _heroAttackType = "to the head";
        return true;
    }

    private bool HeroLegsAttackType()
    {
        _heroDamage *= 0.7;
        _heroAccuracy *= 1.5;
        _heroAttackType = "in the legs";
        return true;
    }

    private bool HeroSkillAttackType()
    {
        if (_hero.ActiveSkill == null)
        {
            _messages.Log.Add("No active skill available");
            return false;
        }

        if (_hero.Mp >= _hero.ActiveSkill.MpCost)
        {
            _heroDamage *= _hero.ActiveSkill.DamageMod;
            _heroAccuracy *= _hero.ActiveSkill.AccuracyMod;
            _heroAttackType = _hero.ActiveSkill.Name;
            _hero.Mp -= _hero.ActiveSkill.MpCost;
            return true;
        }
        else
        {
            _messages.Log.Add($"Not enough MP to {_hero.ActiveSkill.Name}");
            return false;
        }
    }

    #endregion

    #region Enemy Attack Selection

    private void EnemySelectTypeOfAttack()
    {
        int selectedType = Random.Shared.Next(1, 5);

        switch (selectedType)
        {
            case 1:
                EnemyHeadAttackType();
                break;
            case 2:
                EnemyLegsAttackType();
                break;
            default:
                EnemyBodyAttackType();
                break;
        }
    }

    private void EnemyBodyAttackType()
    {
        _enemyAttackType = "to the body";
    }

    private void EnemyHeadAttackType()
    {
        _enemyDamage *= 1.5;
        _enemyAccuracy *= 0.7;
        _enemyAttackType = "to the head";
    }

    private void EnemyLegsAttackType()
    {
        _enemyDamage *= 0.7;
        _enemyAccuracy *= 1.5;
        _enemyAttackType = "in the legs";
    }

    #endregion

    #region Damage Calculation

    private void CountHeroFinalDamage()
    {
        HeroBeforeHitPassiveSkillEffects();
        HeroDamageReducedByEnemyBlock();
        HeroDamageReducedByEnemyArmor();
    }

    private void HeroBeforeHitPassiveSkillEffects()
    {
        if (_hero.PassiveSkill is Berserk berserk)
        {
            _heroDamage *= berserk.DamageCoeff;
        }
    }

    private void HeroDamageReducedByEnemyBlock()
    {
        if (_enemyBlockSuccessful)
        {
            _heroDamage /= _enemy.BlockPowerCoeff;
        }
    }

    private void HeroDamageReducedByEnemyArmor()
    {
        int armorReduction = _enemy.Armor > _hero.ArmorPenetration ? _enemy.Armor - _hero.ArmorPenetration : 0;
        _heroDamage -= armorReduction;
        _heroDamage = Math.Max(0, _heroDamage);
    }

    private void CountEnemyFinalDamage()
    {
        EnemyDamageReducedByHeroBlock();
        EnemyDamageReducedByHeroArmor();
    }

    private void EnemyDamageReducedByHeroBlock()
    {
        if (_heroBlockSuccessful)
        {
            _enemyDamage /= _hero.BlockPowerCoeff;
        }
    }

    private void EnemyDamageReducedByHeroArmor()
    {
        int armorReduction = _hero.Armor > _enemy.ArmorPenetration ? _hero.Armor - _enemy.ArmorPenetration : 0;
        _enemyDamage -= armorReduction;
        _enemyDamage = Math.Max(0, _enemyDamage);
    }

    #endregion

    #region Attack Effects

    private void HeroAttackEffects()
    {
        _heroHit = _heroAccuracy >= Random.Shared.Next(1, 101);
        HeroHitOrMiss();
        HeroAfterHitPassiveSkillEffects();
        if (_hero.ActiveSkill != null && _heroAttackType == _hero.ActiveSkill.Name)
            HeroAfterHitActiveSkillEffects();

        _messages.Main = $"{_hero.Name} attack {_enemy.Name}";
        _messages.Actions = "";
        DisplayBattleScreenWithArt("damaged");
    }

    private void HeroHitOrMiss()
    {
        if (_heroHit)
        {
            string blockMessage = _enemyBlockSuccessful ?
                $"{_enemy.Name} blocked {_enemy.BlockPowerInPercents}% damage. " : "";

            _enemy.Hp -= (int)Math.Round(_heroDamage);
            _messages.Log.Add($"{blockMessage}You dealt {(int)Math.Round(_heroDamage)} damage {_heroAttackType}");
        }
        else
        {
            _messages.Log.Add($"You missed {_heroAttackType}");
        }
    }

    private void HeroAfterHitPassiveSkillEffects()
    {
        if (_hero.PassiveSkill is Concentration concentration && _heroHit)
        {
            double damageBonus = concentration.DamageBonus;
            if (damageBonus > 0)
            {
                _enemy.Hp -= (int)Math.Round(damageBonus);
                if (_messages.Log.Count > 0)
                    _messages.Log[^1] += $", additional damage from concentration {Math.Round(damageBonus, 1)}";
            }
        }
        else if (_hero.PassiveSkill is Dazed dazed && _heroHit)
        {
            if (_heroDamage * dazed.HpPartCoeff > _enemy.Hp / 2.0)
            {
                double accuracyReduceCoef = dazed.AccuracyReduceCoeff;
                _enemyAccuracy *= accuracyReduceCoef;
                if (_messages.Log.Count > 0)
                    _messages.Log[^1] += $" and dazed, reducing accuracy to {(int)Math.Round(_enemy.Accuracy * accuracyReduceCoef)}";
            }
        }
    }

    private void HeroAfterHitActiveSkillEffects()
    {
        if (_hero.ActiveSkill != null && _hero.ActiveSkill is TraumaticStrike traumatic && _heroHit && _heroDamage > 0)
        {
            _enemyDamage *= traumatic.EffectCoeff;
            _activeSkillMessage = $"{_enemy.Name} injured, damage reduced by {traumatic.Effect}%. ";
        }
    }

    private void EnemyAttackEffects()
    {
        Thread.Sleep((int)(_enemyAnimationSpeed * 1000));

        _messages.Main = $"{_enemy.Name}'s move";
        _messages.Actions = $"{_enemy.Name} chooses the method of attack";
        DisplayBattleScreenWithArt("normal");

        EnemyHitOrMiss();

        Thread.Sleep((int)(_enemyAnimationSpeed * 1000));

        _messages.Main = $"{_enemy.Name} attacks";
        _messages.Actions = "";

        string art = _enemyAttackType == "to the head" ? "attack_head" :
                     _enemyAttackType == "in the legs" ? "attack_legs" : "attack";
        DisplayBattleScreenWithArt(art);
    }

    private void EnemyHitOrMiss()
    {
        bool enemyHit = _enemyAccuracy >= Random.Shared.Next(1, 101);

        if (enemyHit)
        {
            _hero.Hp -= (int)Math.Round(_enemyDamage);

            string blockMessage = _heroBlockSuccessful ?
                $"You have blocked {_hero.BlockPowerInPercents}% damage. " : "";

            string mainMessage = $"{_enemy.Name} dealt {(int)Math.Round(_enemyDamage)} damage {_enemyAttackType}";
            _messages.Log.Add(_activeSkillMessage + blockMessage + mainMessage);
        }
        else
        {
            _messages.Log.Add($"{_enemy.Name} missed {_enemyAttackType}");
        }
    }

    #endregion

    private void RoundResult()
    {
        if (_hero.Hp <= 0)
        {
            Thread.Sleep((int)(_enemyAnimationSpeed * 1000));

            MainMessage messages = new()
            {
                Main = "You're dead! To continue press Enter"
            };
            foreach (string log in _messages.Log)
                messages.Log.Add(log);

            new DeleteHeroInRun(_hero, "game_over", messages)
                .AddCampLootAndDeleteHeroFile();
        }
        else if (_enemy.Hp <= 0)
        {
            Thread.Sleep((int)(_enemyAnimationSpeed * 1000));

            _messages.Main = $"{_enemy.Name} dead, victory!";
            _messages.Actions = "To continue press Enter";
            DisplayBattleScreenWithArt("dead");
            Console.ReadLine();
        }
        else
        {
            Thread.Sleep((int)(_enemyAnimationSpeed * 1000));
        }
    }

    /// <summary>
    /// Affiche l'écran de bataille avec l'art de l'ennemi
    /// </summary>
    private void DisplayBattleScreenWithArt(string artType)
    {
        MainRenderer renderer = new("battle_screen",
            characters: [_hero, _enemy],
            entity: _messages);

        renderer.AddArt(artType, _enemy);
        renderer.RenderScreen();
    }
}
