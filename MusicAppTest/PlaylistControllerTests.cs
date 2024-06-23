using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicApp.Controllers;
using MusicApp.Services;
using System.Threading.Tasks;

namespace MusicApp.Tests
{
    [TestClass]
    public class PlaylistControllerTests
    {
        private PlaylistController _controller;
        private Mock<MusicService> _mockMusicService;  // Mocking the MusicService

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMusicService = new Mock<MusicService>();  // Initialize the mock
            _controller = new PlaylistController(_mockMusicService.Object);  // Use the mock object
        }

        [TestMethod]
        public async Task DeletePlaylist_ReturnsRedirectToAction()
        {
            // Setup the mock to avoid NullReferenceException
            _mockMusicService.Setup(s => s.DeletePlaylistAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePlaylist(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }
}
