using faabBot.GUI.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace faabBot.GUI.Controllers
{
    public class SeleniumController
    {
        private readonly string _url;
        private readonly ChromeDriver _driver;

        public SeleniumController(string url)
        {
            _url = url;
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl(_url);
        }

        public void GoToNextCataloguePage()
        {
            var maxCatalogueIndex = GetMaxCatalogueIndex();

            var counter = 0;
            while (counter < maxCatalogueIndex)
            {
                if (counter != maxCatalogueIndex - 1)
                {
                    try
                    {
                        var nextPageBtn = ExplicitWait(Globals.ExplicitWaitInSeconds)
                                            .Until(wd => wd.FindElement(By.ClassName("c-pager__next")));

                        ImplicitWait(Globals.ImplicitWaitSeconds);

                        nextPageBtn.Click();
                    }
                    catch (Exception e)
                    {
                        MsgWindowHelper.ShowErrorMsgWindow(e.Message);
                    }
                }

                counter++;
            }
        }

        public int GetMaxCatalogueIndex()
        {
            var lastIndexItem = ExplicitWait(Globals.ExplicitWaitInSeconds)
                .Until(wd => wd.FindElement(By.XPath("//ol[@class='c-pager-page-number-list']/li[last()]/a")));
            var lastIdexItemInnerHtml = lastIndexItem.GetAttribute("innerHTML");

            if (int.TryParse(lastIdexItemInnerHtml, out var lastIndex))
            {
                return lastIndex;
            }

            return 0;
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
