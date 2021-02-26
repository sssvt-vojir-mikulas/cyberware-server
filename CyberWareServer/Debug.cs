using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
namespace CyberWareServer
{
    class Debug
    {

        public static void Log(string _message)
        {
            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine(_message);
            ResetColor();
        }

        public static void ErrorLog(string _message)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(_message);
            ResetColor();
        }

        public static void AlertLog(string _message)
        {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine(_message);
            ResetColor();
        }

         
        public static void Heading(string _message)
        {
            WriteLine(_message);
            WriteLine("-----------------------------------------");
            WriteLine();
        }

    }
}
