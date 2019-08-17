using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestAutomation.Helper.Models;

namespace TestAutomation.Helper
{
    public class AutomationHelper
    {
        private List<MovieInfo> movieInfos;
        private object synchronizer = new object();
        private readonly AutomationBrowserType browserType;
        private readonly string driverPath;
        private string outputDirectory;

        public AutomationHelper(AutomationBrowserType browserType, string driverPath = "")
        {
            this.movieInfos = new List<MovieInfo>();

            this.browserType = browserType;
            this.driverPath = driverPath;
            this.outputDirectory = GetOutputDirectory();
        }

        public void Automate(List<string> testInputs)
        {
            Console.WriteLine($"UTC-{DateTime.UtcNow}: Automation started.");
            ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = 1 };

            Parallel.ForEach(testInputs, option, (input) =>
            {
                var automationDriver = new AutomationDriver();
                automationDriver.StartBrowser(browserType, 60, driverPath);
                AutomationFacade facade = new AutomationFacade(automationDriver.Browser, input);
                MovieInfo info = null;
                try
                {
                    info = facade.Run(this.outputDirectory);
                    Console.WriteLine($"The test {info.Passed} for {info.Name}");
                }
                catch (Exception)
                {
                    info = new MovieInfo();
                    info.Name = input;
                    info.ImdbLink = "Cannot find IMDb result";
                    info.WikiLink = "Cannot find Wikipedia result";
                }

                lock (synchronizer)
                {
                    this.movieInfos.Add(info);
                }

                automationDriver.StopBrowser();
            });

            var header = new List<string> { "Name", "WikiLink", "Wiki_Directors", "Wiki_Screenshot", "ImdbLink", "Imdb_Directors", "Wiki_Screenshot", "Result" };
            var reportData = new List<List<string>>();
            reportData.Add(header);
            reportData.AddRange(this.movieInfos.Select(info => info.ToArray().ToList()).ToList());

            var report = ReportGenerator.Generate(reportData, "Report Template\\TestAutothonReport.html");
            SaveReport(report);

            Console.WriteLine($"UTC-{DateTime.UtcNow}: Automation completed.");
        }

        private string GetOutputDirectory()
        {
            if (!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            var dir = $"Output\\Report -{ DateTime.UtcNow.ToString("MM-dd-yyyy h-mm-ss-ms")}";
            Directory.CreateDirectory(dir);
            return dir;
        }

        private void SaveReport(string report)
        {
            File.WriteAllText($"{this.outputDirectory}\\Report.html", report);
        }
    }
}
