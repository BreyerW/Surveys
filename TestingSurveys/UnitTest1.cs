using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Net.Http;
using System.Threading;
using Surveys;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace TestingSurveys
{
    public class Tests
    {
        private Factory _server;
        private HttpClient _client;
        private IWebDriver _driver;

        /// <summary>
        /// Ustanowienie po³¹czenia z przegl¹dark¹ i bazy danych w pamiêci.
        /// Tylko raz na test
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            // Arrange
            Startup.testing = true;
            _server = new Factory();
            _client = _server.CreateClient();
            var options = new FirefoxOptions();
            options.AcceptInsecureCertificates = true;
            _driver = new FirefoxDriver(options);//@"C:\Users\damia\Downloads\edgedriver_win64"
            Factory.context.Roles.Add(new Surveys.Model.Role() { RoleName = "Owner" });
            Factory.context.Roles.Add(new Surveys.Model.Role() { RoleName = "Admin" });
            Factory.context.Roles.Add(new Surveys.Model.Role() { RoleName = "User" });
            Factory.context.SaveChanges();
            CreateTestUser();
        }
        /// <summary>
        /// Zamkniêcie ca³ego testu
        /// </summary>
        [OneTimeTearDown]
        public void Stop()
        {
            Startup.testing = false;
            _client.Dispose();
            _server.Dispose();
            // _driver.Close();
        }
        /// <summary>
        /// Test pustego formularza rejestracyjnego
        /// </summary>
        [Test]
        public void InvalidRegisterData()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Create");
            _driver.FindElement(By.Id("registerButton")).Submit();
            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }
        /// <summary>
        /// Test niezgadzaj¹cych siê hase³ przy rejestracji
        /// </summary>
        [Test]
        public void InvalidConfirmPassword()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Create");
            _driver.FindElement(By.Id("Username")).SendKeys("test");
            _driver.FindElement(By.Id("Password")).SendKeys("test");
            _driver.FindElement(By.Id("ConfirmPassword")).SendKeys("test1");
            _driver.FindElement(By.Id("registerButton")).Submit();
            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }

        /// <summary>
        /// SprawdŸ czy u¿ytkownik ju¿ istnieje
        /// </summary>
        [Test]
        public void UserAlreadyExists()
        {
            CreateTestUser();
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }
        private void CreateTestUser()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Create");

            _driver.FindElement(By.Id("Username")).SendKeys("test");
            _driver.FindElement(By.Id("Password")).SendKeys("test");
            _driver.FindElement(By.Id("ConfirmPassword")).SendKeys("test");
            _driver.FindElement(By.Id("registerButton")).Submit();
            Thread.Sleep(1000);
        }
        private void LogInTestUser()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Login");
            _driver.FindElement(By.Id("Username")).SendKeys("test");
            _driver.FindElement(By.Id("Password")).SendKeys("test");
            _driver.FindElement(By.Id("loginButton")).Submit();
            Thread.Sleep(1000);
        }
        /// <summary>
        /// Test rejestracji i logowania wiêcej ni¿ jednego u¿ytkownika
        /// </summary>
        [Test]
        public void CreateAndLogInAnotherTestUser()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Create");

            _driver.FindElement(By.Id("Username")).SendKeys("test1");
            _driver.FindElement(By.Id("Password")).SendKeys("test1");
            _driver.FindElement(By.Id("ConfirmPassword")).SendKeys("test1");
            _driver.FindElement(By.Id("registerButton")).Submit();
            Thread.Sleep(1000);
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Login");
            _driver.FindElement(By.Id("Username")).SendKeys("test1");
            _driver.FindElement(By.Id("Password")).SendKeys("test1");
            _driver.FindElement(By.Id("loginButton")).Submit();
            Thread.Sleep(1000);
            Assert.False(_driver.Url == $"{_server.RootUri}/Account/Login");
        }
        /// <summary>
        /// Test pustego formularza logowania
        /// </summary>
        [Test]
        public void InvalidLoginData()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Login");
            _driver.FindElement(By.Id("loginButton")).Submit();
            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }
        /// <summary>
        /// Test b³êdnego has³a
        /// </summary>
        [Test]
        public void InvalidLoginPassword()
        {
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Account/Login");
            _driver.FindElement(By.Id("Username")).SendKeys("test");
            _driver.FindElement(By.Id("Password")).SendKeys("test2");
            _driver.FindElement(By.Id("loginButton")).Submit();
            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }

        /// <summary>
        /// Test poprawnego logowania
        /// </summary>
        [Test]
        public void ValidLogin()
        {
            LogInTestUser();
            Assert.False(_driver.Url == $"{_server.RootUri}/Account/Login");
        }
        /// <summary>
        /// Test pustego formularza tworzenia ankiety
        /// </summary>
        [Test]
        public void InvalidSurveyData()
        {
            LogInTestUser();
            Thread.Sleep(1000);
            _driver.FindElement(By.Id("createSurveyButton")).Click();
            _driver.FindElement(By.Name("create")).Submit();
            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }
        /// <summary>
        /// Test tworzenia ankiety zawieraj¹cej pytanie zamkniête, wielokrotnego wyboru i otwarte
        /// </summary>
        [Test]
        public void CreateSampleSurvey()
        {
            LogInTestUser();
            Thread.Sleep(1000);
            if (Factory.context.Surveys.Any()) return;
            _driver.FindElement(By.Id("createSurveyButton")).Click();
            //fill basic data about survey
            _driver.FindElement(By.Name("Topic")).SendKeys("Spis ludnoœci");
            _driver.FindElement(By.Name("MinBirthYear")).FindElement(By.CssSelector("option[value='2000']")).Click();
            _driver.FindElement(By.Name("Sex")).FindElement(By.CssSelector("option[value='Female']")).Click();

            //create 3 new questions
            _driver.FindElement(By.Name("new")).Submit();
            _driver.FindElement(By.Name("new")).Submit();
            _driver.FindElement(By.Name("new")).Submit();

            //provide sentences for 3 questions
            _driver.FindElement(By.Name("q[0]")).SendKeys("Gdzie mieszkasz");
            _driver.FindElement(By.Name("q[1]")).SendKeys("Twoja p³eæ");
            _driver.FindElement(By.Name("q[2]")).SendKeys("Powiedz coœ o sobie");

            //configure options for questions
            _driver.FindElement(By.Name("config[0][0]")).Click();
            _driver.FindElement(By.Name("config[0][1]")).Click();

            _driver.FindElement(By.Name("config[1][0]")).Click();

            //create 3 and 2 extra answers respectively for first and 2nd questions
            _driver.FindElement(By.CssSelector("input[type=submit][value$='0']")).Submit();
            _driver.FindElement(By.CssSelector("input[type=submit][value$='0']")).Submit();
            _driver.FindElement(By.CssSelector("input[type=submit][value$='0']")).Submit();

            _driver.FindElement(By.CssSelector("input[type=submit][value$='1']")).Submit();
            _driver.FindElement(By.CssSelector("input[type=submit][value$='1']")).Submit();

            //provide sentences for answers
            _driver.FindElement(By.Name("a[0][0]")).SendKeys("Kraków");
            _driver.FindElement(By.Name("a[0][1]")).SendKeys("Warszawa");
            _driver.FindElement(By.Name("a[0][2]")).SendKeys("Szczecin");
            _driver.FindElement(By.Name("a[0][3]")).SendKeys("W ¿adnym z powy¿szych");

            _driver.FindElement(By.Name("a[1][0]")).SendKeys("Mê¿czyzna");
            _driver.FindElement(By.Name("a[1][1]")).SendKeys("Kobieta");
            _driver.FindElement(By.Name("a[1][2]")).SendKeys("Inne");

            //submit new survey
            _driver.FindElement(By.Name("create")).Submit();
            Thread.Sleep(1000);
            Assert.True(_driver.Url == $"{_server.RootUri}/Surveys");

        }
        /// <summary>
        /// Test pustego formularza do wype³niania ankiet 
        /// </summary>
        [Test]
        public void InvalidCompleteSampleSurvey()
        {
            CreateSampleSurvey();
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Surveys/IndexUncompletedSurveys");
            _driver.FindElement(By.CssSelector("a[href^='/Surveys/Complete']")).Click();
            _driver.FindElement(By.Name("complete")).Submit();

            Thread.Sleep(1000);
            var errors = _driver.FindElements(By.ClassName("field-validation-error"));
            Assert.NotNull(errors);
            foreach (var error in errors)
                Assert.NotNull(error.Text);
        }
        /// <summary>
        /// Test poprawnego wype³niania ankiet
        /// </summary>
        [Test]
        public void CompleteSampleSurvey()
        {
            CreateSampleSurvey();
            //Start completing sample survey
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Surveys/IndexUncompletedSurveys");
            _driver.FindElement(By.CssSelector("a[href^='/Surveys/Complete']")).Click();
            Thread.Sleep(1000);

            _driver.FindElement(By.Name("answer[0][0]")).SendKeys("bla bla bla");
            _driver.FindElement(By.CssSelector("input[type=radio][value='Kobieta']")).Click();
            _driver.FindElement(By.Name("answer[2][0]")).Click();
            _driver.FindElement(By.Name("answer[2][2]")).Click();
            _driver.FindElement(By.Name("complete")).Submit();

            Thread.Sleep(1000);
            Assert.True(_driver.Url == $"{_server.RootUri}/Surveys/Success");
        }
        /// <summary>
        /// Test nieautoryzowanego wejœcia na chronion¹ stronê
        /// </summary>
        [Test]
        public void UnauthorizedAcces()
        {
            //var btn = _driver.FindElement(By.Id("logoutButton"));
            //if (btn is not null)
            //    btn.Click();
            _driver.Navigate().GoToUrl($"{_server.RootUri}/Surveys/IndexUncompletedSurveys");

            Thread.Sleep(1000);
            Assert.False(_driver.Url == $"{_server.RootUri}/Surveys/IndexUncompletedSurveys");
        }
    }
}