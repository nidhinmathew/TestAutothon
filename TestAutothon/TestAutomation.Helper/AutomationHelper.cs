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
        public void GetWikiLinks(List<string> testInputs, AutomationBrowserType browserType, string outputDirectory, int noOfResults = 2)
        {
            Console.WriteLine($"UTC-{DateTime.UtcNow}: Retrieving Wiki Links.");
            var googleDriver = new AutomationDriver();
            googleDriver.StartBrowser(browserType, 3);

            AutomationFacade facade = new AutomationFacade(googleDriver.Browser);
            if(testInputs != null && testInputs.Any())
            {
                facade.NavigateToGoogle();
                testInputs.ForEach(input =>
                {
                    var info = facade.GetWikiLinks(input, noOfResults);
                    AutomationUtility.Serialize<MovieInfo>(info, $"{outputDirectory}\\{AutomationUtility.ExcludeSymbols(info.Name)}.xml");
                });
            }

            googleDriver.StopBrowser();
        }

        public void Automate(List<string> testInputs, AutomationBrowserType browserType, string outputDirectory, int degreeOfParallelism = 5)
        {
            Console.WriteLine($"UTC-{DateTime.UtcNow}: Automation started.");
            if (browserType == AutomationBrowserType.PCChromeBrowser)
            {
                List<Task> tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => {
                       RunTests(testInputs.Take(1).ToList(), AutomationBrowserType.MobileChromeBrowser, outputDirectory, 1);
                    }),
                    Task.Factory.StartNew(() => {
                        RunTests(testInputs.Skip(1).ToList(), AutomationBrowserType.PCChromeBrowser, outputDirectory, degreeOfParallelism);
                    })
                };

                Task.WaitAll(tasks.ToArray());
            }
            else
            {
                RunTests(testInputs, AutomationBrowserType.PCHeadlessChromeBrowser, outputDirectory, degreeOfParallelism);
            }            

            var reportData = GetReportData(testInputs, outputDirectory);

            var report = ReportGenerator.Generate(reportData, "Report Template\\TestAutothonReport.html");
            AutomationUtility.SaveReport(report, $"{outputDirectory}\\Report.html");

            Console.WriteLine($"UTC-{DateTime.UtcNow}: Automation completed.");
        }


        private void RunTests(List<string> testInputs, AutomationBrowserType browserType, string outputDirectory, int degreeOfParallelism = 5)
        {
            ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = browserType != AutomationBrowserType.MobileChromeBrowser ? degreeOfParallelism : 1 };

            Parallel.ForEach(testInputs, option, (input) =>
            {
                var info = AutomationUtility.Deserialize<MovieInfo>($"{outputDirectory}\\{AutomationUtility.ExcludeSymbols(input)}.xml");
                if (info != null)
                {
                    var automationDriver = new AutomationDriver();
                    automationDriver.StartBrowser(browserType, 3);
                    AutomationFacade facade = new AutomationFacade(automationDriver.Browser, 120);

                    try
                    {
                        info = facade.Run(info, outputDirectory, browserType);

                        if (string.IsNullOrEmpty(info.Directors_Wiki))
                            info.Directors_Wiki = "Cannot find Wikipedia result";

                        if (string.IsNullOrEmpty(info.Directors_Imdb))
                            info.Directors_Imdb = "Cannot find IMDb result";

                        Console.WriteLine($"The test {info.Passed} for {info.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        info = new MovieInfo();
                        info.Name = input;
                        info.Directors_Imdb = "Cannot find IMDb result";
                        info.Directors_Wiki = "Cannot find Wikipedia result";
                    }

                    AutomationUtility.Serialize<MovieInfo>(info, $"{outputDirectory}\\{AutomationUtility.ExcludeSymbols(info.Name)}.xml");
                    automationDriver.StopBrowser();
                }
            });
        }
        private List<List<string>> GetReportData(List<string> testInputs, string outputDirectory)
        {
            var header = new List<string> { "Name", "WikiLink", "Wiki_Directors", "Wiki_Screenshot", "ImdbLink", "Imdb_Directors", "Wiki_Screenshot", "Result" };
            var reportData = new List<List<string>>();
            reportData.Add(header);
             
            testInputs.ForEach(input => {
                var info = AutomationUtility.Deserialize<MovieInfo>($"{outputDirectory}\\{AutomationUtility.ExcludeSymbols(input)}.xml");
                if(info != null)
                    reportData.Add(info.ToArray().ToList());
            });

            return reportData;
        }

        
    }
}
