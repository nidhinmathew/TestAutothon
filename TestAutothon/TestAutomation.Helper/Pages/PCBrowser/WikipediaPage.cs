using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using TestAutomation.Helper.Models;

namespace TestAutomation.Helper.Pages
{
    public class WikipediaPage
    {
        private readonly IWebDriver driver;
        private readonly int timeOutInSeconds;

        public WikipediaPage(IWebDriver _driver, int timeOutInSeconds = 30)
        {
            this.driver = _driver;
            this.timeOutInSeconds = timeOutInSeconds;
        }

        public IWebElement SingleDirector
        {
            get
            {
                return this.driver.FindElement(By.XPath($"//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a"), this.timeOutInSeconds);
            }
        }

        public IEnumerable<IWebElement> MultipleDirectors
        {
            get
            {
                try
                {
                    return this.driver.FindElements(By.XPath($"//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a/div/ul/li"));
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public IWebElement IMDbLink
        {
            get
            {
                return this.driver.FindElement(By.XPath($"//ul/li/a[@class='external text'][contains(@href,'.imdb.com/title/')]"), this.timeOutInSeconds);
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
        
        public string GetIMDbLink(AutomationBrowserType browserType)
        {
            string link = string.Empty;
            switch(browserType)
            {
                case AutomationBrowserType.MobileChromeBrowser: link = this.IMDbLink.GetProperty("href"); break;
                default: link = this.IMDbLink.GetAttribute("href"); break;
            }

            return link;
        }

        public void GetScreenshot(string path)
        {
            ITakesScreenshot screenshot = (ITakesScreenshot)this.driver;
            screenshot.GetScreenshot().SaveAsFile(path);
        }


    }
}
