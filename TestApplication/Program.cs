using System;
using System.Text;

using BadBoy;

namespace TestApplication
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            
            BadBoyLogger.Info = Console.WriteLine;
            BadBoyLogger.Error = Console.Error.WriteLine;

            var badBoy = new BadBoyCore();
            badBoy.Start();

            Console.ReadKey();

            badBoy.Stop();
        }
    }
}