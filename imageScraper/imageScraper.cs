using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace imageScraper
{
    public class ImageScraper
    {
        // Get the path to the Drivers directory. This will be used by ChromeDriver to access chromedriver.exe
        private static readonly string ChromeDriverPath = Path.Combine(Directory.GetCurrentDirectory(), "Drivers");

        // Get the path to the Downloads directory. This directory will be filled with the downloaded images.
        private static readonly string DownloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

        private static ChromeDriver _catalogDriver;

        private static readonly WebClient WebClient = new();

        private static int _globalDownloadCounter;

        /*
         *  This function is executed when the browser is on the BapeLand catalog.
         *  <param name="url">The URL of the catalog page</param>
         */
        public static IEnumerable<string> GetItemsUrl(string url)
        {
            _catalogDriver = new ChromeDriver(ChromeDriverPath);
            
            // var driver = new ChromeDriver(ChromeDriverPath);
            _catalogDriver.Navigate().GoToUrl(url);

            Thread.Sleep(1000);

            // Get all <img class="o-responsive-thumbnail__image">. This is the collection of clickable items.
            var items = _catalogDriver.FindElements(By.ClassName("c-catalog-header__link"));

            Console.WriteLine("items: " + items.Count);

            var itemsUrl = items.Select(item => item.GetAttribute("href")).ToList();
            
            Thread.Sleep(1000);

            Console.WriteLine("Item count: " + items.Count);

            _catalogDriver.Close();
            return itemsUrl;
        }

        /*
         *  This function is executed when the browser is on the page of a specific item.
         *  <param name="url">The URL of the item</param>
         */
        public static void DownloadImages(string url)
        {
            // Get the path to the Drivers directory. This will be used by ChromeDriver to access chromedriver.exe
            var chromeDriverPath = Path.Combine(Directory.GetCurrentDirectory(), "Drivers");
            var itemDriver = new ChromeDriver(chromeDriverPath);
            itemDriver.Navigate().GoToUrl(url);

            // This is a collection of the different color variants of the item. These objects are clicked to show the actual image.
            var itemColors = itemDriver.FindElements(By.ClassName("p-goods-thumbnail-list__item"));

            /*
             This is a collection of color tags. These tags are used to determine the download count per item.
             Given only the first few pictures are tagged. (This will not work if the pattern is different)
            */
            var itemColorsTags = itemDriver.FindElements(By.ClassName("p-goods-photograph__name"));

            for (var i = 0; i < itemColorsTags.Count; i++)
            {
                itemColors[i].Click();
                Thread.Sleep(1000);

                var imageObject = itemDriver.FindElementByXPath("//div[@id=\"photoMain\"]/img");
                var imageSource = imageObject.GetAttribute("src");
                var itemName = itemDriver.FindElementByClassName("p-goods-information__heading").GetAttribute("innerHTML");

                // itemName may contain illegal characters. Replace all illegal characters with _
                itemName = Path.GetInvalidPathChars().Aggregate(itemName, (current, character) => current.Replace(character, '_'));

                // File name consist of "Name" + "imageIteration" + "itemIteration"
                WebClient.DownloadFile(imageSource, DownloadsPath + @"\" + itemName + i + "_" + _globalDownloadCounter + ".jpg");
                _globalDownloadCounter++;
            }

            Console.WriteLine("Downloaded images from: " + url);
            itemDriver.Close();
        }
    }
}