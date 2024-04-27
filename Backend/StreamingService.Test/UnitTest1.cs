
using StreamingPlatform;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
namespace StreamingService.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]

        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("TestInitialize");
        }

        [TestMethod]
        public void Test3()
        {
            WeatherForecast weatherForecast = new WeatherForecast()
            {
                TemperatureC = 10,
                Summary = "Cool"
            };
            Assert.AreEqual(10, weatherForecast.TemperatureC);
        }
    }
}