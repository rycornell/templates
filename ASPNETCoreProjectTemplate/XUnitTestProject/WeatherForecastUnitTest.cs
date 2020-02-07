using ASPNETCoreProjectTemplate.Controllers;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace XUnitTestProject
{
    public class WeatherForecastUnitTest
    {
        protected Fixture Fixture;
        // if using EF and Unit of Work pattern:
        //protected Mock<IUnitOfWork> MockUow;
        protected Mock<ILogger<WeatherForecastController>> MockLogger;
        protected WeatherForecastController WeatherForecastController;

        public WeatherForecastUnitTest()
        {
            // if using EF and Unit of Work pattern:
            // Use AutoFixture to create a usable instance of IUnitOfWork
            // Use Mock to verify calls made to the IUnitOfWork repositories

            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
            //MockUow = Fixture.Create<Mock<IUnitOfWork>>();
            MockLogger = new Mock<ILogger<WeatherForecastController>>();
            WeatherForecastController = new WeatherForecastController(MockLogger.Object);
        }

        [Fact]
        public void Test1()
        {
            var response = WeatherForecastController.Get(5);
            Assert.True(response == "5");
        }
    }
}
