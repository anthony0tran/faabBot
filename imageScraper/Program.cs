using System;

namespace imageScraper
{
    internal static class Program
    {
        private static string _urlInput = "";

        private static void Main(string[] args)
        {
            Console.Title = "FaabBot";
            PrintStartupArt();
            
            while (_urlInput != "exit")
            {
                Console.WriteLine("Type 'help' for more information\nPlease insert the URL, and then press Enter");
                _urlInput = Console.ReadLine();

                if (_urlInput == "help")
                {
                    PrintHelp();
                    Console.WriteLine("\nPlease insert URL:");
                    _urlInput = Console.ReadLine();
                }
                
                while (!IsUrlValid(_urlInput) && _urlInput != "exit" && !int.TryParse(_urlInput, out _) &&
                       _urlInput != "help")
                {
                    Console.WriteLine("Invalid URL!\n");
                    Console.WriteLine("Please insert URL:");
                    _urlInput = Console.ReadLine();
                }
                
                if (_urlInput == "exit")
                {
                    break;
                }
                
                InitScraping(_urlInput);
            }

            Environment.Exit(0);
        }

        /*
         *  This function is called when all prerequisites is in order.
         *  <param name="urlInput">The URL inserted in the console by the user.</param>
         */
        private static void InitScraping(string urlInput)
        {
            // Fetch the whole catalog if no url is provided.
            if (urlInput == string.Empty)
            {
                // Fetch all the urls to the items.
                var itemsUrls = ImageScraper.GetItemsUrl("https://zozo.jp/shop/bapeland/");

                // Download the images per item.
                foreach (var url in itemsUrls)
                {
                    ImageScraper.DownloadImages(url);
                }
            }
            // Fetch the whole catalog on the specified page number if an integer is provided.
            else if (int.TryParse(urlInput, out _))
            {
                // Fetch all the urls to the items on the given catalog page.
                var itemsUrls = ImageScraper.GetItemsUrl("https://zozo.jp/shop/bapeland/?pno=" + urlInput);

                // Download the images per item.
                foreach (var url in itemsUrls)
                {
                    ImageScraper.DownloadImages(url);
                }
            }
            else
            {
                try
                {
                    ImageScraper.DownloadImages(urlInput);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
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
            const string asciiArt = @"    ______            __    ____        __     __             ___          __  __                     
   / ____/___ _____ _/ /_  / __ )____  / /_   / /_  __  __   /   |  ____  / /_/ /_  ____  ____  __  __
  / /_  / __ `/ __ `/ __ \/ __  / __ \/ __/  / __ \/ / / /  / /| | / __ \/ __/ __ \/ __ \/ __ \/ / / /
 / __/ / /_/ / /_/ / /_/ / /_/ / /_/ / /_   / /_/ / /_/ /  / ___ |/ / / / /_/ / / / /_/ / / / / /_/ / 
/_/    \__,_/\__,_/_.___/_____/\____/\__/  /_.___/\__, /  /_/  |_/_/ /_/\__/_/ /_/\____/_/ /_/\__, /  
                                                 /____/                                      /____/   ";
            Console.WriteLine(asciiArt + "\n");
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"
                1. An URL to the item page. Example: https://zozo.jp/shop/bapeland/goods/32485477/?did=56620270&rid=1019
                2. An integer, this is the index of the catalog. Example: 8
                3. Nothing. The whole first catalog will be scraped if the user presses Enter without providing data.
                4. Type 'exit' to close the console.

                Go to 'https://github.com/anthony0tran/faabBot' for more information.
            ");
        }
    }
}