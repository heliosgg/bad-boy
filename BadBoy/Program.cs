using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BadBoy
{
   class Program
   {
      public const bool bPlaySound = false;
      public const bool bDoLog = false;

      static void Main(string[] args)
      {
         C_KbController.OnNewWord += s => {
            if (bDoLog)
            {
               Console.Write("Got a word \'" + s + "\'");
            }

            if (Regex.IsMatch(s,
               @"\b(?:(?:[Оо][Бб])?[Мм][Уу][Дд][ОоАаЕеИиЫы][КкНнИиЗзШш][А-Яа-яЕё]{0,4}" +
               @"|[Гг][АаОо][Нн][Дд][Оо][Нн](?:[АаУуОоЕеИи]?[А-Яа-я]{0,3})?" +
               @"|[А-Яа-яЕё]{0,4}[Хх][Уу][ЙйЯяЕеИиЮюЁёЛл][А-Яа-яЕё]{0,8}" +
               @"|(?:[А-Яа-яЁё]{0,5}[ЪъАаОоЫыЕеУу])?[ЕеЁё][Бб](?:[АаНнКкЩщИиЫыЕеЁёУуОоЛлСс][А-Яа-яЁё]{0,6})?" +
               @"|(?:[А-Яа-яЁё]{1,3}[ЗзЫыОоАаБбТтЕеСс])?[Дд][Рр][АаОо][Чч][ИиЕеЁёКк][А-Яа-яЁё]{0,4}" +
               @"|(?:[А-Яа-яЁё]{0,4}[ЫыАаОоУ])?[Бб][Лл][Яя](?:[ДдТт][А-Яа-яЁё]{0,7})?" +
               @"|(?:[А-Яа-яЁё]{0,3}[А-Ва-вС-Ус-уЫыОоЕеДд])?[Пп][ИиЕеЁё][Зз][Дд][АаИиОоУуЮюЯяЕеЁёЫы]+[А-Яа-яЁё]{0,10}" +
               @"|(?:[А-Яа-яЁё]{0,3}[А-Ва-вОоЫыЗзТтЕеДдИиСс])?[Мм][АаОо][Нн][Дд][АаЕеИиЮюЯяОоУуЫы][А-Яа-яЁё]{0,8}" +
               @"|(?:[А-Яа-яЁё]{0,3}[ОоАаИиЕе])?[Хх][ЕеИи][Рр](?:[АаОоНнЬьЕеИиСсУу][А-Яа-яЁё]{0,7})?" +
               @"|[А-Яа-яЁё]{0,4}[Пп][ИиЕе][Дд][ОоЕеАа]?[Рр](?:[АаИиОоЮюСс][А-Яа-яЁё]{0,9})?)\b"))
            {
               string sStaredWord = StarWord(s);

               if (bPlaySound)
               {
                  System.Media.SystemSounds.Exclamation.Play();
               }

               if (bDoLog)
               {
                  Console.Write(" => " + sStaredWord);
               }

               SendKeys.SendWait("^{BACKSPACE}");
               SendKeys.SendWait(sStaredWord);
            }

            if (bDoLog)
            {
               Console.WriteLine();
            }
         };

         C_KbController.HookKeyboard();

         Application.Run();

         C_KbController.UnhookKeyboard();


      }

      public static string StarWord(string str)
      {
         char[] result = str.ToCharArray();

         for(int i = 1; i < str.Length - 1; i++)
         {
            result[i] = '*';
         }

         return new string(result);
      }

      [DllImport("kernel32.dll")]
      private static extern bool Beep(int dwFreq, int dwDuration);

      [DllImport("user32.dll")]
      static extern bool SetKeyboardState(byte[] lpKeyState);

      [DllImport("user32.dll")]
      static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

      [DllImport("user32.dll")]
      static extern IntPtr GetForegroundWindow();

   }
}
