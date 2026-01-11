using DungeonAttack.Controllers.Skills;
using DungeonAttack.Factories;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills;
using DungeonAttack.Renderers;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DungeonAttack.Controllers.Characters;

/// <summary>
/// Contrôleur pour la création de héros avec prompts utilisateur
/// </summary>
public partial class HeroCreator(string dungeonName, Warehouse warehouse, Monolith monolith, Shop shop, StatisticsTotal stats)
{
    private readonly string _dungeonName = dungeonName;
    private readonly Warehouse _warehouse = warehouse;
    private readonly Monolith _monolith = monolith;
    private readonly Shop _shop = shop;
    private readonly StatisticsTotal _stats = stats;
    private readonly MainMessage _messages = new();

    private Hero? _hero;
    private string _heroName = string.Empty;
    private string _background = string.Empty;

    /// <summary>
    /// Crée un héros avec toute la séquence de prompts utilisateur
    /// </summary>
    public Hero? Create()
    {
        NameInput();

        if (string.IsNullOrEmpty(_heroName))
            return null;

        BackgroundSelect();

        _hero = HeroFactory.Create(_heroName, _background, _dungeonName);

        MonolithBonusesApply();
        StatisticsBonusesApply();
        ShopBonusesApply();

        ActiveSkillSelect();
        PassiveSkillSelect();
        CampSkillSelect();

        Cheating();

        _hero.Statistics = new StatisticsRun(_dungeonName, newObj: true);

        return _hero;
    }

    #region Name Input

    private void NameInput()
    {
        while (true)
        {
            _messages.ClearLog();

            if (string.IsNullOrEmpty(_messages.Main))
            {
                _messages.Main = "Enter character name";
            }
            _messages.Log.Add("The character name must contain at least 1 letter and be no more than 20 characters");

            MainRenderer renderer = new("messages_screen", entity: _messages);
            renderer.AddArt("scroll", "choose_name");
            renderer.RenderScreen();

            int screenWidth = 120;
            int inputLine = 29;
            int maxLength = 20;
            string input = "";

            try { Console.CursorVisible = false; } catch { }

            void DisplayInput(string text)
            {
                try
                {
                    Console.SetCursorPosition(2, inputLine);
                    Console.Write(new string(' ', screenWidth - 4));

                    int textWidth = text.Length + 4;
                    int startX = (screenWidth - textWidth) / 2;

                    Console.SetCursorPosition(startX, inputLine);
                    Console.Write("► " + text + " ◄");
                }
                catch { }
            }

            DisplayInput("");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input[..^1];
                        DisplayInput(input);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    input = "";
                    DisplayInput(input);
                }
                else if (!char.IsControl(keyInfo.KeyChar) && input.Length < maxLength)
                {
                    input += keyInfo.KeyChar;
                    DisplayInput(input);
                }
            }

            input = input.Trim();

            // Validate name (must contain at least 1 letter - Latin or Cyrillic)
            if (!LetterValidationRegex().IsMatch(input))
            {
                _messages.Main = $"{input} is an incorrect name. The name must contain at least one letter";
                continue;
            }

            if (input.Length > 20)
            {
                _messages.Main = $"{input} is an incorrect name. The name must be no more than 20 characters";
                continue;
            }

            _heroName = input;
            break;
        }
    }

    #endregion

    #region Background Selection

    private void BackgroundSelect()
    {
        string content = File.ReadAllText("Data/Characters/heroes.yml");
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        Dictionary<string, Dictionary<string, object>> heroesData = deserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(content);
        List<KeyValuePair<string, Dictionary<string, object>>> heroes = [.. heroesData
            .Where(kvp => kvp.Key != "cheater")
            .OrderBy(kvp => kvp.Value.TryGetValue("n", out object? value) ? Convert.ToInt32(value) : 999)];

        int selectedIndex = 0;
        int tableOffset = 5;
        int indicatorX = tableOffset + 2;
        int firstDataLine = 6;

        void RenderBackgroundMenu()
        {
            _messages.Main = "Select a background";
            _messages.Log.Clear();

            string header = "Background:".PadRight(13) +
                           "HP:".PadRight(10) + "MP:".PadRight(10) +
                           "Min dmg:".PadRight(10) + "Max dmg:".PadRight(10) +
                           "Accuracy:".PadRight(10) + "Armor:".PadRight(10) + "Skill pts:";
            _messages.Log.Add(new string(' ', tableOffset) + header);

            for (int i = 0; i < heroes.Count; i++)
            {
                Dictionary<string, object> hero = heroes[i].Value;
                string name = hero["name"]?.ToString() ?? "";
                int hp = Convert.ToInt32(hero["hp"]);
                int mp = Convert.ToInt32(hero["mp"]);
                int minDmg = Convert.ToInt32(hero["min_dmg"]);
                int maxDmg = Convert.ToInt32(hero["max_dmg"]);
                int accuracy = Convert.ToInt32(hero["accuracy"]);
                int armor = Convert.ToInt32(hero["armor"]);
                int skillPoints = Convert.ToInt32(hero["skill_points"]);

                _messages.Log.Add(new string(' ', tableOffset) + new string('-', 103));

                string indicator = (i == selectedIndex) ? "►" : " ";
                _messages.Log.Add(new string(' ', tableOffset) + $"{indicator} " +
                                 name.PadRight(13) +
                                 hp.ToString().PadRight(10) +
                                 mp.ToString().PadRight(10) +
                                 minDmg.ToString().PadRight(10) +
                                 maxDmg.ToString().PadRight(10) +
                                 accuracy.ToString().PadRight(10) +
                                 armor.ToString().PadRight(10) +
                                 skillPoints.ToString());
            }

            MainRenderer renderer = new("messages_full_screen", entity: _messages);
            renderer.RenderScreen();
        }

        // Fonction pour mettre à jour seulement l'indicateur (sans re-render tout)
        void UpdateIndicator(int oldIndex, int newIndex)
        {
            try
            {
                int oldY = firstDataLine + (oldIndex * 2);
                Console.SetCursorPosition(indicatorX, oldY);
                Console.Write(" ");

                int newY = firstDataLine + (newIndex * 2);
                Console.SetCursorPosition(indicatorX, newY);
                Console.Write("►");
            }
            catch { }
        }

        try { Console.CursorVisible = false; } catch { }

        RenderBackgroundMenu();

        bool easterEgg = false;
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.LeftArrow)
            {
                int oldIndex = selectedIndex;
                selectedIndex = (selectedIndex - 1 + heroes.Count) % heroes.Count;
                UpdateIndicator(oldIndex, selectedIndex);
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.RightArrow)
            {
                int oldIndex = selectedIndex;
                selectedIndex = (selectedIndex + 1) % heroes.Count;
                UpdateIndicator(oldIndex, selectedIndex);
            }
            else
            {
                // Easter egg: touche invalide = drunk!
                easterEgg = true;
                break;
            }
        }

        _messages.ClearLog();

        if (easterEgg)
        {
            // Drunk background (easter egg)
            _messages.Main = "You pressed the wrong key, you STUPID DRUNK!!! Press Enter to continue";
            int offset = 44;
            _messages.Log.Clear();
            _messages.Log.AddRange(Enumerable.Repeat("", 8));
            _messages.Log.Add(new string(' ', offset) + " DRUNK background stats is:");
            _messages.Log.Add(new string(' ', offset) + "----------------------------");
            _messages.Log.Add("");
            _messages.Log.Add(new string(' ', offset) + " HP                      85");
            _messages.Log.Add("");
            _messages.Log.Add(new string(' ', offset) + " MP                      85");
            _messages.Log.Add("");
            _messages.Log.Add(new string(' ', offset) + " DMG                    4-4");
            _messages.Log.Add("");
            _messages.Log.Add(new string(' ', offset) + " Accurasy                70");

            MainRenderer renderer = new("messages_full_screen", entity: _messages);
            renderer.RenderScreen();
            Console.ReadKey(true);

            _background = "drunk";
        }
        else
        {
            _background = heroes[selectedIndex].Key;
        }
    }

    #endregion

    #region Bonuses

    private void MonolithBonusesApply()
    {
        if (_hero == null || _monolith == null)
            return;

        if (_monolith.Hp > 0)
        {
            int hpBonus = _monolith.Hp * 5;
            _hero.HpMax += hpBonus;
            _hero.Hp += hpBonus;
            _messages.Log.Add($"Monolith HP bonus: +{hpBonus}");
        }

        if (_monolith.Mp > 0)
        {
            int mpBonus = _monolith.Mp * 3;
            _hero.MpMax += mpBonus;
            _hero.Mp += mpBonus;
            _messages.Log.Add($"Monolith MP bonus: +{mpBonus}");
        }

        int damageCount = _monolith.Damage;
        while (damageCount > 0)
        {
            _hero.AddDmgBase();
            damageCount--;
        }

        if (_monolith.Accuracy > 0)
        {
            int accBonus = _monolith.Accuracy * 3;
            _hero.AccuracyBase += accBonus;
            _messages.Log.Add($"Monolith accuracy bonus: +{accBonus}");
        }

        if (_monolith.Armor > 0)
        {
            int armorBonus = _monolith.Armor * 2;
            _hero.ArmorBase += armorBonus;
            _messages.Log.Add($"Monolith armor bonus: +{armorBonus}");
        }

        if (_monolith.StatPoints > 0)
        {
            _hero.StatPoints += _monolith.StatPoints;
            _messages.Log.Add($"Monolith stat points bonus: +{_monolith.StatPoints}");
        }

        if (_monolith.SkillPoints > 0)
        {
            _hero.SkillPoints += _monolith.SkillPoints;
            _messages.Log.Add($"Monolith skill points bonus: +{_monolith.SkillPoints}");
        }

        if (_monolith.RegenHp > 0)
        {
            _hero.RegenHpBase += _monolith.RegenHp;
            _messages.Log.Add($"Monolith HP regen bonus: +{_monolith.RegenHp}");
        }

        if (_monolith.RegenMp > 0)
        {
            _hero.RegenMpBase += _monolith.RegenMp;
            _messages.Log.Add($"Monolith MP regen bonus: +{_monolith.RegenMp}");
        }

        if (_monolith.ArmorPenetration > 0)
        {
            _hero.ArmorPenetrationBase += _monolith.ArmorPenetration;
            _messages.Log.Add($"Monolith armor penetration bonus: +{_monolith.ArmorPenetration}");
        }

        if (_monolith.BlockChance > 0)
        {
            _hero.BlockChanceBase += _monolith.BlockChance;
            _messages.Log.Add($"Monolith block chance bonus: +{_monolith.BlockChance}");
        }
    }

    private void StatisticsBonusesApply()
    {
        if (_hero == null || _stats == null)
            return;

        Dictionary<string, Dictionary<string, int>> statsData = _stats.Data;

        if (statsData.TryGetValue("bandits", out Dictionary<string, int>? banditsData))
        {
            foreach (KeyValuePair<string, int> kvp in banditsData)
            {
                string enemy = kvp.Key;
                int count = kvp.Value;

                if (enemy == "rabble" && count >= 30)
                {
                    _hero.Weapon = (Weapon)AmmunitionFactory.Create("weapon", "stick");
                }
                else if (enemy == "rabid_dog" && count >= 30)
                {
                    _hero.HpMax += 2;
                    _hero.Hp += 2;
                }
                else if (enemy == "poacher" && count >= 30)
                {
                    _hero.AccuracyBase += 1;
                }
                else if (enemy == "thug" && count >= 30)
                {
                    _hero.HpMax += 5;
                    _hero.Hp += 5;
                }
                else if (enemy == "deserter" && count >= 30)
                {
                    _hero.StatPoints += 1;
                }
                else if (enemy == "bandit_leader" && count >= 5)
                {
                    _hero.SkillPoints += 1;
                }
            }
        }

        if (statsData.TryGetValue("undeads", out Dictionary<string, int>? undeadsData))
        {
            foreach (KeyValuePair<string, int> kvp in undeadsData)
            {
                string enemy = kvp.Key;
                int count = kvp.Value;

                if (enemy == "zombie" && count >= 30)
                {
                    _hero.ArmsArmor = (ArmsArmor)AmmunitionFactory.Create("arms_armor", "worn_gloves");
                }
                else if (enemy == "skeleton" && count >= 30)
                {
                    _hero.MpMax += 3;
                    _hero.Mp += 3;
                }
                else if (enemy == "ghost" && count >= 30)
                {
                    _hero.AccuracyBase += 1;
                }
                else if (enemy == "fat_ghoul" && count >= 30)
                {
                    _hero.HpMax += 7;
                    _hero.Hp += 7;
                }
                else if (enemy == "skeleton_soldier" && count >= 30)
                {
                    _hero.BlockChanceBase += 3;
                }
                else if (enemy == "zombie_knight" && count >= 5)
                {
                    _hero.RegenMpBase += 1;
                }
            }
        }

        if (statsData.TryGetValue("swamp", out Dictionary<string, int>? swampData))
        {
            foreach (KeyValuePair<string, int> kvp in swampData)
            {
                string enemy = kvp.Key;
                int count = kvp.Value;

                if (enemy == "leech" && count >= 30)
                {
                    _hero.MpMax += 3;
                    _hero.Mp += 3;
                }
                else if (enemy == "goblin" && count >= 30)
                {
                    _hero.Shield = (Shield)AmmunitionFactory.Create("shield", "holey_wicker_buckler");
                }
                else if (enemy == "sworm" && count >= 30)
                {
                    _hero.HpMax += 3;
                    _hero.Hp += 3;
                }
                else if (enemy == "spider" && count >= 30)
                {
                    _hero.AccuracyBase += 1;
                }
                else if (enemy == "orc" && count >= 30)
                {
                    _hero.MaxDmgBase += 1;
                }
                else if (enemy == "ancient_snail" && count >= 5)
                {
                    _hero.ArmorBase += 1;
                }
            }
        }
    }

    private void ShopBonusesApply()
    {
        if (_hero != null && _warehouse != null)
            _warehouse.TakeAmmunitionBy(_hero);
    }

    #endregion

    #region Skill Selection

    private void ActiveSkillSelect()
    {
        _messages.Main = "Select an active skill";
        SelectSkill("active_skill");
    }

    private void PassiveSkillSelect()
    {
        _messages.Main = "Select a passive skill";
        SelectSkill("passive_skill");
    }

    private void CampSkillSelect()
    {
        _messages.Main = "Select a non-combat skill";
        SelectSkill("camp_skill");
    }

    /// <summary>
    /// Cheat code detection
    /// Si le nom est BAMBUGA, change l'arme vers bambuga et le nom vers Cheater
    /// </summary>
    private void Cheating()
    {
        if (_hero == null)
            return;

        if (_hero.Name == "BAMBUGA")
        {
            _hero.Weapon = (Weapon)AmmunitionFactory.Create("weapon", "bambuga");
            _hero.Name = "Cheater";
        }
    }

    /// <summary>
    /// select_skill method
    /// </summary>
    private void SelectSkill(string skillType)
    {
        if (_hero == null)
            return;

        List<string> skillCodes = SkillsShow.GetSkillCodesForType(skillType);
        int selectedIndex = 0;
        int tableOffset = 3;
        int indicatorX = 2 + tableOffset;
        int logStartY = 4;

        int totalLines = skillCodes.Count * 3;
        int offsetLines = Math.Max(0, (29 - totalLines) / 4);

        void RenderSkillMenu()
        {
            _messages.Log.Clear();

            for (int i = 0; i < offsetLines; i++)
            {
                _messages.Log.Add("");
            }

            for (int i = 0; i < skillCodes.Count; i++)
            {
                string skillCode = skillCodes[i];
                ISkill skill = SkillFactory.Create(skillCode, _hero);
                if (skill == null) continue;

                string indicator = (i == selectedIndex) ? "►" : " ";
                string alignedSkillName = skill.Name.PadRight(20);
                _messages.Log.Add($"{new string(' ', tableOffset)}{indicator}  {alignedSkillName} {skill.DescriptionShort}");
                _messages.Log.Add("");
                _messages.Log.Add("");
            }

            MainRenderer renderer = new("messages_full_screen", entity: _messages);
            renderer.RenderScreen();
        }

        void UpdateIndicator(int oldIndex, int newIndex)
        {
            try
            {
                int oldY = logStartY + offsetLines + (oldIndex * 3);
                Console.SetCursorPosition(indicatorX, oldY);
                Console.Write(" ");

                int newY = logStartY + offsetLines + (newIndex * 3);
                Console.SetCursorPosition(indicatorX, newY);
                Console.Write("►");
            }
            catch { }
        }

        RenderSkillMenu();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.LeftArrow)
            {
                int oldIndex = selectedIndex;
                selectedIndex = (selectedIndex - 1 + skillCodes.Count) % skillCodes.Count;
                UpdateIndicator(oldIndex, selectedIndex);
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.RightArrow)
            {
                int oldIndex = selectedIndex;
                selectedIndex = (selectedIndex + 1) % skillCodes.Count;
                UpdateIndicator(oldIndex, selectedIndex);
            }
        }

        string selectedSkillCode = skillCodes[selectedIndex];

        if (skillType == "active_skill")
            _hero.ActiveSkill = SkillFactory.CreateActive(selectedSkillCode, _hero);
        else if (skillType == "passive_skill")
            _hero.PassiveSkill = SkillFactory.CreatePassive(selectedSkillCode, _hero);
        else if (skillType == "camp_skill")
            _hero.CampSkill = SkillFactory.CreateCamp(selectedSkillCode, _hero);
    }

    [GeneratedRegex(@"[a-zA-Zа-яА-Я]")]
    private static partial Regex LetterValidationRegex();

    #endregion
}
