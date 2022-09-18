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
        private HashSet<string> _allProductUrls;
        private LogMessageHelper _log;

        public SeleniumController(string url, MainWindow mainWindow)
        {
            _url = url;
            _driver = new ChromeDriver();
            _allProductUrls = new();
            _log = new(mainWindow);

            _driver.Navigate().GoToUrl(_url);
        }

        public HashSet<string> GetAllProductUrls()
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

                _allProductUrls.UnionWith(GetProductsOnPage());
            }
            else
            {
                if (currentCatalogueIndex != firstCatalogueIndex)
                {
                    ClickFirstCatalogueIndex();
                    currentCatalogueIndex = GetCurrentCatalogueIndex();
                }

                while (counter < lastCatalogueIndex)
                {
                    _allProductUrls.UnionWith(GetProductsOnPage());

                    GoToNextCataloguePage(lastCatalogueIndex);

                    counter++;
                }
            }

            var count = _allProductUrls.Count;
            _log.Log(string.Format("Found {0} products", count));

            return _allProductUrls;
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

        private HashSet<string> GetProductsOnPage()
        {
            var productUrls = new HashSet<string>();

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
                    productUrls.Add(productHtmlElement.GetAttribute("href"));
                }
            }
            catch (Exception e)
            {
                _log.Log(e.Message);
            }

            return productUrls;
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
                _log.Log(string.Format("{0}, cannot retrieve current catalogue index", e.Message));
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
                _log.Log(string.Format("{0}, cannot retrieve last catalogue index", e.Message));
            }

            if (lastIndexItem == null)
            {
                lastIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[last()]")));
                var lastIndexItemInnerHtml = lastIndexItem.GetAttribute("innerHTML");

                if (int.TryParse(lastIndexItemInnerHtml, out var lastIndex))
                {
                    _log.Log(string.Format("Retrieved last catalogue index: {0}", lastIndex));
                    return lastIndex;
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
                _log.Log(string.Format("{0}, cannot retrieve first catalogue index", e.Message));
            }

            if (firstIndexItem == null)
            {
                try
                {
                    firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                    .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]")));

                    ImplicitWait(Globals.ImplicitWaitInSeconds);

                    firstIndexItem.Click();
                    _log.Log(string.Format("Retrieved first catalogue index: {0}", firstIndexItem.GetAttribute("innerHTML")));
                }
                catch (Exception e)
                {
                    _log.Log(string.Format("{0}, cannot retrieve first catalogue index", e.Message));
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
                _log.Log(string.Format("{0}, cannot retrieve first catalogue index", e.Message));
            }

            if (firstIndexItem == null)
            {
                firstIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[1]")));
                var firstIndexItemInnerHtml = firstIndexItem.GetAttribute("innerHTML");

                if (int.TryParse(firstIndexItemInnerHtml, out var firstIndex))
                {
                    _log.Log(string.Format("Retrieved first catalogue index, {0}", firstIndex));
                    return firstIndex;
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
