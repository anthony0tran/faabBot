using System;

namespace imageScraper
{
    internal static class Program
    {
        private static string _urlInput = "";
        private const double Version = 0.1;

        private static void Main(string[] args)
        {
            Console.Title = "FaabBot";
            PrintStartupArt();
            
            while (_urlInput != "exit")
            {
                Console.WriteLine("Please insert URL:");
                _urlInput = Console.ReadLine();

                while (!IsUrlValid(_urlInput) && _urlInput != "exit")
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

            Console.WriteLine("Sessions terminated...\nExit by closing this console window.");
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
            Console.WriteLine(asciiArt + "v" + Version + "\n");
        }
    }
}