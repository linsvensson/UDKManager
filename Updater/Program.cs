using System;

namespace Updater
{
    internal class Program
    {
        private static UpdateManager _updateManager;

        private static void Main()
        {
            Start();
        }

        private static void Start()
        {      
            Console.WriteLine("                              ▄              ▄");
            Console.WriteLine("                             ▌▒█           ▄▀▒▌           ");
            Console.WriteLine("                             ▌▒▒▀▄       ▄▀▒▒▒▐           ");
            Console.WriteLine("                            ▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐                      ");
            Console.WriteLine("                          ▄▄▀▒▒▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐           ");
            Console.WriteLine("                        ▄▀▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▀██▀▒▌           ");
            Console.WriteLine("                       ▐▒▒▒▄▄▄▒▒▒▒▒▒▒▒▒▒▒▒▒▀▄▒▒▌          ");
            Console.WriteLine("                       ▌▒▒▐▄█▀▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐          ");
            Console.WriteLine("                      ▐▒▒▒▒▒▒▒▒▒▒▒▌██▀▒▒▒▒▒▒▒▒▀▄▌         ");
            Console.WriteLine("                      ▌▒▀▄██▄▒▒▒▒▒▒▒▒▒▒▒░░░░▒▒▒▒▌         ");
            Console.WriteLine("                      ▌▀▐▄█▄█▌▄▒▀▒▒▒▒▒▒░░░░░░▒▒▒▐         ");
            Console.WriteLine("                     ▐▒▀▐▀▐▀▒▒▄▄▒▄▒▒▒▒▒░░░░░░▒▒▒▒▌        ");
            Console.WriteLine("                     ▐▒▒▒▀▀▄▄▒▒▒▄▒▒▒▒▒▒░░░░░░▒▒▒▐         ");
            Console.WriteLine("                      ▌▒▒▒▒▒▒▀▀▀▒▒▒▒▒▒▒▒░░░░▒▒▒▒▌         ");
            Console.WriteLine("                      ▐▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▐          ");
            Console.WriteLine("                       ▀▄▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▄▒▒▒▒▌          ");
            Console.WriteLine("                         ▀▄▒▒▒▒▒▒▒▒▒▒▄▄▄▀▒▒▒▒▄▀           ");
            Console.WriteLine("                        ▐▀▒▀▄▄▄▄▄▄▀▀▀▒▒▒▒▒▄▄▀             ");
            Console.WriteLine("                       ▐▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▀▀                \n\n");

            Tones.PlaySong(UpdateManager.Intro());

                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    UpdateManager.Animate("Starting Update . . .\n\n", 100);
                    _updateManager = new UpdateManager("https://dl.dropboxusercontent.com/u/41918503/PinkPoo/AppCast.xml");
                    _updateManager.Install();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("-------------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("ERROR! Could not install update: " + ex.Message);
                    Console.WriteLine("\nPoking Lin seems like a good idea.");
                    Console.ForegroundColor = ConsoleColor.White;
                    UpdateManager.Animate("\n\nPress any key to exit . . .", 100);

                    Console.ReadKey(false);
                }
        }
    }
}
