using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestAutomation.Helper.Pages
{
    public class WikipediaPage
    {
        private readonly IWebDriver driver;

        public WikipediaPage(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        public IWebElement SingleDirector
        {
            get
            {
                return this.driver.FindElement(By.XPath($"//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a"));
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
                return this.driver.FindElement(By.XPath($".//ul/li/a[@class='external text'][contains(@href,'.imdb.com/title/')]"));
            }
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

        public void NavigateToIMDb()
        {
            this.IMDbLink.Click();
        }

        public void GetScreenshot(string path)
        {
            ITakesScreenshot screenshot = (ITakesScreenshot)this.driver;
            screenshot.GetScreenshot().SaveAsFile(path);
        }


    }
}
