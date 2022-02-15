using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using WeatherForecast.API.BLL.Services.Concretes;
using Xunit;

namespace WeatherForecast.API.Test.Services
{
    public class IPServiceTest
    {
        private readonly IPService _ipService;
        private readonly Mock<IHttpContextAccessor> _mockHtpContextAccessor;

        public IPServiceTest()
        {
            _mockHtpContextAccessor = new Mock<IHttpContextAccessor>();
            _ipService = new IPService(_mockHtpContextAccessor.Object);

        }

        [Fact]
        public void GetCurrentHttpClientIPAddress_ReturnsIP()
        {
            //arrange
            var context = new DefaultHttpContext();
            var fakeIp = GetFakeIPAddress();
            context.Connection.RemoteIpAddress = GetFakeIPAddress();
            _mockHtpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            //act
            var actual = _ipService.GetCurrentHttpClientIPAddress();

            //assert
            Assert.NotNull(actual);
            Assert.Equal(fakeIp, actual);
        }

        private IPAddress GetFakeIPAddress()
        {
            return IPAddress.Parse("8.8.8.8");
        }
    }
}
