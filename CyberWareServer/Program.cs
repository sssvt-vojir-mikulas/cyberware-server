using System;
using static System.Console;

namespace CyberWareServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Title = "CyberWare server 0.0.1b";
            Console.SetWindowSize(60, 15);

            ForegroundColor = ConsoleColor.Black;
            BackgroundColor = ConsoleColor.DarkYellow;
            WriteLine("Je to komplikované a cool!");
            WriteLine();
            ResetColor();

            Server.Start(4,42069);
            ReadKey();
        }
    }
}
