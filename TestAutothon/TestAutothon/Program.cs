using System;
using System.Collections.Generic;
using System.IO;
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
            "The Shawshank Redemption",
            "NonExisting",
            "The Godfather",
            //"The Dark Knight",
            //"Pulp Fiction",
            //"Schindler's List",
            //"The Lord of the Rings: The Return of the King",//* multiple imdb links in wiki
            //"The Good,The Bad,The Ugly", //* Invalid wiki link in bing
            //"12 Angry Men",
            //"Inception",
            //"Forrest Gump",
            //"Fight Club",
            //"Star Wars:Episode V-The Empire Strikes Back", //Works, but confused with video game
            //"Goodfellas",
            //"The Matrix", //The Wachowskis(Combined brother names in wiki)
            //"One Flew Over The Cuckoo's Nest", //Novel vs Movie wiki page and special characters in director name
            //"Seven Samurai",
            //"Avengers: Infinity War", // "Anthony Russo\r\nJoe Russo" as single name in wiki
            //"Interstellar",
            //"Se7en"
        };

        public static AutomationBrowserType browserType;

        public static int maxNoOfThreads = 5;

        public static int noOfWikiLinks = 1;

        public static string outputDirectory = "Output";

        static void Main(string[] args)
        {
            string _iFilePath, _bType, _nThreads, _nWLinks;

            //args[0] => Input filename
            if (!string.IsNullOrEmpty(args[0]) && args[0] != "null")
            {
                _iFilePath = args[0];
                MovieNames = GetInputFromFile(_iFilePath);
            }

            //args[1] => Browser Type (gui, headless)
            if (!string.IsNullOrEmpty(args[1]) && args[1] != "null")
            {
                _bType = args[1];
                browserType = GetBrowserType(_bType);
            }

            //args[2] => Max no of threads
            if (!string.IsNullOrEmpty(args[2]) && args[2] != "null")
            {
                _nThreads = args[2];
                Int32.TryParse(_nThreads, out maxNoOfThreads);
            }

            //args[3] => Max no of wiki links
            if (!string.IsNullOrEmpty(args[3]) && args[3] != "null")
            {
                _nWLinks = args[3];
                Int32.TryParse(_nWLinks, out noOfWikiLinks);
            }

            //args[4] => Output folder
            if (!string.IsNullOrEmpty(args[4]) && args[4] != "null")
            {
                outputDirectory = args[4];
            }

            Console.WriteLine("Starting job");

            Console.WriteLine(args); 

            string reportDirectory = AutomationUtility.GetOutputDirectory(outputDirectory);
            AutomationHelper helper = new AutomationHelper();
            helper.GetWikiLinks(MovieNames, browserType, @"F:\Softwares\chromedriver_win32", reportDirectory, noOfWikiLinks);
            helper.Automate(MovieNames, browserType, @"F:\Softwares\chromedriver_win32", reportDirectory, maxNoOfThreads);

            Console.WriteLine("Completed job");
        }

        private static List<string> GetInputFromFile(string filePath)
        {
            List<string> inputs = null;

            if (File.Exists(filePath))
            {
                var data = File.ReadAllLines(filePath);
                inputs = data != null && data.Any() ? data.ToList() : null;
            }

            return inputs;
        }

        private static AutomationBrowserType GetBrowserType(string browserType)
        {
            switch (browserType.ToLower())
            {
                case "gui": return AutomationBrowserType.PCChromeBrowser;
                default: return AutomationBrowserType.PCHeadlessChromeBrowser;
            }
        }
    }
}
