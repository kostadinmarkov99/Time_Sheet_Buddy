namespace Time_Sheet_Buddy.Test.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Collections.Generic;
    using Time_Sheet_Buddy.Controllers;
    using Time_Sheet_Buddy.Data;
    using Time_Sheet_Buddy.Entities;
    using Time_Sheet_Buddy.Models;
    using Time_Sheet_Buddy.Test.Mocks;
    using Xunit;

    public class HomeControllerTest
    {

        [Fact]
        public void IndexShouldReturnViewResult()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser() { Id = "1" },
            new ApplicationUser() { Id = "2" }
        };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            var mock = new Mock<ILogger<HomeController>>();
            ILogger<HomeController> logger = mock.Object;

            data.Issue.Add(new Issue { Id = 1 });
            data.SaveChanges();

            // Act
            var homeController = new HomeController(logger, _userManager, data);

            var result = homeController.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}
