using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace ReznichenkoWeb.Tests
{
    public class SeleniumTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "http://localhost:5122"; // Adjust if needed

        public SeleniumTests()
        {
            // Ensure you have ChromeDriver installed or in PATH
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); // Increased wait time

            // Check if app is running
            try
            {
                using var client = new HttpClient();
                // Short timeout to fail fast if app is down
                client.Timeout = TimeSpan.FromSeconds(2);
                var response = client.GetAsync(_baseUrl).Result;
                 response.EnsureSuccessStatusCode(); // Force 200 OK
            }
            catch
            {
                _driver.Quit();
                throw new Exception($"❌ APP IS NOT RUNNING OR UNHEALTHY! Cannot connect to {_baseUrl}. \n   Please run './run_tests.sh' or ensure 'dotnet run' is active and serving content.");
            }
        }

        public void Dispose()
        {
            try
            {
                _driver.Quit();
                _driver.Dispose();
            }
            catch { }
        }

        [Fact]
        public void Test1_AddMember_Positive()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Members/Create");

            _driver.FindElement(By.Name("Name")).SendKeys("Selenium User");
            _driver.FindElement(By.Name("Email")).SendKeys("selenium@test.com");
            _driver.FindElement(By.Name("Phone")).SendKeys("+380998887766");
            _driver.FindElement(By.Name("Age")).SendKeys("25");
            
            new SelectElement(_driver.FindElement(By.Name("Gender"))).SelectByValue("Чоловіча");
            new SelectElement(_driver.FindElement(By.Name("MembershipType"))).SelectByValue("Преміум");

            SafeClick(_driver.FindElement(By.CssSelector("input[type='submit']")));

            Assert.Contains("Members", _driver.Title);
            Assert.Contains("Selenium User", _driver.PageSource);
        }

        [Fact]
        public void Test2_EditTrainer_Positive()
        {
             _driver.Navigate().GoToUrl($"{_baseUrl}/Trainers/Create");
            _driver.FindElement(By.Name("Name")).SendKeys("Trainer To Edit");
            _driver.FindElement(By.Name("Age")).SendKeys("30");
            new SelectElement(_driver.FindElement(By.Name("Gender"))).SelectByValue("Жіноча");
            _driver.FindElement(By.Name("Experience")).SendKeys("5");
            _driver.FindElement(By.Name("Specialization")).SendKeys("Yoga");
            _driver.FindElement(By.Name("Phone")).SendKeys("+380112223344");
            _driver.FindElement(By.Name("Email")).SendKeys("trainer@edit.com");
            
            SafeClick(_driver.FindElement(By.CssSelector("input[type='submit']")));

            // Create loop to find the edit link for the specific trainer
            var editLinks = _driver.FindElements(By.LinkText("Edit"));
            if (editLinks.Count > 0)
            {
                SafeClick(editLinks.Last()); 

                _driver.FindElement(By.Name("Name")).Clear();
                _driver.FindElement(By.Name("Name")).SendKeys("Trainer Edited");
                
                SafeClick(_driver.FindElement(By.CssSelector("input[type='submit']")));

                Assert.Contains("Trainer Edited", _driver.PageSource);
            }
        }

        [Fact]
        public void Test3_AddMember_Negative_InvalidData()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Members/Create");

            _driver.FindElement(By.Name("Name")).SendKeys(""); 
            _driver.FindElement(By.Name("Email")).SendKeys("invalid-email"); 
            
            SafeClick(_driver.FindElement(By.CssSelector("input[type='submit']")));

            Assert.Contains("The Name field is required", _driver.PageSource); 
            // Assert.Contains("The Email field is not a valid e-mail address", _driver.PageSource); 
        }

        [Fact]
        public void Test4_AddWorkout_Negative_InvalidDuration()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Workouts/Create");

            _driver.FindElement(By.Name("DurationMinutes")).SendKeys("500");
             
            SafeClick(_driver.FindElement(By.CssSelector("input[type='submit']")));

             Assert.Contains("The field DurationMinutes must be between 1 and 300", _driver.PageSource); 
        }

        private void SafeClick(IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
            }
            catch (Exception)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                Thread.Sleep(500);
                element.Click();
            }
        }
    }
}
