using DungeonAttack.Infrastructure;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DungeonAttack.Renderers;

/// <summary>
/// Renderer pour les menus (remplace les champs par les valeurs)
/// </summary>
public partial class MenuRenderer(List<string> view, object? entity, Dictionary<int, Dictionary<string, FieldOptions>>? insertOptions)
{
    private readonly List<string> _view = [.. view];
    private readonly object? _entity = entity;
    private readonly Dictionary<int, Dictionary<string, FieldOptions>>? _insertOptions = insertOptions;

    public List<string> View => _view;

    public MenuRenderer Render()
    {
        if (_insertOptions == null || _entity == null)
            return this;

        foreach ((int lineIndex, Dictionary<string, FieldOptions>? fields) in _insertOptions)
        {
            if (lineIndex >= _view.Count) continue;

            foreach ((string? fieldChar, FieldOptions? options) in fields)
            {
                string pattern = $"{fieldChar}{{3,}}";
                Match match = Regex.Match(_view[lineIndex], pattern);

                if (match.Success)
                {
                    int fieldLength = match.Value.Length;
                    string data = GetEntityValue(options.Methods);
                    string aligned = AlignData(data, fieldLength, options.Modifier);

                    Regex regex = new(pattern);
                    _view[lineIndex] = regex.Replace(_view[lineIndex], aligned, 1);
                }
            }
        }

        return this;
    }

    private string GetEntityValue(List<string> methods)
    {
        if (_entity == null) return "";

        object? current = _entity;

        foreach (string method in methods)
        {
            if (current == null) return "";

            if (LogMethodPattern().IsMatch(method))
            {
                string[] parts = method.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int index))
                {
                    Type type = current.GetType();
                    PropertyInfo? logProp = type.GetProperty("Log", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (logProp != null)
                    {
                        if (logProp.GetValue(current) is List<string> logList && index >= 0 && index < logList.Count)
                        {
                            current = logList[index];
                        }
                        else
                        {
                            current = "";
                        }
                    }
                    else
                    {
                        MethodInfo? getLogMethod = type.GetMethod("GetLog", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (getLogMethod != null)
                        {
                            current = getLogMethod.Invoke(current, [index]);
                        }
                        else
                        {
                            current = "";
                        }
                    }
                }
                continue;
            }

            if (method.Equals("round", StringComparison.OrdinalIgnoreCase))
            {
                if (current is double d)
                    current = (int)Math.Round(d);
                else if (current is float f)
                    current = (int)Math.Round(f);
                continue;
            }

            if (method.Contains("__"))
            {
                string[] parts = method.Split("__");
                if (parts.Length == 2)
                {
                    string methodName = parts[0];
                    string parameter = parts[1];

                    string pascalMethodName = SnakeCaseToPascalCase(methodName);

                    Type type = current.GetType();
                    
                    PropertyInfo? indexedProp = type.GetProperty($"{pascalMethodName}__{parameter}", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (indexedProp != null)
                    {
                        current = indexedProp.GetValue(current);
                        continue;
                    }

                    MethodInfo? methodInfo = type.GetMethod(pascalMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (methodInfo != null)
                    {
                        if (int.TryParse(parameter, out int paramValue))
                        {
                            current = methodInfo.Invoke(current, [paramValue]);
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                string pascalName = SnakeCaseToPascalCase(method);
                
                Type type = current.GetType();
                PropertyInfo? prop = type.GetProperty(pascalName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop != null)
                {
                    current = prop.GetValue(current);
                }
                else
                {
                    MethodInfo? methodInfo = type.GetMethod(pascalName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (methodInfo != null)
                    {
                        current = methodInfo.Invoke(current, null);
                    }
                    else
                    {
                        methodInfo = type.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (methodInfo != null)
                        {
                            current = methodInfo.Invoke(current, null);
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
        }

        return current?.ToString() ?? "";
    }

    /// <summary>
    /// Convertit snake_case en PascalCase
    /// </summary>
    private static string SnakeCaseToPascalCase(string snakeCase)
    {
        if (string.IsNullOrEmpty(snakeCase))
            return snakeCase;

        if (snakeCase.Equals("lvl", StringComparison.OrdinalIgnoreCase))
            return "Level";

        return string.Concat(
            snakeCase.Split('_')
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower())
        );
    }

    private static string AlignData(string data, int fieldLength, string modifier)
    {
        if (data.Length >= fieldLength)
            return data[..fieldLength];

        return modifier switch
        {
            "m" => data.Center(fieldLength), // middle
            "s" => data.PadRight(fieldLength), // start (left)
            "e" => data.PadLeft(fieldLength), // end (right)
            _ => data.PadRight(fieldLength)
        };
    }

    [GeneratedRegex(@"^log_\d+$")]
    private static partial Regex LogMethodPattern();
}
