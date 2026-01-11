using System.Runtime.InteropServices;

namespace DungeonAttack.Infrastructure;

/// <summary>
/// Gère les paramètres avancés de la fenêtre console (Windows uniquement)
/// </summary>
public static partial class ConsoleWindowManager
{
    #region Windows API

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetConsoleWindow();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static partial nint GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static partial nint GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static partial nint SetWindowLongPtr64(IntPtr hWnd, int nIndex, nint dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static partial nint SetWindowLongPtr32(IntPtr hWnd, int nIndex, nint dwNewLong);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll")]
    private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

    // Wrappers pour 32/64-bit
    private static nint GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : GetWindowLongPtr32(hWnd, nIndex);
    }

    private static nint SetWindowLongPtr(IntPtr hWnd, int nIndex, nint dwNewLong)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
    }

    private const int STD_INPUT_HANDLE = -10;
    private const int GWL_STYLE = -16;

    // Window Styles
    private const nint WS_MAXIMIZEBOX = 0x10000;
    private const nint WS_SIZEBOX = 0x40000;

    // Console Modes
    private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
    private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    private const uint ENABLE_INSERT_MODE = 0x0020;

    // System Menu
    private const uint SC_MAXIMIZE = 0xF030;
    private const uint SC_SIZE = 0xF000;
    private const uint MF_BYCOMMAND = 0x00000000;

    // SetWindowPos flags
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;

    #endregion

    /// <summary>
    /// Désactive la sélection de texte, le redimensionnement, le fullscreen, etc.
    /// </summary>
    public static void DisableConsoleFeatures()
    {
        if (!OperatingSystem.IsWindows())
            return;

        try
        {
            IntPtr consoleWindow = GetConsoleWindow();
            if (consoleWindow == IntPtr.Zero)
                return;

            DisableQuickEditMode();

            DisableWindowResize(consoleWindow);

            DisableSystemMenuOptions(consoleWindow);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Désactive le Quick Edit Mode pour empêcher la sélection de texte
    /// </summary>
    private static void DisableQuickEditMode()
    {
        IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
        if (consoleHandle == IntPtr.Zero)
            return;

        if (GetConsoleMode(consoleHandle, out uint mode))
        {
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode &= ~ENABLE_INSERT_MODE;

            mode |= ENABLE_EXTENDED_FLAGS;

            SetConsoleMode(consoleHandle, mode);
        }
    }

    /// <summary>
    /// Désactive le redimensionnement de la fenêtre console
    /// </summary>
    private static void DisableWindowResize(IntPtr consoleWindow)
    {
        nint style = GetWindowLongPtr(consoleWindow, GWL_STYLE);

        style &= ~WS_MAXIMIZEBOX;
        style &= ~WS_SIZEBOX;

        _ = SetWindowLongPtr(consoleWindow, GWL_STYLE, style);

        SetWindowPos(consoleWindow, IntPtr.Zero, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    /// <summary>
    /// Désactive certaines options du menu système (clic droit sur la barre de titre)
    /// </summary>
    private static void DisableSystemMenuOptions(IntPtr consoleWindow)
    {
        IntPtr systemMenu = GetSystemMenu(consoleWindow, false);
        if (systemMenu != IntPtr.Zero)
        {
            DeleteMenu(systemMenu, SC_MAXIMIZE, MF_BYCOMMAND);

            DeleteMenu(systemMenu, SC_SIZE, MF_BYCOMMAND);
        }
    }
}
