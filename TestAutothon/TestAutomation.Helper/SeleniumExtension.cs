using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAutomation.Helper
{
    public static class SeleniumExtension
    {
        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, string xPath, int timeOutInSeconds = 30)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElements(By.XPath(xPath)));

        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, By by, int timeOutInSeconds = 30)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElements(by));

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeOutInSeconds = 30)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElement(by));

        public static WebDriverWait Wait(this IWebDriver driver, int timeOutInSeconds = 30)
            => new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds));

        public static void WaitForPageLoad(this IWebDriver driver, uint timeOutInSeconds = 30)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(timeOutInSeconds);
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("driver", "Driver must support javascript execution");

            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript(
                    "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

    }
}
