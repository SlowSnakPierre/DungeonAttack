using DungeonAttack.Infrastructure;

namespace DungeonAttack.Renderers;

/// <summary>
/// Renderer principal qui orchestre l'affichage des menus avec arts ASCII et partials
/// </summary>
public class MainRenderer(string menuName, List<object?>? characters = null, object? entity = null)
{
    private readonly string _menuName = menuName;
    private readonly List<object?> _characters = characters ?? [];
    private readonly object? _entity = entity;
    private readonly List<(string artType, object artObj)> _artsToAdd = [];
    private MenuTemplate? _template;
    private MenuSelector? _menuSelector;

    /// <summary>
    /// Ajoute un art à afficher
    /// </summary>
    public void AddArt(string artType, object artObj)
    {
        _artsToAdd.Add((artType, artObj));
    }

    /// <summary>
    /// Affiche le menu et gère la sélection avec arrow selector si configuré
    /// </summary>
    public string RenderMenuScreen()
    {
        string templatePath = $"Views/menus/{_menuName}.yml";
        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Menu template not found: {templatePath}");
            return "";
        }

        _template = YamlLoader.Load<MenuTemplate>(templatePath);

        if (_template.Menu != null && _template.Menu.Count > 0 && _template.Menu[0].ShowArrow)
        {
            MenuConfig menuConfig = _template.Menu[0];
            List<MenuOption> options = [.. menuConfig.Buttons.Select(b => new MenuOption(b.Value.Key, b.Key))];

            RenderScreenWithArrowOptimized(menuConfig, true);

            MenuOrientation orientation = menuConfig.Orientation?.ToLower() == "horizontal"
                ? MenuOrientation.Horizontal
                : MenuOrientation.Vertical;

            _menuSelector = new MenuSelector(options, () => RenderScreenWithArrowOptimized(menuConfig, false), orientation, menuConfig.ItemsCount);

            string selectedKey = _menuSelector.Select();

            MenuButton? selectedButton = menuConfig.Buttons.Values.FirstOrDefault(b => b.Key == selectedKey);
            return selectedButton?.Action ?? "";
        }
        else
        {
            RenderScreen();
            return "";
        }
    }

    private void RenderScreenWithArrowOptimized(MenuConfig menuConfig, bool initialRender)
    {
        if (_template == null)
            return;

        if (initialRender)
        {
            List<string> view = [.. _template.View];

            if (_template.Partials != null && _characters.Count > 0)
            {
                InsertPartials(view, _template.Partials);
            }

            if (_template.Arts != null)
            {
                InsertArts(view, _template.Arts);
            }

            MenuRenderer renderer = new(view, _entity, _template.InsertOptions);
            renderer.Render();

            if (_menuSelector != null)
            {
                foreach (KeyValuePair<string, MenuButton> button in menuConfig.Buttons)
                {
                    int y = button.Value.Y;
                    int x = button.Value.X;

                    if (y >= 0 && y < renderer.View.Count)
                    {
                        string arrow = _menuSelector.GetIndicatorForValue(button.Value.Key);

                        char[] lineChars = renderer.View[y].ToCharArray();
                        if (x - 2 >= 0 && x - 2 < lineChars.Length)
                        {
                            lineChars[x - 2] = arrow[0];
                        }
                        renderer.View[y] = new string(lineChars);
                    }
                }
            }

            string clearType = Models.Options.Options.Load().ScreenReplacementType;
            ConsoleHelper.Clear(clearType);

            (int width, int height) = ConsoleHelper.CalculateViewSize(renderer.View);
            ConsoleHelper.ConfigureWindowSize(width, height);

            try
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
            }
            catch
            {
            }

            int lastNonEmptyLine = renderer.View.Count - 1;
            while (lastNonEmptyLine >= 0 && string.IsNullOrWhiteSpace(renderer.View[lastNonEmptyLine]))
            {
                lastNonEmptyLine--;
            }

            for (int i = 0; i <= lastNonEmptyLine; i++)
            {
                if (i < lastNonEmptyLine)
                {
                    Console.WriteLine(renderer.View[i]);
                }
                else
                {
                    Console.Write(renderer.View[i]);
                }
            }
        }
        else
        {
            if (_menuSelector != null)
            {
                try
                {
                    foreach (KeyValuePair<string, MenuButton> button in menuConfig.Buttons)
                    {
                        int y = button.Value.Y;
                        int x = button.Value.X;

                        if (x - 2 >= 0)
                        {
                            string arrow = _menuSelector.GetIndicatorForValue(button.Value.Key);

                            Console.SetCursorPosition(x - 2, y);
                            Console.Write(arrow[0]);
                        }
                    }
                }
                catch
                {
                    RenderScreenWithArrowOptimized(menuConfig, true);
                }
            }
        }
    }

    public void RenderScreen()
    {
        string templatePath = $"Views/menus/{_menuName}.yml";
        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Menu template not found: {templatePath}");
            return;
        }

        MenuTemplate template = YamlLoader.Load<MenuTemplate>(templatePath);
        List<string> view = [.. template.View];

        if (template.Partials != null && _characters.Count > 0)
        {
            InsertPartials(view, template.Partials);
        }

        if (template.Arts != null)
        {
            InsertArts(view, template.Arts);
        }

        MenuRenderer renderer = new(view, _entity, template.InsertOptions);
        renderer.Render();

        string clearType = Models.Options.Options.Load().ScreenReplacementType;
        ConsoleHelper.Clear(clearType);

        (int width, int height) = ConsoleHelper.CalculateViewSize(renderer.View);
        ConsoleHelper.ConfigureWindowSize(width, height);

        try
        {
            Console.SetCursorPosition(0, 0);
        }
        catch
        {
        }

        int lastNonEmptyLine = renderer.View.Count - 1;
        while (lastNonEmptyLine >= 0 && string.IsNullOrWhiteSpace(renderer.View[lastNonEmptyLine]))
        {
            lastNonEmptyLine--;
        }

        // Afficher seulement jusqu'à la dernière ligne non-vide
        // IMPORTANT: Ne pas faire de WriteLine() sur la dernière ligne pour éviter le scroll
        for (int i = 0; i <= lastNonEmptyLine; i++)
        {
            if (i < lastNonEmptyLine)
            {
                Console.WriteLine(renderer.View[i]);
            }
            else
            {
                Console.Write(renderer.View[i]);
            }
        }
    }

    private void InsertPartials(List<string> view, List<PartialConfig> partials)
    {
        for (int i = 0; i < partials.Count && i < _characters.Count; i++)
        {
            PartialConfig partial = partials[i];
            object? character = _characters[i];

            if (character == null)
                continue;

            string partialPath = $"Views/menus/{partial.PartialName}.yml";
            if (!File.Exists(partialPath))
            {
                partialPath = $"Views/menus/_{partial.PartialName}.yml";
                if (!File.Exists(partialPath))
                    continue;
            }

            MenuTemplate partialTemplate = YamlLoader.Load<MenuTemplate>(partialPath);

            MenuRenderer partialRenderer = new(partialTemplate.View, character, partialTemplate.InsertOptions);
            partialRenderer.Render();

            int yMin = partial.Y[0];
            int yMax = partial.Y[1];
            int xMin = partial.X[0];
            int xMax = partial.X[1];
            InsertPartialToView(view, partialRenderer.View, yMin, yMax, xMin, xMax);
        }
    }

    private void InsertArts(List<string> view, List<ArtConfig> arts)
    {
        if (_artsToAdd.Count > 0)
        {
            for (int i = 0; i < _artsToAdd.Count && i < arts.Count; i++)
            {
                (string? artType, object? artObj) = _artsToAdd[i];
                List<string> artLines = ArtRenderer.Load(artObj, artType);

                (int yMin, int yMax, int xMin, int xMax) = AlignArtToViewField(artLines, arts[i]);
                InsertPartialToView(view, artLines, yMin, yMax, xMin, xMax);
            }
        }
        else if (_entity != null)
        {
            for (int i = 0; i < arts.Count; i++)
            {
                List<string> artLines = ArtRenderer.Load(_entity, "main");
                (int yMin, int yMax, int xMin, int xMax) = AlignArtToViewField(artLines, arts[i]);
                InsertPartialToView(view, artLines, yMin, yMax, xMin, xMax);
            }
        }
    }

    /// <summary>
    /// Centre l'art dans le champ défini par la configuration
    /// </summary>
    private static (int yMin, int yMax, int xMin, int xMax) AlignArtToViewField(List<string> art, ArtConfig artConfig)
    {
        int fieldYMin = artConfig.Y[0];
        int fieldYMax = artConfig.Y[1];
        int fieldXMin = artConfig.X[0];
        int fieldXMax = artConfig.X[1];

        int fieldYCenter = (fieldYMin + fieldYMax) / 2;
        int fieldXCenter = (fieldXMin + fieldXMax) / 2;

        int artHeight = art.Count;
        int artWidth = art.Count > 0 ? art[0].Length : 0;

        int yHalf1 = artHeight / 2 - (artHeight % 2 == 1 ? 0 : 1);
        int yHalf2 = artHeight / 2;
        int yMin = fieldYCenter - yHalf1;
        int yMax = fieldYCenter + yHalf2;

        int xHalf1 = artWidth / 2 - (artWidth % 2 == 1 ? 0 : 1);
        int xHalf2 = artWidth / 2;
        int xMin = fieldXCenter - xHalf1;
        int xMax = fieldXCenter + xHalf2;

        return (yMin, yMax, xMin, xMax);
    }

    /// <summary>
    /// Insère caractère par caractère dans la vue
    /// </summary>
    private static void InsertPartialToView(List<string> view, List<string> partial, int yMin, int yMax, int xMin, int xMax)
    {
        int i = 0;
        for (int y = yMin; y <= yMax && i < partial.Count; y++, i++)
        {
            if (y < 0 || y >= view.Count)
                continue;

            char[] lineChars = view[y].ToCharArray();

            int j = 0;
            for (int x = xMin; x <= xMax && j < partial[i].Length; x++, j++)
            {
                if (x >= 0 && x < lineChars.Length)
                {
                    lineChars[x] = partial[i][j];
                }
            }

            view[y] = new string(lineChars);
        }
    }
}
