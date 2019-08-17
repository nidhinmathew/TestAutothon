using OpenQA.Selenium;
using System.IO;
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
        private string keyWord;

        public AutomationFacade(IWebDriver _driver, string _keyword)
        {
            this.driver = _driver;
            this.keyWord = _keyword;
        }

        public GooglePage GPage
        {
            get
            {
                if (gPage == null)
                {
                    gPage = new GooglePage(this.driver, this.keyWord);
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
                    wPage = new WikipediaPage(this.driver);
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
                    iPage = new IMDbPage(this.driver);
                }

                return iPage;
            }
        }

        public MovieInfo Run(string outputDirectory)
        {
            MovieInfo info = new MovieInfo()
            {
                Name = this.keyWord
            };

            this.GPage
                .Navigate()
                .Search()
                .NavigateToWiki();

            info.Directors_Wiki = this.WPage.GetDirector();
            info.WikiScreenShotPath = Path.Combine(outputDirectory, $"{this.keyWord}_wiki.png");
            this.WPage.GetScreenshot(info.WikiScreenShotPath);

            this.WPage.NavigateToIMDb();

            info.Directors_Imdb = this.IPage.GetDirector();
            info.ImdbScreenShotPath = Path.Combine(outputDirectory, $"{this.keyWord}_imdb.png");
            this.IPage.GetScreenshot(info.ImdbScreenShotPath);
            return info;
        }
    }
}
