namespace DungeonAttack.Infrastructure;

/// <summary>
/// Orientation du menu pour déterminer quelles flèches utiliser
/// </summary>
public enum MenuOrientation
{
    Vertical,
    Horizontal
}

/// <summary>
/// Système de sélection de menu avec navigation par flèches
/// </summary>
public class MenuSelector(List<MenuOption> options, Action renderAction, MenuOrientation orientation = MenuOrientation.Vertical, int itemsCount = 0)
{
    private int _selectedIndex = 0;
    private int _previousIndex = 0;
    private readonly List<MenuOption> _options = options;
    private readonly string _indicator = "►";
    private readonly Action _renderAction = renderAction;
    private readonly MenuOrientation _orientation = orientation;
    private readonly int _itemsCount = itemsCount;

    public int SelectedIndex => _selectedIndex;

    private bool IsHybridMode => _itemsCount > 0 && _itemsCount < _options.Count;
    private bool IsInItemsZone => _selectedIndex < _itemsCount;
    private bool IsInActionsZone => _selectedIndex >= _itemsCount;
    private int ActionsCount => _options.Count - _itemsCount;

    /// <summary>
    /// Affiche le menu et attend la sélection de l'utilisateur
    /// </summary>
    /// <returns>La valeur de l'option sélectionnée</returns>
    public string Select()
    {
        while (true)
        {
            _renderAction();

            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    HandleUpArrow();
                    break;

                case ConsoleKey.DownArrow:
                    HandleDownArrow();
                    break;

                case ConsoleKey.LeftArrow:
                    HandleLeftArrow();
                    break;

                case ConsoleKey.RightArrow:
                    HandleRightArrow();
                    break;

                case ConsoleKey.Enter:
                    return _options[_selectedIndex].Value;

                case ConsoleKey.Escape:
                case ConsoleKey.Backspace:
                    MenuOption? backOption = _options.FirstOrDefault(o => o.Value == "0" || o.Value == "back");
                    if (backOption != null)
                        return backOption.Value;
                    break;

                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    if (_options.Any(o => o.Value == "0"))
                        return "0";
                    break;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    if (_options.Any(o => o.Value == "1"))
                        return "1";
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    if (_options.Any(o => o.Value == "2"))
                        return "2";
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    if (_options.Any(o => o.Value == "3"))
                        return "3";
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    if (_options.Any(o => o.Value == "4"))
                        return "4";
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    if (_options.Any(o => o.Value == "5"))
                        return "5";
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    if (_options.Any(o => o.Value == "6"))
                        return "6";
                    break;
                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    if (_options.Any(o => o.Value == "7"))
                        return "7";
                    break;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    if (_options.Any(o => o.Value == "8"))
                        return "8";
                    break;
                case ConsoleKey.D9:
                case ConsoleKey.NumPad9:
                    if (_options.Any(o => o.Value == "9"))
                        return "9";
                    break;
            }
        }
    }

    /// <summary>
    /// Retourne l'indicateur pour une option donnée
    /// </summary>
    public string GetIndicator(int index)
    {
        return index == _selectedIndex ? _indicator : " ";
    }

    /// <summary>
    /// Retourne l'indicateur pour une valeur d'option donnée
    /// </summary>
    public string GetIndicatorForValue(string value)
    {
        int index = _options.FindIndex(o => o.Value == value);
        return index == _selectedIndex ? _indicator : " ";
    }

    #region Navigation Handlers

    private void HandleUpArrow()
    {
        if (IsHybridMode)
        {
            if (IsInActionsZone)
            {
                _selectedIndex = _previousIndex >= 0 && _previousIndex < _itemsCount
                    ? _previousIndex
                    : _itemsCount - 1;
            }
            else
            {
                _selectedIndex = (_selectedIndex - 1 + _itemsCount) % _itemsCount;
            }
        }
        else if (_orientation == MenuOrientation.Vertical)
        {
            _selectedIndex = (_selectedIndex - 1 + _options.Count) % _options.Count;
        }
        else if (_orientation == MenuOrientation.Horizontal && _selectedIndex == _options.Count - 1)
        {
            _selectedIndex = _previousIndex;
        }
    }

    private void HandleDownArrow()
    {
        if (IsHybridMode)
        {
            if (IsInItemsZone)
            {
                int nextIndex = _selectedIndex + 1;
                if (nextIndex >= _itemsCount)
                {
                    _previousIndex = _selectedIndex;
                    _selectedIndex = _itemsCount;
                }
                else
                {
                    _selectedIndex = nextIndex;
                }
            }
        }
        else if (_orientation == MenuOrientation.Vertical)
        {
            _selectedIndex = (_selectedIndex + 1) % _options.Count;
        }
        else if (_orientation == MenuOrientation.Horizontal && _selectedIndex != _options.Count - 1)
        {
            _previousIndex = _selectedIndex;
            _selectedIndex = _options.Count - 1;
        }
    }

    private void HandleLeftArrow()
    {
        if (IsHybridMode)
        {
            if (IsInActionsZone)
            {
                int actionIndex = _selectedIndex - _itemsCount;
                actionIndex = (actionIndex - 1 + ActionsCount) % ActionsCount;
                _selectedIndex = _itemsCount + actionIndex;
            }
        }
        else if (_orientation == MenuOrientation.Horizontal)
        {
            _selectedIndex = (_selectedIndex - 1 + _options.Count) % _options.Count;
        }
    }

    private void HandleRightArrow()
    {
        if (IsHybridMode)
        {
            if (IsInActionsZone)
            {
                int actionIndex = _selectedIndex - _itemsCount;
                actionIndex = (actionIndex + 1) % ActionsCount;
                _selectedIndex = _itemsCount + actionIndex;
            }
        }
        else if (_orientation == MenuOrientation.Horizontal)
        {
            _selectedIndex = (_selectedIndex + 1) % _options.Count;
        }
    }

    #endregion
}

/// <summary>
/// Représente une option de menu
/// </summary>
public class MenuOption(string value, string label)
{
    public string Value { get; set; } = value;
    public string Label { get; set; } = label;
}
