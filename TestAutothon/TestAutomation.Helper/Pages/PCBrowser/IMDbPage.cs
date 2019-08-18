using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using TestAutomation.Helper.Models;

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

        public IWebElement PCSingleDirector {
            get
            {
                return this.driver.FindElement(By.XPath($"//div[contains(@class,'plot_summary')]/div[contains(@class,'credit_summary_item')]/h4[contains(text(), 'Director:')]/following-sibling::a"), this.timeOutInSeconds);
            }
        }

        public IEnumerable<IWebElement> PCMultipleDirectors {
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

        public IWebElement MobDirector
        {
            get
            {
                return this.driver.FindElement(By.XPath($"//a[contains(@itemprop,'director')]/div/h3[contains(text(), 'Director')]/following-sibling::span"), this.timeOutInSeconds);
            }
        }
        
        public void Navigate(string url)
        {
            this.driver.Navigate().GoToUrl(url);
            this.driver.WaitForPageLoad();
        }

        public string GetDirector(AutomationBrowserType browserType)
        {
            string director = string.Empty;
            switch (browserType)
            {
                case AutomationBrowserType.MobileChromeBrowser:
                    {
                        director = this.MobDirector.Text.Replace("  ", "");
                        break;
                    }
                default:
                    {
                        if (this.PCMultipleDirectors != null && this.PCMultipleDirectors.Any())
                        {
                            director = string.Join(",", this.PCMultipleDirectors.ToList().Select(d => d.Text).ToList());
                        }
                        else
                        {
                            director = this.PCSingleDirector.Text;
                        }
                        break;
                    }
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
