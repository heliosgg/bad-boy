using System.Text.RegularExpressions;

namespace BadBoy.BackEnd
{
    internal static class Filter
    {
        // TODO: Add more words support
        private const string FilterRegex =
            @"\b(об)?муд[оаеиы][книзш][а-яё]{0,4}" +
            @"|г[ао]ндон([ауоеи]?[а-я]{0,3})?" +
            @"|[а-яё]{0,4}ху[йяеиюёл][а-яё]{0,8}" +
            @"|([а-яё]{0,5}[ъаоыеу])?[её]б([анкщиыеёуолс][а-яё]{0,6})?" +
            @"|([а-яё]{1,3}[зыоабтес])?др[ао]ч[иеёк][а-яё]{0,4}" +
            @"|([а-яё]{0,4}[ыаоу])?бля([дт][а-яё]{0,7})?" +
            @"|([а-яё]{0,3}[а-вс-уыоед])?п[иеё]зд[аиоуюяеёы]+[а-яё]{0,10}" +
            @"|([а-яё]{0,3}[а-воызтедис])?м[ао]нд[аеиюяоуы][а-яё]{0,8}" +
            @"|([а-яё]{0,3}[оаие])?х[еи]р([аоньеису][а-яё]{0,7})?" +
            @"|[а-яё]{0,4}п[ие]д[оеа]?р([аиоюс][а-яё]{0,9})?\b";

        public static bool Analyse(string word, out string analysedWord)
        {
            if (Regex.IsMatch(word, FilterRegex, RegexOptions.IgnoreCase))
            {
                analysedWord = CensorWord(word);

                return true;
            }

            analysedWord = word;

            return false;
        }

        private static string CensorWord(string word)
        {
            var charArray = word.ToCharArray();

            for (int i = 1; i < word.Length - 1; i++)
            {
                charArray[i] = '*';
            }

            return new string(charArray);
        }
    }
}