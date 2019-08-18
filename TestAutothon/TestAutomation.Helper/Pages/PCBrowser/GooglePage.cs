using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestAutomation.Helper.Pages
{
    public class GooglePage
    {
        private readonly IWebDriver driver;
        private readonly string url = @"https://www.google.co.in";
        private string searchKeyword;

        public GooglePage(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        public IWebElement SearchBox
        {
            get
            {
                return this.driver.FindElement(By.Name("q"));
            }
        }

        //Returning this to achieve Fluent Page Object Pattern
        public GooglePage Navigate()
        {
            this.driver.Navigate().GoToUrl(this.url);
            this.driver.WaitForPageLoad();
            return this;
        }

        public GooglePage Search(string keyword)
        {
            this.searchKeyword = keyword;
            this.SearchBox.Clear();
            this.SearchBox.SendKeys(this.searchKeyword);
            this.SearchBox.Submit();
            return this;
        }

        public List<string> GetWikiResults(int noOfResults = 1)
        {
            var wikiLinks = this.driver.FindElements(By.XPath($"//a[contains(@href, '.wikipedia.org/wiki/')]"));
            if (wikiLinks != null && wikiLinks.Any())
            {
                List<Tuple<string, double>> filtered = new List<Tuple<string, double>>();

                wikiLinks.ToList().ForEach(link =>
                {
                    var href = link.GetAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        filtered.Add(new Tuple<string, double>(href, AutomationUtility.CalculateSimilarity(href, this.searchKeyword)));
                    }
                });

                if (filtered.Any())
                {
                    filtered = filtered.OrderByDescending(f => f.Item2).ToList();
                    return filtered.Take(noOfResults).Select(f => f.Item1).ToList();
                }
            }

            return null;
        }


    }
}
