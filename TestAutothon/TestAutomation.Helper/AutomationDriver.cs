using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using TestAutomation.Helper.Models;

namespace TestAutomation.Helper
{
    public class AutomationDriver
    {
        private WebDriverWait browserWait;

        private IWebDriver browser;

        public IWebDriver Browser
        {
            get
            {
                if (browser == null)
                {
                    throw new NullReferenceException("The WebDriver browser instance was not initialized. You should first call the method Start.");
                }
                return browser;
            }
            private set
            {
                browser = value;
            }
        }

        public WebDriverWait BrowserWait
        {
            get
            {
                if (browserWait == null || browser == null)
                {
                    throw new NullReferenceException("The WebDriver browser wait instance was not initialized. You should first call the method Start.");
                }
                return browserWait;
            }
            private set
            {
                browserWait = value;
            }
        }

        public void StartBrowser(AutomationBrowserType browserType = AutomationBrowserType.PCChromeBrowser, int defaultTimeOut = 1, string driverPath = "")
        {
            switch (browserType)
            {
                case AutomationBrowserType.PCChromeBrowser:
                    this.Browser = GetPCChromeDriver(driverPath);
                    break;
                case AutomationBrowserType.PCHeadlessChromeBrowser:
                    this.Browser = GetPCHeadlessChromeDriver(driverPath);
                    break;
                case AutomationBrowserType.MobileChromeBrowser:
                    this.Browser = GetMobileChromeDriver();
                    break;
                default:
                    break;
            }

            BrowserWait = new WebDriverWait(this.Browser, TimeSpan.FromMinutes(defaultTimeOut));
        }

        public void StopBrowser()
        {
            this.Browser.Quit();
            this.Browser = null;
            this.BrowserWait = null;
        }

        private IWebDriver GetPCChromeDriver(string driverPath = "")
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("start-maximized");
            return new ChromeDriver(driverPath, chromeOptions);
        }

        private IWebDriver GetPCHeadlessChromeDriver(string driverPath = "")
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("headless");
            return new ChromeDriver(driverPath, chromeOptions);
        }

        private IWebDriver GetMobileChromeDriver()
        {
            DesiredCapabilities testCapabilities = new DesiredCapabilities();
            testCapabilities.SetCapability(CapabilityType.BrowserName, "Chrome");
            testCapabilities.SetCapability("platformName", "Android");
            testCapabilities.SetCapability("deviceName", "Moto Z Play");
            return new AndroidDriver<AndroidElement>(new Uri("http://127.0.0.1:4723/wd/hub"), testCapabilities, TimeSpan.FromSeconds(180));
        }
    }
}
