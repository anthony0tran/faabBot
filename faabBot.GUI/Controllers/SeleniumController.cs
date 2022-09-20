using faabBot.GUI.EventArguments;
using faabBot.GUI.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V102.Debugger;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace faabBot.GUI.Controllers
{
    public class SeleniumController
    {
        private readonly string _url;
        private readonly ChromeDriver _driver;
        private readonly LogController _log;
        private readonly ProductController _productController;
        private readonly MainWindow _mainWindow;

        public SeleniumController(string url, MainWindow mainWindow)
        {
            _url = url;
            _mainWindow = mainWindow;
            _log = new(mainWindow);
            _productController = new(mainWindow);
            _driver = new ChromeDriver();
            _log.NewLogCreated += SeleniumController_LogMessage;
            _productController.NewProductAdded += SeleniumController_AddNewProduct;

            _driver.Navigate().GoToUrl(_url);
        }

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

        public void GetAllProductUrls()
        {
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

                    ImplicitWait(Globals.ImplicitWaitInSeconds);

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
                _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve last catalogue index", e.Message), DateTime.Now);
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
                    _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve last catalogue index", e.Message), DateTime.Now);
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

                ImplicitWait(Globals.ImplicitWaitInSeconds);

                firstIndexItem.Click();
                return;
            }
            catch (Exception e)
            {
                _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve first catalogue index", e.Message), DateTime.Now);
            }

            if (firstIndexItem == null)
            {
                try
                {
                    firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                    .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]")));

                    ImplicitWait(Globals.ImplicitWaitInSeconds);

                    firstIndexItem.Click();
                    _log.NewLogCreatedEvent(string.Format("Retrieved first catalogue index: {0}", firstIndexItem.GetAttribute("innerHTML")), DateTime.Now);
                }
                catch (Exception e)
                {
                    _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve first catalogue index", e.Message), DateTime.Now);
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
                _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve first catalogue index", e.Message), DateTime.Now);
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
                    _log.NewLogCreatedEvent(string.Format("{0}, cannot retrieve first catalogue index", e.Message), DateTime.Now);
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

        public void CloseDriver()
        {
            _driver.Quit();
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
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(durationInSeconds);
        }
    }
}
