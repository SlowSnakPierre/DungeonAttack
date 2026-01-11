using DungeonAttack.Infrastructure;
using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;
using DungeonAttack.Renderers;
using Microsoft.EntityFrameworkCore;

namespace DungeonAttack.Editor;

/// <summary>
/// Classe de données pour afficher une liste dans le template YAML
/// </summary>
public class EditorListData
{
    public string Title { get; set; } = "";
    public string PageInfo { get; set; } = "";
    public string Item0 { get; set; } = "";
    public string Item1 { get; set; } = "";
    public string Item2 { get; set; } = "";
    public string Item3 { get; set; } = "";
    public string Item4 { get; set; } = "";
    public string Item5 { get; set; } = "";
    public string Item6 { get; set; } = "";
    public string Item7 { get; set; } = "";
    public string Item8 { get; set; } = "";
}

/// <summary>
/// Classe de données pour afficher un formulaire dans le template YAML
/// </summary>
public class EditorFormData
{
    public string Title { get; set; } = "";
    public string Line0 { get; set; } = "";
    public string Line1 { get; set; } = "";
    public string Line2 { get; set; } = "";
    public string Line3 { get; set; } = "";
    public string Line4 { get; set; } = "";
    public string Line5 { get; set; } = "";
    public string Line6 { get; set; } = "";
    public string Line7 { get; set; } = "";
    public string Line8 { get; set; } = "";
    public string Line9 { get; set; } = "";
    public string Line10 { get; set; } = "";
    public string Line11 { get; set; } = "";
    public string Line12 { get; set; } = "";
    public string Line13 { get; set; } = "";
    public string Line14 { get; set; } = "";
    public string Line15 { get; set; } = "";
    public string Line16 { get; set; } = "";
    public string Line17 { get; set; } = "";
    public string Line18 { get; set; } = "";
}

/// <summary>
/// Classe de données pour afficher l'écran de saisie de nom
/// </summary>
public class EditorInputData
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
}

public class GameEditor
{
    private readonly GameDbContext _db;
    private const int PageSize = 9;
    private const int ScreenWidth = 120;

    public GameEditor()
    {
        _db = new GameDbContext();
    }

    public void Run()
    {
        while (true)
        {
            MainRenderer renderer = new("editor_main_screen");
            string action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "weapons": WeaponsMenu(); break;
                case "armors": ArmorsMenu(); break;
                case "shields": ShieldsMenu(); break;
                case "enemies": EnemiesMenu(); break;
                case "skills": SkillsMenu(); break;
                case "heroes": HeroesMenu(); break;
                case "exit": return;
            }
        }
    }

    #region Generic List Display

    private void DisplayList<T>(string title, IEnumerable<T> items, Func<T, string> formatItem,
        Action addAction, Action<T> editAction) where T : class
    {
        List<T> itemList = [.. items];
        int page = 0;
        int totalPages = Math.Max(1, (int)Math.Ceiling(itemList.Count / (double)PageSize));

        while (true)
        {
            List<T> pageItems = [.. itemList.Skip(page * PageSize).Take(PageSize)];

            EditorListData listData = new()
            {
                Title = title,
                PageInfo = $"Page {page + 1}/{totalPages} | Total: {itemList.Count} items"
            };

            for (int i = 0; i < PageSize; i++)
            {
                string itemText = i < pageItems.Count 
                    ? $"{i + 1}. {formatItem(pageItems[i])}"
                    : "";
                
                typeof(EditorListData).GetProperty($"Item{i}")?.SetValue(listData, itemText);
            }

            MainRenderer renderer = new("editor_list_screen", entity: listData);
            string action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "add":
                    addAction();
                    itemList = [.. items];
                    totalPages = Math.Max(1, (int)Math.Ceiling(itemList.Count / (double)PageSize));
                    break;

                case "prev":
                    page = (page - 1 + totalPages) % totalPages;
                    break;

                case "next":
                    page = (page + 1) % totalPages;
                    break;

                case "back":
                    return;

                default:
                    if (action.StartsWith("select_") && int.TryParse(action[7..], out int itemIndex))
                    {
                        if (itemIndex >= 0 && itemIndex < pageItems.Count)
                        {
                            ShowItemActions(pageItems[itemIndex], editAction);
                            itemList = [.. items];
                            totalPages = Math.Max(1, (int)Math.Ceiling(itemList.Count / (double)PageSize));
                        }
                    }
                    break;
            }
        }
    }

    private void ShowItemActions<T>(T item, Action<T> editAction) where T : class
    {
        MainRenderer renderer = new("editor_item_actions");
        string action = renderer.RenderMenuScreen();

        switch (action)
        {
            case "edit":
                editAction(item);
                break;
            case "delete":
                DeleteItem(item);
                break;
        }
    }

    private void DeleteItem<T>(T item) where T : class
    {
        MainRenderer renderer = new("editor_confirm_delete");
        string action = renderer.RenderMenuScreen();

        if (action == "yes")
        {
            _db.Remove(item);
            _db.SaveChanges();
        }
    }

    #endregion

    #region Input Helpers (Style du jeu avec ► text ◄)

    /// <summary>
    /// Affiche un écran de saisie de nom comme dans l'App (Enter Character Name)
    /// Retourne le nom saisi ou null si annulé (Escape)
    /// </summary>
    private static string? NameInput(string title, string message, int maxLength = 50)
    {
        EditorInputData inputData = new()
        {
            Title = title,
            Message = message
        };

        MainRenderer renderer = new("editor_input_screen", entity: inputData);
        renderer.RenderScreen();

        int inputLine = 29;
        string input = "";

        try { Console.CursorVisible = false; } catch { }

        void DisplayInput(string text)
        {
            try
            {
                Console.SetCursorPosition(2, inputLine);
                Console.Write(new string(' ', ScreenWidth - 4));

                int textWidth = text.Length + 4;
                int startX = (ScreenWidth - textWidth) / 2;

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
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                return null;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input = input[..^1];
                    DisplayInput(input);
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar) && input.Length < maxLength)
            {
                input += keyInfo.KeyChar;
                DisplayInput(input);
            }
        }

        return input.Trim();
    }

    /// <summary>
    /// Affiche un formulaire et permet l'édition de chaque champ
    /// Utilise NameInput pour chaque saisie (style Enter Character Name)
    /// Navigation: flèches haut/bas pour les champs + SAVE/CANCEL, Enter pour éditer/valider
    /// </summary>
    private static bool ShowEditForm(string title, List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields)
    {
        int selectedItem = 0;
        int totalItems = fields.Count + 2;

        while (true)
        {
            EditorFormData formData = new() { Title = title };

            for (int i = 0; i < fields.Count && i < 19; i++)
            {
                string indicator = (i == selectedItem) ? "►" : " ";
                string readonlyMarker = fields[i].isReadonly ? "[RO] " : "";
                string line = $"{indicator} {readonlyMarker}{fields[i].label,-20}: {fields[i].getValue()}";
                typeof(EditorFormData).GetProperty($"Line{i}")?.SetValue(formData, line);
            }

            MainRenderer renderer = new("editor_form_screen", entity: formData);
            renderer.RenderScreen();

            bool saveSelected = (selectedItem == fields.Count);
            bool cancelSelected = (selectedItem == fields.Count + 1);
            
            try
            {
                Console.SetCursorPosition(31, 31);
                Console.Write(saveSelected ? "►" : " ");
                
                Console.SetCursorPosition(80, 31);
                Console.Write(cancelSelected ? "►" : " ");
            }
            catch { }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.S || keyInfo.KeyChar == 's')
            {
                return true;
            }
            else if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.D0 || keyInfo.KeyChar == '0')
            {
                return false;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedItem = (selectedItem - 1 + totalItems) % totalItems;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedItem = (selectedItem + 1) % totalItems;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && selectedItem >= fields.Count)
            {
                selectedItem = fields.Count;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && selectedItem >= fields.Count)
            {
                selectedItem = fields.Count + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (selectedItem == fields.Count)
                {
                    return true;
                }
                else if (selectedItem == fields.Count + 1)
                {
                    return false;
                }
                else if (!fields[selectedItem].isReadonly)
                {
                    string? newValue = NameInput(
                        $"Edit: {fields[selectedItem].label}",
                        $"Current value: {fields[selectedItem].getValue()}",
                        100
                    );
                    
                    if (newValue != null)
                    {
                        fields[selectedItem].setValue(newValue);
                    }
                }
            }
        }
    }

    #endregion

    #region Weapons

    private void WeaponsMenu()
    {
        DisplayList(
            "WEAPONS",
            _db.Weapons.OrderBy(w => w.Code),
            w => $"{w.Code,-20} {w.Name,-20} DMG:{w.MinDmg}-{w.MaxDmg}",
            AddWeapon,
            EditWeapon
        );
    }

    private void AddWeapon()
    {
        string? code = NameInput("ADD WEAPON", "Enter the weapon code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Weapons.Any(w => w.Code == code))
            return;

        WeaponData weapon = new() { Code = code };
        if (EditWeaponForm(weapon, isNew: true))
        {
            _db.Weapons.Add(weapon);
            _db.SaveChanges();
        }
    }

    private void EditWeapon(WeaponData weapon)
    {
        if (EditWeaponForm(weapon, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditWeaponForm(WeaponData weapon, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Code", () => weapon.Code, v => { }, true),
            ("Name", () => weapon.Name, v => weapon.Name = v, false),
            ("Price", () => weapon.Price.ToString(), v => weapon.Price = int.TryParse(v, out int i) ? i : weapon.Price, false),
            ("Min Damage", () => weapon.MinDmg.ToString(), v => weapon.MinDmg = int.TryParse(v, out int i) ? i : weapon.MinDmg, false),
            ("Max Damage", () => weapon.MaxDmg.ToString(), v => weapon.MaxDmg = int.TryParse(v, out int i) ? i : weapon.MaxDmg, false),
            ("Accuracy", () => weapon.Accuracy.ToString(), v => weapon.Accuracy = int.TryParse(v, out int i) ? i : weapon.Accuracy, false),
            ("Armor", () => weapon.Armor.ToString(), v => weapon.Armor = int.TryParse(v, out int i) ? i : weapon.Armor, false),
            ("Mp", () => weapon.Mp.ToString(), v => weapon.Mp = int.TryParse(v, out int i) ? i : weapon.Mp, false),
            ("Block Chance", () => weapon.BlockChance.ToString(), v => weapon.BlockChance = int.TryParse(v, out int i) ? i : weapon.BlockChance, false),
            ("Armor Penetration", () => weapon.ArmorPenetration.ToString(), v => weapon.ArmorPenetration = int.TryParse(v, out int i) ? i : weapon.ArmorPenetration, false),
            ("Enhance Min Dmg", () => weapon.EnhanceMinDmg.ToString(), v => weapon.EnhanceMinDmg = int.TryParse(v, out int i) ? i : weapon.EnhanceMinDmg, false),
            ("Enhance Max Dmg", () => weapon.EnhanceMaxDmg.ToString(), v => weapon.EnhanceMaxDmg = int.TryParse(v, out int i) ? i : weapon.EnhanceMaxDmg, false),
            ("Enhance Accuracy", () => weapon.EnhanceAccuracy.ToString(), v => weapon.EnhanceAccuracy = int.TryParse(v, out int i) ? i : weapon.EnhanceAccuracy, false),
            ("Enhance Armor", () => weapon.EnhanceArmor.ToString(), v => weapon.EnhanceArmor = int.TryParse(v, out int i) ? i : weapon.EnhanceArmor, false),
            ("Enhance Mp", () => weapon.EnhanceMp.ToString(), v => weapon.EnhanceMp = int.TryParse(v, out int i) ? i : weapon.EnhanceMp, false),
            ("Enhance Block", () => weapon.EnhanceBlockChance.ToString(), v => weapon.EnhanceBlockChance = int.TryParse(v, out int i) ? i : weapon.EnhanceBlockChance, false),
            ("Enhance Armor Pen", () => weapon.EnhanceArmorPenetration.ToString(), v => weapon.EnhanceArmorPenetration = int.TryParse(v, out int i) ? i : weapon.EnhanceArmorPenetration, false)
        ];

        string formTitle = isNew ? "ADD WEAPON" : $"EDIT WEAPON: {weapon.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion

    #region Armors

    private void ArmorsMenu()
    {
        DisplayList(
            "ARMORS (Body, Head, Arms)",
            _db.Armors.OrderBy(a => a.AmmunitionType).ThenBy(a => a.Code),
            a => $"[{a.AmmunitionType,-10}] {a.Code,-15} {a.Name,-15} ARM:{a.Armor}",
            AddArmor,
            EditArmor
        );
    }

    private void AddArmor()
    {
        string? code = NameInput("ADD ARMOR", "Enter the armor code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Armors.Any(a => a.Code == code))
            return;

        ArmorData armor = new() { Code = code, AmmunitionType = "body_armor" };
        if (EditArmorForm(armor, isNew: true))
        {
            _db.Armors.Add(armor);
            _db.SaveChanges();
        }
    }

    private void EditArmor(ArmorData armor)
    {
        if (EditArmorForm(armor, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditArmorForm(ArmorData armor, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Type (body/head/arms)", () => armor.AmmunitionType, v => armor.AmmunitionType = v.Contains("head") ? "head_armor" : v.Contains("arms") ? "arms_armor" : "body_armor", !isNew),
            ("Code", () => armor.Code, v => { }, true),
            ("Name", () => armor.Name, v => armor.Name = v, false),
            ("Price", () => armor.Price.ToString(), v => armor.Price = int.TryParse(v, out int i) ? i : armor.Price, false),
            ("Armor", () => armor.Armor.ToString(), v => armor.Armor = int.TryParse(v, out int i) ? i : armor.Armor, false),
            ("Accuracy", () => armor.Accuracy.ToString(), v => armor.Accuracy = int.TryParse(v, out int i) ? i : armor.Accuracy, false),
            ("Mp", () => armor.Mp.ToString(), v => armor.Mp = int.TryParse(v, out int i) ? i : armor.Mp, false),
            ("Block Chance", () => armor.BlockChance.ToString(), v => armor.BlockChance = int.TryParse(v, out int i) ? i : armor.BlockChance, false),
            ("Armor Penetration", () => armor.ArmorPenetration.ToString(), v => armor.ArmorPenetration = int.TryParse(v, out int i) ? i : armor.ArmorPenetration, false),
            ("Enhance Armor", () => armor.EnhanceArmor.ToString(), v => armor.EnhanceArmor = int.TryParse(v, out int i) ? i : armor.EnhanceArmor, false),
            ("Enhance Accuracy", () => armor.EnhanceAccuracy.ToString(), v => armor.EnhanceAccuracy = int.TryParse(v, out int i) ? i : armor.EnhanceAccuracy, false),
            ("Enhance Mp", () => armor.EnhanceMp.ToString(), v => armor.EnhanceMp = int.TryParse(v, out int i) ? i : armor.EnhanceMp, false),
            ("Enhance Block", () => armor.EnhanceBlockChance.ToString(), v => armor.EnhanceBlockChance = int.TryParse(v, out int i) ? i : armor.EnhanceBlockChance, false),
            ("Enhance Armor Pen", () => armor.EnhanceArmorPenetration.ToString(), v => armor.EnhanceArmorPenetration = int.TryParse(v, out int i) ? i : armor.EnhanceArmorPenetration, false)
        ];

        string formTitle = isNew ? "ADD ARMOR" : $"EDIT ARMOR: {armor.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion

    #region Shields

    private void ShieldsMenu()
    {
        DisplayList(
            "SHIELDS",
            _db.Shields.OrderBy(s => s.Code),
            s => $"{s.Code,-20} {s.Name,-20} BLK:{s.BlockChance}%",
            AddShield,
            EditShield
        );
    }

    private void AddShield()
    {
        string? code = NameInput("ADD SHIELD", "Enter the shield code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Shields.Any(s => s.Code == code))
            return;

        ShieldData shield = new() { Code = code };
        if (EditShieldForm(shield, isNew: true))
        {
            _db.Shields.Add(shield);
            _db.SaveChanges();
        }
    }

    private void EditShield(ShieldData shield)
    {
        if (EditShieldForm(shield, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditShieldForm(ShieldData shield, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Code", () => shield.Code, v => { }, true),
            ("Name", () => shield.Name, v => shield.Name = v, false),
            ("Price", () => shield.Price.ToString(), v => shield.Price = int.TryParse(v, out int i) ? i : shield.Price, false),
            ("Block Chance", () => shield.BlockChance.ToString(), v => shield.BlockChance = int.TryParse(v, out int i) ? i : shield.BlockChance, false),
            ("Armor", () => shield.Armor.ToString(), v => shield.Armor = int.TryParse(v, out int i) ? i : shield.Armor, false),
            ("Min Damage", () => shield.MinDmg.ToString(), v => shield.MinDmg = int.TryParse(v, out int i) ? i : shield.MinDmg, false),
            ("Max Damage", () => shield.MaxDmg.ToString(), v => shield.MaxDmg = int.TryParse(v, out int i) ? i : shield.MaxDmg, false),
            ("Accuracy", () => shield.Accuracy.ToString(), v => shield.Accuracy = int.TryParse(v, out int i) ? i : shield.Accuracy, false),
            ("Mp", () => shield.Mp.ToString(), v => shield.Mp = int.TryParse(v, out int i) ? i : shield.Mp, false),
            ("Armor Penetration", () => shield.ArmorPenetration.ToString(), v => shield.ArmorPenetration = int.TryParse(v, out int i) ? i : shield.ArmorPenetration, false),
            ("Enhance Block", () => shield.EnhanceBlockChance.ToString(), v => shield.EnhanceBlockChance = int.TryParse(v, out int i) ? i : shield.EnhanceBlockChance, false),
            ("Enhance Armor", () => shield.EnhanceArmor.ToString(), v => shield.EnhanceArmor = int.TryParse(v, out int i) ? i : shield.EnhanceArmor, false),
            ("Enhance Min Dmg", () => shield.EnhanceMinDmg.ToString(), v => shield.EnhanceMinDmg = int.TryParse(v, out int i) ? i : shield.EnhanceMinDmg, false),
            ("Enhance Max Dmg", () => shield.EnhanceMaxDmg.ToString(), v => shield.EnhanceMaxDmg = int.TryParse(v, out int i) ? i : shield.EnhanceMaxDmg, false),
            ("Enhance Accuracy", () => shield.EnhanceAccuracy.ToString(), v => shield.EnhanceAccuracy = int.TryParse(v, out int i) ? i : shield.EnhanceAccuracy, false),
            ("Enhance Mp", () => shield.EnhanceMp.ToString(), v => shield.EnhanceMp = int.TryParse(v, out int i) ? i : shield.EnhanceMp, false),
            ("Enhance Armor Pen", () => shield.EnhanceArmorPenetration.ToString(), v => shield.EnhanceArmorPenetration = int.TryParse(v, out int i) ? i : shield.EnhanceArmorPenetration, false)
        ];

        string formTitle = isNew ? "ADD SHIELD" : $"EDIT SHIELD: {shield.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion

    #region Enemies

    private void EnemiesMenu()
    {
        DisplayList(
            "ENEMIES",
            _db.Enemies.OrderBy(e => e.DungeonName).ThenBy(e => e.N),
            e => $"[{e.DungeonName,-12}] {e.Code,-15} {e.Name,-15} HP:{e.Hp}",
            AddEnemy,
            EditEnemy
        );
    }

    private void AddEnemy()
    {
        string? code = NameInput("ADD ENEMY", "Enter the enemy code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Enemies.Any(e => e.Code == code))
            return;

        EnemyData enemy = new() { Code = code };
        if (EditEnemyForm(enemy, isNew: true))
        {
            _db.Enemies.Add(enemy);
            _db.SaveChanges();
        }
    }

    private void EditEnemy(EnemyData enemy)
    {
        if (EditEnemyForm(enemy, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditEnemyForm(EnemyData enemy, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Dungeon Name", () => enemy.DungeonName, v => enemy.DungeonName = v, !isNew),
            ("Code", () => enemy.Code, v => { }, true),
            ("Code Name", () => enemy.CodeName, v => enemy.CodeName = v, false),
            ("Name", () => enemy.Name, v => enemy.Name = v, false),
            ("Order (N)", () => enemy.N.ToString(), v => enemy.N = int.TryParse(v, out int i) ? i : enemy.N, false),
            ("HP", () => enemy.Hp.ToString(), v => enemy.Hp = int.TryParse(v, out int i) ? i : enemy.Hp, false),
            ("MP", () => enemy.Mp.ToString(), v => enemy.Mp = int.TryParse(v, out int i) ? i : enemy.Mp, false),
            ("Min Damage", () => enemy.MinDmg.ToString(), v => enemy.MinDmg = int.TryParse(v, out int i) ? i : enemy.MinDmg, false),
            ("Max Damage", () => enemy.MaxDmg.ToString(), v => enemy.MaxDmg = int.TryParse(v, out int i) ? i : enemy.MaxDmg, false),
            ("Accuracy", () => enemy.Accuracy.ToString(), v => enemy.Accuracy = int.TryParse(v, out int i) ? i : enemy.Accuracy, false),
            ("Armor", () => enemy.Armor.ToString(), v => enemy.Armor = int.TryParse(v, out int i) ? i : enemy.Armor, false),
            ("Block Chance", () => enemy.BlockChance.ToString(), v => enemy.BlockChance = int.TryParse(v, out int i) ? i : enemy.BlockChance, false),
            ("Armor Penetration", () => enemy.ArmorPenetration.ToString(), v => enemy.ArmorPenetration = int.TryParse(v, out int i) ? i : enemy.ArmorPenetration, false),
            ("Regen HP", () => enemy.RegenHp.ToString(), v => enemy.RegenHp = int.TryParse(v, out int i) ? i : enemy.RegenHp, false),
            ("Regen MP", () => enemy.RegenMp.ToString(), v => enemy.RegenMp = int.TryParse(v, out int i) ? i : enemy.RegenMp, false),
            ("Experience", () => enemy.Exp.ToString(), v => enemy.Exp = int.TryParse(v, out int i) ? i : enemy.Exp, false),
            ("Coins", () => enemy.Coins.ToString(), v => enemy.Coins = int.TryParse(v, out int i) ? i : enemy.Coins, false)
        ];

        string formTitle = isNew ? "ADD ENEMY" : $"EDIT ENEMY: {enemy.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion

    #region Skills

    private void SkillsMenu()
    {
        DisplayList(
            "SKILLS",
            _db.Skills.OrderBy(s => s.SkillType).ThenBy(s => s.Code),
            s => $"[{s.SkillType,-8}] {s.Code,-20} {s.Name,-20}",
            AddSkill,
            EditSkill
        );
    }

    private void AddSkill()
    {
        string? code = NameInput("ADD SKILL", "Enter the skill code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Skills.Any(s => s.Code == code))
            return;

        SkillData skill = new() { Code = code, SkillType = "active" };
        if (EditSkillForm(skill, isNew: true))
        {
            _db.Skills.Add(skill);
            _db.SaveChanges();
        }
    }

    private void EditSkill(SkillData skill)
    {
        if (EditSkillForm(skill, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditSkillForm(SkillData skill, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Type (active/passive/camp)", () => skill.SkillType, v => skill.SkillType = v.Contains("passive") ? "passive" : v.Contains("camp") ? "camp" : "active", !isNew),
            ("Code", () => skill.Code, v => { }, true),
            ("Name", () => skill.Name, v => skill.Name = v, false),
            ("Description", () => skill.Description, v => skill.Description = v, false),
            ("MP Cost", () => skill.MpCost.ToString(), v => skill.MpCost = int.TryParse(v, out int i) ? i : skill.MpCost, false),
            ("HP Cost", () => skill.HpCost.ToString(), v => skill.HpCost = int.TryParse(v, out int i) ? i : skill.HpCost, false),
            ("Damage Mod", () => skill.DamageMod.ToString("F2"), v => skill.DamageMod = double.TryParse(v, out double d) ? d : skill.DamageMod, false),
            ("Accuracy Mod", () => skill.AccuracyMod.ToString("F2"), v => skill.AccuracyMod = double.TryParse(v, out double d) ? d : skill.AccuracyMod, false),
            ("Level Mod", () => skill.LvlMod.ToString("F2"), v => skill.LvlMod = double.TryParse(v, out double d) ? d : skill.LvlMod, false),
            ("Restore Effect", () => skill.RestoreEffect.ToString("F2"), v => skill.RestoreEffect = double.TryParse(v, out double d) ? d : skill.RestoreEffect, false),
            ("Basic Mod", () => skill.BasicMod.ToString(), v => skill.BasicMod = int.TryParse(v, out int i) ? i : skill.BasicMod, false)
        ];

        string formTitle = isNew ? "ADD SKILL" : $"EDIT SKILL: {skill.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion

    #region Heroes

    private void HeroesMenu()
    {
        DisplayList(
            "HEROES",
            _db.Heroes.OrderBy(h => h.N),
            h => $"{h.Code,-20} {h.Name,-20} HP:{h.Hp} DMG:{h.MinDmg}-{h.MaxDmg}",
            AddHero,
            EditHero
        );
    }

    private void AddHero()
    {
        string? code = NameInput("ADD HERO", "Enter the hero code (unique identifier)");
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (_db.Heroes.Any(h => h.Code == code))
            return;

        HeroData hero = new() { Code = code };
        if (EditHeroForm(hero, isNew: true))
        {
            _db.Heroes.Add(hero);
            _db.SaveChanges();
        }
    }

    private void EditHero(HeroData hero)
    {
        if (EditHeroForm(hero, isNew: false))
        {
            _db.SaveChanges();
        }
    }

    private static bool EditHeroForm(HeroData hero, bool isNew)
    {
        List<(string label, Func<string> getValue, Action<string> setValue, bool isReadonly)> fields =
        [
            ("Code", () => hero.Code, v => { }, true),
            ("Name", () => hero.Name, v => hero.Name = v, false),
            ("Order (N)", () => hero.N.ToString(), v => hero.N = int.TryParse(v, out int i) ? i : hero.N, false),
            ("HP", () => hero.Hp.ToString(), v => hero.Hp = int.TryParse(v, out int i) ? i : hero.Hp, false),
            ("MP", () => hero.Mp.ToString(), v => hero.Mp = int.TryParse(v, out int i) ? i : hero.Mp, false),
            ("Min Damage", () => hero.MinDmg.ToString(), v => hero.MinDmg = int.TryParse(v, out int i) ? i : hero.MinDmg, false),
            ("Max Damage", () => hero.MaxDmg.ToString(), v => hero.MaxDmg = int.TryParse(v, out int i) ? i : hero.MaxDmg, false),
            ("Accuracy", () => hero.Accuracy.ToString(), v => hero.Accuracy = int.TryParse(v, out int i) ? i : hero.Accuracy, false),
            ("Armor", () => hero.Armor.ToString(), v => hero.Armor = int.TryParse(v, out int i) ? i : hero.Armor, false),
            ("Armor Penetration", () => hero.ArmorPenetration.ToString(), v => hero.ArmorPenetration = int.TryParse(v, out int i) ? i : hero.ArmorPenetration, false),
            ("Skill Points", () => hero.SkillPoints.ToString(), v => hero.SkillPoints = int.TryParse(v, out int i) ? i : hero.SkillPoints, false),
            ("Weapon", () => hero.Weapon, v => hero.Weapon = v, false),
            ("Body Armor", () => hero.BodyArmor, v => hero.BodyArmor = v, false),
            ("Head Armor", () => hero.HeadArmor, v => hero.HeadArmor = v, false),
            ("Arms Armor", () => hero.ArmsArmor, v => hero.ArmsArmor = v, false),
            ("Shield", () => hero.Shield, v => hero.Shield = v, false)
        ];

        string formTitle = isNew ? "ADD HERO" : $"EDIT HERO: {hero.Code}";
        return ShowEditForm(formTitle, fields);
    }

    #endregion
}
