using faabBot.GUI.EventArguments;
using faabBot.GUI.Helpers;
using faabBot.GUI.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V102.Debugger;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace faabBot.GUI.Controllers
{
    public class SeleniumController
    {
        private readonly string _url;
        private readonly ChromeDriver _driver;
        private readonly LogController _log;
        private readonly ProductController _productController;
        private readonly MainWindow _mainWindow;
        private readonly HttpClientController _httpClientController;

        public SeleniumController(string url, MainWindow mainWindow)
        {
            _url = url;
            _mainWindow = mainWindow;
            _log = new(mainWindow);
            _productController = new(mainWindow);

            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.DisableButtons();
                _mainWindow.mainProgressBar.Value = 0;
            });

            if (Globals.DevelopersMode)
            {
                _driver = new ChromeDriver();
            }
            else
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--headless");

                var chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;

                _driver = new ChromeDriver(chromeDriverService, chromeOptions);
            }

            _log.NewLogCreated += SeleniumController_LogMessage;
            _productController.NewProductAdded += SeleniumController_AddNewProduct;
            _httpClientController = new(mainWindow);
            ImplicitWait(Globals.ImplicitWaitInMilliseconds);


            _log.NewLogCreatedEvent("Session started, please wait...", DateTime.Now);

            _driver.Navigate().GoToUrl(_url);
        }

        #region Event Functions
        void SeleniumController_LogMessage(object? sender, LogEventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.LogInstance.Log(e.Message!, e.Created);
            });
        }

        void SeleniumController_AddNewProduct(object? sender, ProductEventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                if (e.Product != null)
                {
                    _mainWindow.ProductInstance.AddNewProduct(e.Product);
                }
            });
        }
        #endregion

        public void ScrapeAllProducts()
        {
            var subImageDirectory = string.Empty;
            GetAllProductUrls();

            var totalProductCount = _mainWindow.ProductInstance.ProductQueue.Count;
            var processedCount = 0d;

            _log.NewLogCreatedEvent(string.Format("found {0} products", _mainWindow.ProductInstance.ProductQueue.Count), DateTime.Now);

            if (_mainWindow.ProductInstance.ProductQueue.Any())
            {
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    _mainWindow.SetStatus(EnumTypes.StatusType.Status.DownloadingProducts);
                });

                subImageDirectory = DirectoryHelper.CreateSubImageDirectory(_mainWindow, _mainWindow.LogInstance);
            }

            while (_mainWindow.ProductInstance.ProductQueue.Any())
            {
                try
                {
                    _driver.Navigate().GoToUrl(_mainWindow.ProductInstance.ProductQueue.Last().Url);

                    if (_mainWindow.ProductInstance.ProductQueue.Last().Url != null && subImageDirectory != string.Empty)
                    {
                        DownloadVariations(subImageDirectory!);
                    }
                }
                catch (Exception e)
                {
                    _log.NewLogCreatedEvent(string.Format("{0}, Failed to navigate to product page", e.Message), DateTime.Now);
                }

                processedCount++;

                _mainWindow.Dispatcher.Invoke(() =>
                {
                    _mainWindow.UpdateProgressBar(processedCount / totalProductCount);
                    _mainWindow.ProductInstance.RemoveProduct(_mainWindow.ProductInstance.ProductQueue.Last());
                });
            }

            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.SetStatus(EnumTypes.StatusType.Status.NotStarted);
            });
        }

        #region Catalogue Functions
        public void GetAllProductUrls()
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.SetStatus(EnumTypes.StatusType.Status.FindingProducts);
            });

            var counter = 0;
            var firstCatalogueIndex = GetFirstCatalogueIndex();
            var lastCatalogueIndex = GetLastCatalogueIndex();
            var currentCatalogueIndex = GetCurrentCatalogueIndex();

            if (lastCatalogueIndex == 0)
            {
                if (currentCatalogueIndex != firstCatalogueIndex)
                {
                    ClickFirstCatalogueIndex();
                }

                GetProductsOnPage();
            }
            else
            {
                if (currentCatalogueIndex != firstCatalogueIndex)
                {
                    ClickFirstCatalogueIndex();
                }

                while (counter < lastCatalogueIndex)
                {
                    GetProductsOnPage();

                    GoToNextCataloguePage(lastCatalogueIndex);

                    counter++;
                }
            }
        }

        private void GoToNextCataloguePage(int maxCatalogueIndex)
        {
            var currentCatalogueIndex = GetCurrentCatalogueIndex();

            if (currentCatalogueIndex != maxCatalogueIndex)
            {
                try
                {
                    var nextPageBtn = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                        .Until(wd => wd.FindElement(By.ClassName("c-pager__next")));

                    nextPageBtn.Click();
                }
                catch (Exception e)
                {
                    MsgWindowHelper.ShowErrorMsgWindow(e.Message);
                }
            }
        }

        private void GetProductsOnPage()
        {
            try
            {
                var productHtmlElements = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                            .Until(wd => wd.FindElements(By.XPath("//ul[@id='searchResultList']" +
                                                                                                  "/li[@class='o-grid-catalog__item']" +
                                                                                                  "/div[@class='m-catalog-search']" +
                                                                                                  "/div[@class='c-catalog']" +
                                                                                                  "/div[@class='c-catalog-header']" +
                                                                                                  "/a[@class='c-catalog-header__link']")));

                foreach (var productHtmlElement in productHtmlElements)
                {
                    _productController.NewProductAddedEvent(new Models.Product()
                    {
                        Url = productHtmlElement.GetAttribute("href")
                    });
                }
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, failed to retrieve products on page", e.Message), DateTime.Now);
            }
        }

        private int GetCurrentCatalogueIndex()
        {
            try
            {
                var currentIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//li[@class='c-pager-page-number-list-item--current']")));
                var currentIndexItemInnerHtml = currentIndexItem.GetAttribute("innerHTML");

                if (int.TryParse(currentIndexItemInnerHtml, out var currentIndex))
                {
                    return currentIndex;
                }
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve current catalogue index", e.Message), DateTime.Now);
            }

            return 0;
        }

        private int GetLastCatalogueIndex()
        {
            IWebElement? lastIndexItem = null;
            try
            {
                lastIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[last()]/a")));
                var lastIdexItemHref = lastIndexItem.GetAttribute("href");

                var lastIndex = GetCatalogueIndexFromUrl(lastIdexItemHref);

                if (lastIndex.HasValue)
                {
                    return lastIndex.Value;
                }
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, trying to find last catalogue page button", e.Message), DateTime.Now);
            }

            if (lastIndexItem == null)
            {
                try
                {
                    lastIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                    .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[last()]")));
                    var lastIndexItemInnerHtml = lastIndexItem.GetAttribute("innerHTML");

                    if (int.TryParse(lastIndexItemInnerHtml, out var lastIndex))
                    {
                        _log.NewLogCreatedEvent(string.Format("Retrieved last catalogue index: {0}", lastIndex), DateTime.Now);
                        return lastIndex;
                    }
                }
                catch (Exception e)
                {
                    _log.NewLogCreatedEvent(string.Format("{0}, trying to find last catalogue page button", e.Message), DateTime.Now);
                }
            }

            return 0;
        }

        private void ClickFirstCatalogueIndex()
        {
            IWebElement? firstIndexItem = null;
            try
            {
                firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]/a")));

                firstIndexItem.Click();
                return;
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, trying to first catalogue page button", e.Message), DateTime.Now);
            }

            if (firstIndexItem == null)
            {
                try
                {
                    firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                    .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]")));

                    firstIndexItem.Click();
                    _log.NewLogCreatedEvent(string.Format("Retrieved first catalogue index: {0}", firstIndexItem.GetAttribute("innerHTML")), DateTime.Now);
                }
                catch (Exception e)
                {
                    _log.NewLogCreatedEvent(string.Format("{0}, trying to first catalogue page button", e.Message), DateTime.Now);
                }
            }
        }

        private int GetFirstCatalogueIndex()
        {
            IWebElement? firstIndexItem = null;

            try
            {
                firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]/a")));
                var firstIdexItemHref = firstIndexItem.GetAttribute("href");

                var firstIndex = GetCatalogueIndexFromUrl(firstIdexItemHref);

                if (firstIndex.HasValue)
                {
                    return firstIndex.Value;
                }
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, trying to first catalogue page button", e.Message), DateTime.Now);
            }

            if (firstIndexItem == null)
            {
                try
                {
                    firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                    .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]")));
                    var firstIndexItemInnerHtml = firstIndexItem.GetAttribute("innerHTML");

                    if (int.TryParse(firstIndexItemInnerHtml, out var firstIndex))
                    {
                        _log.NewLogCreatedEvent(string.Format("Retrieved first catalogue index, {0}", firstIndex), DateTime.Now);
                        return firstIndex;
                    }
                }
                catch (Exception e)
                {
                    _log.NewLogCreatedEvent(string.Format("{0}, trying to first catalogue page button", e.Message), DateTime.Now);
                }
            }

            return 0;
        }

        private static int? GetCatalogueIndexFromUrl(string url)
        {
            var pageNumberString = "pno=";
            var indexTest = url.LastIndexOf("pno=") + pageNumberString.Length;

            if (int.TryParse(url[indexTest..], out var index))
            {
                return index;
            }

            return null;
        }

        #endregion

        #region Product Functions

        private bool[] DetermineProductsToDownload()
        {
            try
            {
                var variationHtmlElements = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                            .Until(wd => wd.FindElements(By.XPath("//div[@class='blockMain']" +
                                                                                                  "/dl[@class='p-goods-information-action ']")));

                var variations = new bool[variationHtmlElements.Count];

                foreach (var variationHtmlElement in variationHtmlElements.Select((value, index) => new { index, value }))
                {

                    var sizes = variationHtmlElement.value.FindElements(By.XPath(".//dd[@class='p-goods-information-action__description']" +
                                                                                                     "/ul[@class='p-goods-add-cart-list']" +
                                                                                                     "/li[@class='p-goods-add-cart-list__item']"));
                    var availableSizes = sizes
                        .Where(s => !SizeNoStock(s))
                        .Select(s => s.GetAttribute("data-size"));

                    if (_mainWindow.SizesInstance.Sizes.Where(s => s != "ALL AVAILABLE SIZES").Any())
                    {
                        variations[variationHtmlElement.index] = _mainWindow.SizesInstance.Sizes.Intersect(availableSizes).Any();
                    }
                    else
                    {
                        variations[variationHtmlElement.index] = availableSizes.Any();
                    }
                }
                return variations;
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, failed to retrieve product variations", e.Message), DateTime.Now);
                return Array.Empty<bool>();
            }
        }

        private static bool SizeNoStock(IWebElement sizeElement)
        {
            try
            {
                sizeElement.FindElement(By.XPath(".//div[@class='cartbox p-goods-add-cart']" +
                                                 "/div[@class='stock p-goods-add-cart__meta noStock']"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DownloadVariations(string subImageDirectory)
        {
            var availableVariationArray = DetermineProductsToDownload();
            try
            {
                var variationHtmlElements = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                            .Until(wd => wd.FindElements(By.XPath("//ul[@id='photoThimb']" +
                                                                                                  "/li[contains(@class, 'p-goods-thumbnail-list__item')]" +
                                                                                                  "/div[contains(@class, 'p-goods-photograph')]" +
                                                                                                  "/span[contains(@class, 'p-goods-photograph__image-container')]" +
                                                                                                  "/img[contains(@class, 'p-goods-photograph__image')]")));

                foreach (var variationHtmlElement in variationHtmlElements.Select((value, index) => new { index, value }))
                {
                    if (variationHtmlElement.index >= availableVariationArray.Length)
                    {
                        break;
                    }

                    if (availableVariationArray[variationHtmlElement.index])
                    {
                        variationHtmlElement.value.Click();

                        //Dowload the image
                        var selectedProductImgSrc = GetSelectedProductImgSrc();
                        if (selectedProductImgSrc != null)
                        {
                            _httpClientController.DownloadImage(selectedProductImgSrc, GetProductName(), subImageDirectory, variationHtmlElement.index);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, failed to retrieve product thumbnails", e.Message), DateTime.Now);
            }
        }

        public string? GetSelectedProductImgSrc()
        {
            try
            {
                var mainProductImageElement = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                                            .Until(wd => wd
                                                                            .FindElement(By.Id("photoMain"))
                                                                            .FindElement(By.XPath(".//img")));

                return mainProductImageElement.GetAttribute("src");
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, failed to retrieve main product image", e.Message), DateTime.Now);
                return null;
            }
        }

        public string? GetProductName()
        {
            try
            {
                var mainProductImageElement = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                                                            .Until(wd => wd
                                                                            .FindElement(By.XPath("//h1[@class='p-goods-information__heading']")));

                return mainProductImageElement.GetAttribute("innerHTML");
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, failed to retrieve product name", e.Message), DateTime.Now);
                return null;
            }
        }

        #endregion

        public void CloseDriver()
        {
            _driver.Quit();
            _log.NewLogCreatedEvent(string.Format("Session closed..."), DateTime.Now);

            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.EnableButtons();
            });
        }

        private string GetBaseURL()
        {
            var currentUrl = _driver.Url;
            return currentUrl[..currentUrl.IndexOf("/shop")];
        }

        private WebDriverWait ExplicitWait(int durationInSeconds)
        {
            return new WebDriverWait(_driver, TimeSpan.FromSeconds(durationInSeconds));
        }

        private void ImplicitWait(int durationInSeconds)
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(durationInSeconds);
        }
    }
}
