using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;
using DungeonAttack.Renderers;

namespace DungeonAttack.Controllers.Characters;

/// <summary>
/// Contrôleur pour l'utilisation des camp skills
/// </summary>
public static class HeroUseSkill
{
    /// <summary>
    /// Utilise le camp skill du héros
    /// </summary>
    public static void CampSkill(Hero hero, MainMessage messages)
    {
        if (hero.CampSkill == null)
            return;

        if (hero.CampSkill.Code == "first_aid" && hero.HpMax - hero.Hp > 0)
        {
            UseFirstAid(hero, messages);
        }
        else if (hero.CampSkill.Code == "bloody_ritual" && hero.MpMax - hero.Mp > 0)
        {
            UseBloodyRitual(hero, messages);
        }
        else
        {
            messages.Main = "BACK TO CAMP FIRE OPTIONS  [Enter 0]";
            messages.Log.Add($"You dont need use {hero.CampSkill.Name}");
            Display(hero, messages);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Utilise First Aid (restaure HP pour MP)
    /// </summary>
    private static void UseFirstAid(Hero hero, MainMessage messages)
    {
        if (hero.CampSkill is not FirstAid firstAid) return;

        string? choice = null;
        while (choice != "" && choice != "0" && choice != "N")
        {
            if (hero.Mp >= FirstAid.MpCost && hero.HpMax > hero.Hp)
            {
                messages.Main = $"USE \"{hero.CampSkill.Name.ToUpper()}\"  [Enter Y]        BACK TO CAMP FIRE OPTIONS  [Enter N]";
                messages.Log.Add($"Use {hero.CampSkill.Name}, to restore {firstAid.RestoreEffect} HP for {FirstAid.MpCost} MP?");
            }
            else
            {
                messages.Main = "BACK TO CAMP FIRE OPTIONS  [Enter 0]";
                if (hero.HpMax > hero.Hp)
                    messages.Log.Add($"Not enough MP for next use of \"{hero.CampSkill.Name}\"");
                else
                    messages.Log.Add($"You dont need use {hero.CampSkill.Name}");
            }

            Display(hero, messages);
            choice = Console.ReadLine()?.Trim().ToUpper();

            if (messages.Log.Count > 0)
                messages.Log.RemoveAt(messages.Log.Count - 1);

            if (choice == "Y" && hero.Mp >= FirstAid.MpCost && hero.HpMax > hero.Hp)
            {
                int effectMessage = firstAid.RestoreEffect;
                hero.Hp += firstAid.RestoreEffect;
                hero.Mp -= FirstAid.MpCost;

                if (hero.Hp > hero.HpMax)
                    hero.Hp = hero.HpMax;

                messages.Log.Add($"You restored {effectMessage} HP for {FirstAid.MpCost} MP, now you have {hero.Hp}/{hero.HpMax} HP and {hero.Mp}/{hero.MpMax} MP");
            }

            while (messages.Log.Count > 5)
                messages.Log.RemoveAt(0);
        }
    }

    /// <summary>
    /// Utilise Bloody Ritual (restaure MP pour HP)
    /// </summary>
    private static void UseBloodyRitual(Hero hero, MainMessage messages)
    {
        if (hero.CampSkill is not BloodyRitual bloodyRitual) return;

        string? choice = null;
        while (choice != "" && choice != "0" && choice != "N")
        {
            if (hero.Hp > BloodyRitual.HpCost && hero.MpMax > hero.Mp)
            {
                messages.Main = $"USE \"{hero.CampSkill.Name.ToUpper()}\"  [Enter Y]        BACK TO CAMP FIRE OPTIONS  [Enter N]";
                messages.Log.Add($"Use {hero.CampSkill.Name}, to restore {bloodyRitual.RestoreEffect} MP for {BloodyRitual.HpCost} HP?");
            }
            else
            {
                messages.Main = "BACK TO CAMP FIRE OPTIONS  [Enter 0]";
                if (hero.MpMax > hero.Mp)
                    messages.Log.Add($"Not enough HP for next use of \"{hero.CampSkill.Name}\"");
                else
                    messages.Log.Add($"You dont need use {hero.CampSkill.Name}");
            }

            Display(hero, messages);
            choice = Console.ReadLine()?.Trim().ToUpper();

            if (messages.Log.Count > 0)
                messages.Log.RemoveAt(messages.Log.Count - 1);

            if (choice == "Y" && hero.Hp > BloodyRitual.HpCost && hero.MpMax > hero.Mp)
            {
                int effectMessage = bloodyRitual.RestoreEffect;
                hero.Mp += bloodyRitual.RestoreEffect;
                hero.Hp -= BloodyRitual.HpCost;

                if (hero.Mp > hero.MpMax)
                    hero.Mp = hero.MpMax;

                messages.Log.Add($"You restored {effectMessage} MP for {BloodyRitual.HpCost} HP, now you have {hero.Mp}/{hero.MpMax} MP and {hero.Hp}/{hero.HpMax} HP");
            }

            while (messages.Log.Count > 5)
                messages.Log.RemoveAt(0);
        }
    }

    /// <summary>
    /// Affiche l'écran du camp skill
    /// </summary>
    private static void Display(Hero hero, MainMessage messages)
    {
        MainRenderer renderer = new("camp_skill_screen",
            characters: [hero],
            entity: messages);

        if (hero.CampSkill != null)
            renderer.AddArt("normal", hero.CampSkill.Code);

        renderer.RenderScreen();
    }
}
