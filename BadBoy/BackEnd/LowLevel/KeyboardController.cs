using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace BadBoy.BackEnd.LowLevel
{
    internal static class KeyboardController
    {
        private delegate IntPtr LowLevelKeyboardProcess(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcess lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint thread);

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP   = 0x101;

        private const int EnglishCultureCode      = 1033;
        private const int EnglishKeyboardLayoutId = 1049;

        private static readonly bool[] KeyStates = new bool[256];

        private static LowLevelKeyboardProcess process = HookCalback;
        private static IntPtr                  hookId  = IntPtr.Zero;

        public static event Action<string> OnWordTyped;

        private static StringBuilder currentWord = new StringBuilder();

        public static bool HookKeyboard()
        {
            if (hookId != IntPtr.Zero)
                return false;

            hookId = SetHook(process);

            return hookId != IntPtr.Zero;
        }

        public static bool ReleaseKeyboard()
        {
            if (hookId == IntPtr.Zero)
                return false;

            bool result = UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;

            return result;
        }

        private static IntPtr HookCalback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                int virtualKey = Marshal.ReadInt32(lParam);
                KeyStates[virtualKey] = true;
                HandleKey(virtualKey);
            }

            if (nCode >= 0 && wParam == (IntPtr) WM_KEYUP)
            {
                int virtualKey = Marshal.ReadInt32(lParam);
                KeyStates[virtualKey] = false;
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProcess process)
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var currenModule   = currentProcess.MainModule;

            return SetWindowsHookEx(WH_KEYBOARD_LL, process, GetModuleHandle(currenModule?.ModuleName), 0);
        }

        private static void HandleKey(int virtualKey)
        {
            switch ((VirtualKeys) virtualKey)
            {
                case VirtualKeys.Backspace:

                    if (currentWord.Length == 0)
                        return;

                    if (KeyStates[(int) VirtualKeys.Control] ||
                        KeyStates[(int) VirtualKeys.LeftControl] ||
                        KeyStates[(int) VirtualKeys.RightControl])
                    {
                        currentWord = currentWord.Clear();

                        return;
                    }

                    currentWord = currentWord.Remove(currentWord.Length - 1, 1);

                    return;

                case VirtualKeys.Enter:
                case VirtualKeys.Space:

                    if (currentWord.Length == 0)
                        return;

                    OnWordTyped?.Invoke(currentWord.ToString());

                    currentWord = currentWord.Clear();

                    return;

                case VirtualKeys.Alt:
                case VirtualKeys.LeftAlt:
                case VirtualKeys.RightAlt: return;
            }

            char letter = GetKeyboardLetter(virtualKey);

            if (char.IsLetter(letter))
                currentWord.Append(letter);
        }

        private static char GetKeyboardLetter(int virtualKey)
        {
            int layoutId = GetCurrentKeyboardLayout().KeyboardLayoutId;

            char letter = VirtualKeyToLetter(virtualKey);

            if (layoutId == EnglishKeyboardLayoutId)
                letter = EnglishToRussian(letter);

            if (KeyStates[(int) VirtualKeys.Shift] ||
                KeyStates[(int) VirtualKeys.LeftShift] ||
                KeyStates[(int) VirtualKeys.RightShift] ||
                GetKeyState((int) VirtualKeys.CapsLock) != 0)
            {
                return letter;
            }

            return char.ToLower(letter);
        }

        private static CultureInfo GetCurrentKeyboardLayout()
        {
            try
            {
                var  forgroundWindow   = GetForegroundWindow();
                uint foregroundProcess = GetWindowThreadProcessId(forgroundWindow, IntPtr.Zero);

                int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;

                return new CultureInfo(keyboardLayout);
            }
            catch
            {
                return new CultureInfo(EnglishCultureCode);
            }
        }

        private static char EnglishToRussian(char letter)
        {
            letter = char.ToUpper(letter);

            switch (letter)
            {
                case 'Q':        return 'Й';
                case 'W':        return 'Ц';
                case 'E':        return 'У';
                case 'R':        return 'К';
                case 'T':        return 'Е';
                case 'Y':        return 'Н';
                case 'U':        return 'Г';
                case 'I':        return 'Ш';
                case 'O':        return 'Щ';
                case 'P':        return 'З';
                case (char) 219: return 'Х';
                case (char) 221: return 'Ъ';
                case 'A':        return 'Ф';
                case 'S':        return 'Ы';
                case 'D':        return 'В';
                case 'F':        return 'А';
                case 'G':        return 'П';
                case 'H':        return 'Р';
                case 'J':        return 'О';
                case 'K':        return 'Л';
                case 'L':        return 'Д';
                case (char) 186: return 'Ж';
                case (char) 222: return 'Э';
                case 'Z':        return 'Я';
                case 'X':        return 'Ч';
                case 'C':        return 'С';
                case 'V':        return 'М';
                case 'B':        return 'И';
                case 'N':        return 'Т';
                case 'M':        return 'Ь';
                case (char) 188: return 'Б';
                case (char) 190: return 'Ю';
                case '/':        return '.';
                case (char) 192: return 'Ё';
                default:         return letter;
            }
        }

        private static char VirtualKeyToLetter(int virtualKey) => (char) virtualKey;
    }
}