using System;
using System.Threading;
using static System.Console;


namespace CyberWareServer
{
    class Program
    {

        private static bool isRunning = false;
        static void Main(string[] args)
        {
            Title = "CyberWare server 0.0.1b";
 
            isRunning = true;

            ForegroundColor = ConsoleColor.Black;
            BackgroundColor = ConsoleColor.DarkYellow;
            WriteLine("Je to komplikované a cool!");
            WriteLine();
            ResetColor();

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(4,42069);
             
        }

        private static void MainThread()
        {
            Debug.Log($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
