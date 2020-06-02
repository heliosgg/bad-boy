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

            BadBoyLogger.RegisterInfoLogger(Console.WriteLine);
            BadBoyLogger.RegisterErrorLogger(Console.Error.WriteLine);

            var badBoy = new BadBoyCore();
            badBoy.Start();

            Console.ReadKey();

            badBoy.Stop();
        }
    }
}