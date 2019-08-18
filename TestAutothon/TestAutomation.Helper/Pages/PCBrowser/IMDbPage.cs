using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestAutomation.Helper.Pages
{
    public class IMDbPage
    {
        private readonly IWebDriver driver;
        private readonly int timeOutInSeconds;

        public IMDbPage(IWebDriver _driver, int timeOutInSeconds = 30)
        {
            this.driver = _driver;
            this.timeOutInSeconds = timeOutInSeconds;
        }

        public IWebElement SingleDirector {
            get
            {
                return this.driver.FindElement(By.XPath($"//div[contains(@class,'plot_summary')]/div[contains(@class,'credit_summary_item')]/h4[contains(text(), 'Director:')]/following-sibling::a"), this.timeOutInSeconds);
            }
        }

        public IEnumerable<IWebElement> MultipleDirectors {
            get
            {
                try
                {
                    return this.driver.FindElements(By.XPath($"//div[contains(@class,'plot_summary')]/div[contains(@class,'credit_summary_item')]/h4[contains(text(), 'Directors:')]/following-sibling::a"));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Navigate(string url)
        {
            this.driver.Navigate().GoToUrl(url);
            this.driver.WaitForPageLoad();
        }

        public string GetDirector()
        {
            string director = string.Empty;
            if (this.MultipleDirectors != null && this.MultipleDirectors.Any())
            {
                director = string.Join(",", this.MultipleDirectors.ToList().Select(d => d.Text).ToList());
            }
            else
            {
                director = this.SingleDirector.Text;
            }
            return director;
        }

        public void GetScreenshot(string path)
        {
            ITakesScreenshot screenshot = (ITakesScreenshot)this.driver;
            screenshot.GetScreenshot().SaveAsFile(path);
        }
    }
}
