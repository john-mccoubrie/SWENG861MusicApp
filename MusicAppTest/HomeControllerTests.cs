using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicApp.Controllers;
using MusicApp.Services;

namespace MusicApp.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<ILogger<HomeController>> _loggerMock;
        private Mock<MusicService> _musicServiceMock; // Mock for the MusicService

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the mock objects
            _loggerMock = new Mock<ILogger<HomeController>>();
            _musicServiceMock = new Mock<MusicService>(); // Initializing the mock for MusicService

            // Initialize the HomeController with both mocks
            _controller = new HomeController(_loggerMock.Object, _musicServiceMock.Object);
        }

        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Index should return a ViewResult.");
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Privacy should return a ViewResult.");
        }

        [TestMethod]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Error should return a ViewResult.");
        }
    }
}
