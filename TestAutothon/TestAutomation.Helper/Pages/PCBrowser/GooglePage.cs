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
                return GetWikiResult();
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

        private IWebElement GetWikiResult()
        {
            var wikiLinks = this.driver.FindElements(By.XPath($"//a[contains(@href, '.wikipedia.org/wiki/')]"));
            if(wikiLinks != null && wikiLinks.Any())
            {
                List<Tuple<IWebElement, double>> filtered = new List<Tuple<IWebElement, double>>();
                
                wikiLinks.ToList().ForEach(link => {
                    var href = link.GetAttribute("href");
                    //bool allWordsExist = true;
                    if(!string.IsNullOrEmpty(href))
                    {
                        filtered.Add(new Tuple<IWebElement, double>(link, CalculateSimilarity(href, this.searchKeyword)));
                    }                    
                });

                if(filtered.Any())
                {
                    filtered = filtered.OrderByDescending(f => f.Item2).ToList();
                    return filtered.FirstOrDefault().Item1;     
                }
            }

            return null;
        }

        private int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        private double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
    }
}
