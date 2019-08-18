using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAutomation.Helper;
using TestAutomation.Helper.Models;

namespace TestAutothon_Combined
{
    class Program
    {
        public static List<string> MovieNames = new List<string>()
        {
            "NonExisting",
            "The Shawshank Redemption",
            "The Godfather",
            "The Dark Knight",
            "Pulp Fiction",
            "Schindler's List",
            "The Lord of the Rings: The Return of the King",//* multiple imdb links in wiki
            "The Good,The Bad,The Ugly", //* Invalid wiki link in bing
            "12 Angry Men",
            "Inception",
            "Forrest Gump",
            "Fight Club",
            "Star Wars:Episode V-The Empire Strikes Back", //Works, but confused with video game
            "Goodfellas",
            "The Matrix", //The Wachowskis(Combined brother names in wiki)
            "One Flew Over The Cuckoo's Nest", //Novel vs Movie wiki page and special characters in director name
            "Seven Samurai",
            "Avengers: Infinity War", // "Anthony Russo\r\nJoe Russo" as single name in wiki
            "Interstellar",
            "Se7en"
        };

        static void Main(string[] args)
        {
            string outputDirectory = AutomationUtility.GetOutputDirectory();
            AutomationHelper helper = new AutomationHelper(AutomationBrowserType.PCChromeBrowser, @"F:\Softwares\chromedriver_win32");
            helper.GetWikiLinks(MovieNames, outputDirectory);
            //helper.Automate(MovieNames);


            AutomationHelper mobHelper = new AutomationHelper(AutomationBrowserType.MobileChromeBrowser, @"F:\Softwares\chromedriver_win32");

            mobHelper.Automate(MovieNames.Take(2).ToList(), outputDirectory, 1);
        }
    }
}
