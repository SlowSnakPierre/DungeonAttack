namespace DungeonAttack.Infrastructure;

public static class ConsoleHelper
{
    /// <summary>
    /// Configure la taille de la fenêtre console pour s'adapter au contenu ASCII art
    /// Taille par défaut au démarrage: 124x36
    /// </summary>
    public static void ConfigureWindowSize()
    {
        ConfigureWindowSize(124, 36);
    }

    /// <summary>
    /// Configure la taille de la fenêtre console dynamiquement selon le contenu
    /// </summary>
    /// <param name="width">Largeur nécessaire en caractères</param>
    /// <param name="height">Hauteur nécessaire en lignes</param>
    public static void ConfigureWindowSize(int width, int height)
    {
        try
        {
            if (!OperatingSystem.IsWindows())
                return;

            int maxWidth = Console.LargestWindowWidth;
            int maxHeight = Console.LargestWindowHeight;

            int targetWidth = Math.Min(width, maxWidth);
            int targetHeight = Math.Min(height, maxHeight);

            if (Console.WindowWidth == targetWidth && Console.WindowHeight == targetHeight)
                return;


            bool shrinkingWidth = targetWidth < Console.WindowWidth;
            bool shrinkingHeight = targetHeight < Console.WindowHeight;

            if (shrinkingWidth || shrinkingHeight)
            {
                Console.SetWindowSize(targetWidth, targetHeight);
                Console.SetBufferSize(targetWidth, targetHeight);
            }
            else
            {
                Console.SetBufferSize(targetWidth, targetHeight);
                Console.SetWindowSize(targetWidth, targetHeight);
            }

           
            ConsoleWindowManager.DisableConsoleFeatures();
        }
        catch
        {
        }
    }

    /// <summary>
    /// Calcule la taille nécessaire pour afficher une vue
    /// </summary>
    /// <param name="view">Liste de lignes à afficher</param>
    /// <returns>Tuple (width, height) nécessaire</returns>
    public static (int width, int height) CalculateViewSize(List<string> view)
    {
        if (view == null || view.Count == 0)
            return (124, 36);

        int lastNonEmptyLine = view.Count - 1;
        while (lastNonEmptyLine >= 0 && string.IsNullOrWhiteSpace(view[lastNonEmptyLine]))
        {
            lastNonEmptyLine--;
        }

        if (lastNonEmptyLine < 0)
            return (124, 36);

        int maxWidth = 0;
        for (int i = 0; i <= lastNonEmptyLine; i++)
        {
            if (view[i] != null)
            {
                int lineWidth = view[i].TrimEnd().Length;
                if (lineWidth > maxWidth)
                    maxWidth = lineWidth;
            }
        }

        int height = lastNonEmptyLine + 1;

        return (maxWidth, height);
    }

    /// <summary>
    /// Efface l'écran du terminal en utilisant le code ANSI spécifié ou Console.Clear() natif
    /// </summary>
    public static void ClearScreen(string clearType)
    {
        try
        {
            if (clearType == "console_clear")
            {
                Console.Clear();
            }
            else if (clearType == "\x1bc" || clearType == "ansi_reset")
            {
                Console.Write("\x1bc");
            }
            else if (clearType == "\u001b[H\u001b[2J")
            {
                Console.Write("\u001b[H\u001b[2J");
            }
            else if (clearType == "\u001b[2J")
            {
                Console.Write("\u001b[2J");
            }
            else
            {
                Console.Clear();
            }
        }
        catch
        {
            // Si Console.Clear() échoue, utiliser ANSI comme fallback
            try
            {
                Console.Write("\u001b[H\u001b[2J");
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Alias pour ClearScreen (pour compatibilité)
    /// </summary>
    public static void Clear(string clearType) => ClearScreen(clearType);

    /// <summary>
    /// Centre une chaîne dans une largeur donnée
    /// </summary>
    public static string Center(this string text, int width)
    {
        if (text.Length >= width)
        {
            return text;
        }

        int totalPadding = width - text.Length;
        int padLeft = totalPadding / 2;
        int padRight = totalPadding - padLeft;

        return new string(' ', padLeft) + text + new string(' ', padRight);
    }

    /// <summary>
    /// Aligne une chaîne selon le modificateur spécifié
    /// </summary>
    public static string Align(this string text, int width, char modifier)
    {
        return modifier switch
        {
            'm' => Center(text, width),            // middle (center)
            's' => text.PadRight(width),           // start (left)
            'e' => text.PadLeft(width),            // end (right)
            _ => text
        };
    }

    /// <summary>
    /// Obtient une entrée utilisateur avec un prompt
    /// </summary>
    public static string GetInput(string prompt = "")
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            Console.Write(prompt);
        }

        return Console.ReadLine() ?? string.Empty;
    }

    /// <summary>
    /// Obtient un choix numérique de l'utilisateur
    /// </summary>
    public static int GetChoice(string prompt = "Votre choix: ")
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice))
            {
                return choice;
            }

            Console.WriteLine("Veuillez entrer un nombre valide.");
        }
    }

    /// <summary>
    /// Attend que l'utilisateur appuie sur une touche
    /// </summary>
    public static void WaitForKey(string message = "Appuyez sur une touche pour continuer...")
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }

    /// <summary>
    /// Affiche un texte avec une couleur spécifique
    /// </summary>
    public static void WriteColored(string text, ConsoleColor color)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = oldColor;
    }

    /// <summary>
    /// Affiche une ligne de texte avec une couleur spécifique
    /// </summary>
    public static void WriteLineColored(string text, ConsoleColor color)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = oldColor;
    }
}
