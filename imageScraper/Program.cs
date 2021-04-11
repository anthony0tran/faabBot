using System;

namespace imageScraper
{
    internal static class Program
    {
        private static string _urlInput = "";

        private static void Main(string[] args)
        {
            while (_urlInput != "exit")
            {
                Console.WriteLine("Please insert URL:");
                _urlInput = Console.ReadLine();

                if (_urlInput == "exit")
                {
                    return;
                }

                Init(_urlInput);

            }

            Environment.Exit(0);
        }

        private static void Init(string urlInput)
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
                Console.WriteLine("fetching page: " + urlInput);
                // Fetch all the urls to the items.
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
    }
}