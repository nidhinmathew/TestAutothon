using OpenQA.Selenium;

namespace TestAutomation.Helper.Pages
{
    public class GooglePage
    {
        private readonly IWebDriver driver;
        private readonly string url = @"https://www.google.co.in";
        private readonly string searchKeyword;

        public GooglePage(IWebDriver _driver, string keyword)
        {
            this.driver = _driver;
            this.searchKeyword = keyword;
        }

        public IWebElement SearchBox {
            get
            {
                return this.driver.FindElement(By.Name("q"));
            }
        }

        public IWebElement WikiResult
        {
            get
            {
                return this.driver.FindElement(By.XPath($".//a[contains(@href,'.wikipedia.org/wiki/')]/h3/div[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{this.searchKeyword.ToLower()}')]/ancestor::a"));
            }
        }

        //Returning this to achieve Fluent Page Object Pattern
        public GooglePage Navigate() 
        {
            this.driver.Navigate().GoToUrl(this.url);
            return this;
        }

        public GooglePage Search()
        {
            this.SearchBox.Clear();
            this.SearchBox.SendKeys(this.searchKeyword);
            this.SearchBox.Submit();
            return this;
        }

        public GooglePage NavigateToWiki()
        {
            this.WikiResult.Click();
            return this;
        }
    }
}
