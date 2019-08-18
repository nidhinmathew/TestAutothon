using OpenQA.Selenium;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestAutomation.Helper.Models;
using TestAutomation.Helper.Pages;

namespace TestAutomation.Helper
{
    public class AutomationFacade
    {
        private readonly IWebDriver driver;

        private GooglePage gPage;
        private WikipediaPage wPage;
        private IMDbPage iPage;
        private readonly int timeOutInSeconds;

        public AutomationFacade(IWebDriver _driver, int timeOutInSeconds = 30)
        {
            this.driver = _driver;
            this.timeOutInSeconds = timeOutInSeconds;
        }

        public GooglePage GPage
        {
            get
            {
                if (gPage == null)
                {
                    gPage = new GooglePage(this.driver);
                }

                return gPage;
            }
        }

        public WikipediaPage WPage
        {
            get
            {
                if (wPage == null)
                {
                    wPage = new WikipediaPage(this.driver, this.timeOutInSeconds);
                }

                return wPage;
            }
        }

        public IMDbPage IPage
        {
            get
            {
                if (iPage == null)
                {
                    iPage = new IMDbPage(this.driver, this.timeOutInSeconds);
                }

                return iPage;
            }
        }

        public MovieInfo Run(MovieInfo info, string outputDirectory, AutomationBrowserType browserType)
        {
            if(info != null && info.WikiLinks != null && info.WikiLinks.Any())
            {
                foreach(var link in info.WikiLinks)
                {
                    this.WPage.Navigate(link);
                    try
                    {
                        info.Directors_Wiki = this.WPage.GetDirector();
                        info.WikiLink = link;
                        info.WikiScreenShotPath = $"{AutomationUtility.ExcludeSymbols(info.Name)}_wiki.png";
                        this.WPage.GetScreenshot(Path.Combine(outputDirectory, info.WikiScreenShotPath));
                    }
                    catch { }
                    
                    if (!string.IsNullOrEmpty(info.Directors_Wiki))
                        break;
                };

                info.ImdbLink = this.WPage.GetIMDbLink(browserType);
                this.IPage.Navigate(info.ImdbLink);

                info.Directors_Imdb = this.IPage.GetDirector();

                info.ImdbScreenShotPath = $"{AutomationUtility.ExcludeSymbols(info.Name)}_imdb.png";
                this.IPage.GetScreenshot(Path.Combine(outputDirectory, info.ImdbScreenShotPath));                
            }

            return info;
        }

        public AutomationFacade NavigateToGoogle()
        {
            this.GPage.Navigate();
            return this;
        }

        public MovieInfo GetWikiLinks(string keyword, int noOfResults = 1)
        {
            var results = 
            this.GPage
                .Search(keyword)
                .GetWikiResults(noOfResults);

            return new MovieInfo()
            {
                Name = keyword,
                WikiLinks = results != null ? new List<string>(results) : null
            };
        }        
    }
}
