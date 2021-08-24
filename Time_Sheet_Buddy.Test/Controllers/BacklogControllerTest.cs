using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Sheet_Buddy.Controllers;
using Time_Sheet_Buddy.Entities;
using Time_Sheet_Buddy.Models;
using Time_Sheet_Buddy.Test.Mocks;
using Xunit;

namespace Time_Sheet_Buddy.Test.Controllers
{
    public class BacklogControllerTest
    {
        [Fact]
        public void BacklogCreationShouldReturnRightResult()
        {
            // Arrange

            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            Backlog backlog = new Backlog();
            backlog.Id = 1;
            backlog.Name = "testName2";

            NewBacklogCreation newBacklogCreation = new NewBacklogCreation();
            newBacklogCreation.newBacklogName = "testName";

            // Act 
            var backlogController = new BacklogsController(data, _userManager);
            var result = backlogController.BacklogCreation(newBacklogCreation);

            var t = result.GetType();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
