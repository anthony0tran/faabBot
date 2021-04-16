using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static readonly WebClient WebClient = new();

        // This value is appended to the image name.
        private static int _globalDownloadCounter;

        private static int _highestPageIndex;
        private static IEnumerable<string> _catalogUrls;

        private static List<string> _productList;

        public static IEnumerable<string> GetAllProducts(string url)
        {
            _productList = new List<string>();

            if (_highestPageIndex == 0)
            {
                var tempProductList = GetAllProductsOnPage(url).ToList();

                foreach (var product in tempProductList)
                {
                    _productList.Add(product);
                }
            }

            if (_highestPageIndex > 0)
            {
                var baseUrl = GetCatalogBaseUrl(_catalogUrls);
                for (var i = 2; i <= _highestPageIndex; i++)
                {
                    var tempProductList = GetAllProductsOnPage(baseUrl + i);

                    foreach (var product in tempProductList)
                    {
                        _productList.Add(product);
                    }

                    Console.WriteLine("productList count: " + _productList.ToList().Count);
                }
            }

            return _productList;
        }

        public static void DownloadAllImages(string url)
        {
            GetAllProducts(url);

            foreach (var productUrl in _productList)
            {
                DownloadImages(productUrl);
            }
        }
        
        /*
         *  This function gets all products on the catalog page. Duplicate URLs are only added once. 
         *  <param name="url">The URL of the catalog page</param>
         */
        private static IEnumerable<string> GetAllProductsOnPage(string url)
        {
            var catalogDriver = new ChromeDriver(ChromeDriverPath);
            var itemsUrl = new List<string>();

            catalogDriver.Navigate().GoToUrl(url);

            Thread.Sleep(1000);

            _catalogUrls = GetAllCatalogUrls(catalogDriver);

            var catalogUrls = _catalogUrls.ToList();

            if (catalogUrls.Count > 0)
            {
                _highestPageIndex = GetHighestCatalogIndex(catalogUrls);
            }

            Thread.Sleep(1000);

            // Get all <div class="c-catalog-header">. This is the collection of clickable items.
            var items = catalogDriver.FindElements(By.ClassName("c-catalog-header__link"));

            foreach (var productHtmlObject in items)
            {
                if (itemsUrl.Count == 0)
                {
                    itemsUrl.Add(productHtmlObject.GetAttribute("href"));
                }
                else
                {
                    var lastProductId = GetIdFromUrl(itemsUrl[^1]);
                    if (!productHtmlObject.GetAttribute("href").Contains(lastProductId.ToString()))
                    {
                        itemsUrl.Add(productHtmlObject.GetAttribute("href"));
                    }
                }
            }

            Console.WriteLine("Found " + itemsUrl.Count + " products on " + url);

            catalogDriver.Quit();

            return itemsUrl;
        }

        /*
         *  This function is called when the first catalog page is opened to determine all catalog pages for the category.
         */
        private static IEnumerable<string> GetAllCatalogUrls(ChromeDriver catalogDriver)
        {
            var catalogUrls = new List<string>();

            var catalogPages = catalogDriver.FindElementsByXPath("//li[@class=\"c-pager-page-number-list-item\"]/a");

            foreach (var catalogUrl in catalogPages)
            {
                catalogUrls.Add(catalogUrl.GetAttribute("href"));
            }

            return catalogUrls;
        }

        private static int GetHighestCatalogIndex(IEnumerable<string> catalogUrls)
        {
            const string urlPattern = "?pno=";

            var highestIndexUrl = catalogUrls.ToList()[^1];

            var index = highestIndexUrl.IndexOf(urlPattern, StringComparison.Ordinal);
            var highestIndexString = highestIndexUrl[(index + urlPattern.Length)..];

            int.TryParse(highestIndexString, out var highestIndex);
            return highestIndex;
        }

        private static string GetCatalogBaseUrl(IEnumerable<string> catalogUrls)
        {
            const string urlPattern = "?pno=";
            var enumerable = catalogUrls.ToList();
            var index = enumerable[0].IndexOf(urlPattern, StringComparison.Ordinal);
            var baseUrl = enumerable[0][..(index + urlPattern.Length)];

            return baseUrl;
        }

        /*
         *  Extract product ID from the url.
         *  <param name="url">The URL of the product</param>
         */
        private static int GetIdFromUrl(string url)
        {
            var id = 0;
            const string urlPattern = "goods/";

            if (!url.Contains(urlPattern)) return id;

            var index = url.IndexOf(urlPattern, StringComparison.Ordinal);
            var subString = url[(index + urlPattern.Length)..];
            var resultString = "";

            foreach (var c in subString)
            {
                if (c != '/')
                {
                    resultString += c;
                }
                else
                {
                    break;
                }
            }

            int.TryParse(resultString, out id);

            return id;
        }

        public static void DownloadImages(string url)
        {
            var productDriver = new ChromeDriver(ChromeDriverPath);
            productDriver.Navigate().GoToUrl(url);
            
            // Look for the unordered lists (p-goods-add-cart-list). Each <ul> belongs to a color variant of the product.
            var ulObjects = productDriver.FindElementsByClassName("p-goods-add-cart-list");
            // This is a collection of the different color variants of the item. These objects are clicked to show the actual image.
            var itemColors = productDriver.FindElements(By.ClassName("p-goods-thumbnail-list__item"));
            var colorIndexCounter = 0;
            
            foreach (var ulObject in ulObjects)
            {
                var availableSizes = new List<string>();
                var liObjects = ulObject.FindElements(By.XPath("./li[@class='p-goods-add-cart-list__item']"));
                foreach (var liObject in liObjects)
                {
                    var formObject = liObject.FindElements(By.XPath(
                        "./div[@class='cartbox p-goods-add-cart']/div[@class='cart p-goods-add-cart__action']/form"));
                    if (formObject.Count > 0)
                    {
                        availableSizes.Add(liObject.GetAttribute("data-size"));
                    }
                }

                if (availableSizes.Count > 0)
                {
                    Console.WriteLine("ul: " + ulObjects.Count);
                    itemColors[colorIndexCounter].Click();
                    Thread.Sleep(800);

                    var imageObject = productDriver.FindElementByXPath("//div[@id=\"photoMain\"]/img");
                    var imageSource = imageObject.GetAttribute("src");

                    var itemName = productDriver.FindElementByClassName("p-goods-information__heading")
                        .GetAttribute("innerHTML");

                    // itemName may contain illegal characters. Replace all illegal characters with _
                    itemName = Path.GetInvalidPathChars()
                        .Aggregate(itemName, (current, character) => current.Replace(character, '_'));

                    // Also replace all forward slashes. This causes a fetal error and is not filtered by the filter above.
                    itemName = itemName.Replace("/", "-");

                    foreach (var size in availableSizes)
                    {
                        itemName += size + "-";
                    }

                    WebClient.DownloadFile(imageSource,
                        DownloadsPath + @"\" + itemName + _globalDownloadCounter + ".jpg");
                    _globalDownloadCounter++;
                }

                colorIndexCounter++;
            }
            
            productDriver.Quit();
        }
    }
}