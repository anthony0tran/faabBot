using Microsoft.VisualBasic;
using Newtonsoft.Json.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace faabBot.GUI.Controllers
{
    internal class SeleniumController
    {
        
    }

    public class SeleniumInstance
    {
        private readonly string _url;
        private readonly ChromeDriver _driver;

        public SeleniumInstance(string url)
        {
            _url = url;
            _driver = new ChromeDriver();
        }


    }
}
