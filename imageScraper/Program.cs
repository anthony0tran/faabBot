using System;

namespace imageScraper
{
    internal static class Program
    {
        private static string _urlInput = "";
        private const double Version = 0.6;

        private static void Main(string[] args)
        {
            Console.Title = "FaabBot";
            PrintStartupArt();
            Console.WriteLine("Type 'help' for more information.");

            while (_urlInput != "exit")
            {
                InitScraping();
            }


            Console.WriteLine("Sessions terminated... \nClosing this window...");
        }

        /*
         *  This function is called when all prerequisites is in order.
         *  <param name="urlInput">The URL inserted in the console by the user.</param>
         */
        private static void InitScraping()
        {
            ImageScraper.PrintClothingSizes();
            Console.WriteLine("\nPlease insert an url or command, and then press Enter");
            _urlInput = Console.ReadLine();

            switch (_urlInput)
            {
                case "exit":
                    return;
                case "help":
                    PrintHelp();
                    break;
                case "add-size":
                    Console.WriteLine("Please insert a clothing size, and then press Enter");
                    var addSizeInput = Console.ReadLine();
                    ImageScraper.AddClothingSize(addSizeInput);
                    break;
                case "remove-size":
                    Console.WriteLine("Please insert a clothing size, and then press Enter");
                    var removeSizeInput = Console.ReadLine();
                    ImageScraper.RemoveClothingSize(removeSizeInput);
                    break;
                case "show-size":
                    break;
                case "reset-size":
                    ImageScraper.ResetClothingSize();
                    break;
                default:
                    if (!IsUrlValid(_urlInput))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid URL!");
                        Console.ResetColor();
                    }
                    else
                    {
                        ImageScraper.DownloadAllImages(_urlInput);
                    }

                    break;
            }
        }

        /*
         *  This function checks if the user input is a valid URL.
         *  <param name="url">The URL inserted in the console by the user.</param>
         */
        private static bool IsUrlValid(string url)
        {
            var tryCreateResult = Uri.TryCreate(url, UriKind.Absolute, out _);
            return tryCreateResult;
        }

        private static void PrintStartupArt()
        {
            const string asciiArt =
                @"    ______            __    ____        __     __             ___          __  __                     
   / ____/___ _____ _/ /_  / __ )____  / /_   / /_  __  __   /   |  ____  / /_/ /_  ____  ____  __  __
  / /_  / __ `/ __ `/ __ \/ __  / __ \/ __/  / __ \/ / / /  / /| | / __ \/ __/ __ \/ __ \/ __ \/ / / /
 / __/ / /_/ / /_/ / /_/ / /_/ / /_/ / /_   / /_/ / /_/ /  / ___ |/ / / / /_/ / / / /_/ / / / / /_/ / 
/_/    \__,_/\__,_/_.___/_____/\____/\__/  /_.___/\__, /  /_/  |_/_/ /_/\__/_/ /_/\____/_/ /_/\__, /  
                                                 /____/                                      /____/   ";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(asciiArt + "v" + Version + "\n");
            Console.ResetColor();
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"
    The following things can be typed into the console:
    1. An URL to the catalog page. e.g. 'https://zozo.jp/shop/bapeland/shoes/'
    2. Type 'exit' to close the console.
    3. Set one or multiple sizes to scrape, use the following commands:
       - 'add-size'
       - 'remove-size'
       - 'reset-size'
       - 'show-size'

    Go to 'https://github.com/anthony0tran/faabBot' for more information.
            ");
            ImageScraper.PrintClothingSizes();
            Console.WriteLine("\nPlease insert an url or command, and then press Enter");
            _urlInput = Console.ReadLine();
        }
    }
}