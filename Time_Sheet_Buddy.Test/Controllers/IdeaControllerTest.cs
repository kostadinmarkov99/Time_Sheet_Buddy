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
    public class IdeaControllerTest
    {
        [Fact]
        public void ideasControllerShouldReturnRightResult()
        {
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            var ideaController = new IdeasController(data, _userManager);

            data.Ideas.Add(new Entities.Ideas { Id = 1});
            data.SaveChanges();
            /* StickyCoordinates stickyModel = new StickyCoordinates();
             stickyModel.sticky_id = 1;*/

            var idea = data.Ideas.Find(1);

            var result = ideaController.IdeasExists(data.Ideas.Find(1).Id);

            Assert.NotNull(ideaController);
        }

        [Fact]
        public void EditChangeShouldReturnResult()
        {
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            var ideaController = new IdeasController(data, _userManager);
            Ideas idea = new Ideas();
            idea.Id = 1;
            idea.Title = "234";
            idea.Description = "23456";
            idea.LeftStyle = "12";
            idea.TopStyle = "145";

            data.Ideas.Add(idea);
            data.SaveChanges();

            IdeaPropModel model = new IdeaPropModel();

            model.id = 1;
            model.title = "123";
            model.Description = "12345";

            var result = ideaController.EditChange(model);

            Assert.NotNull(result);
            int a = 5;
        }

        [Fact]
        public void SaveStickyCoordinatesShouldReturn()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            StickyCoordinates stickyCoordinates = new StickyCoordinates();
            stickyCoordinates.sticky_id = 1;
            stickyCoordinates.left_style = "123";
            stickyCoordinates.top_style = "234";

            Ideas idea = new Ideas();
            idea.Id = 1;
            idea.Title = "234";
            idea.Description = "23456";
            idea.LeftStyle = "12";
            idea.TopStyle = "145";

            data.Ideas.Add(idea);
            data.SaveChanges();

            // Act
            var ideaController = new IdeasController(data, _userManager);

            var result = ideaController.SaveStickyCoordinates(stickyCoordinates);

            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Fact]
        public void SaveStickyShouldReturnTrue()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            StickyModel stickyModel = new StickyModel();
            stickyModel.sticky_title = "testTitle";
            stickyModel.sticky_description = "testDescription";

            // Act
            var ideaController = new IdeasController(data, _userManager);

            var result = ideaController.SaveSticky(stickyModel);

            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }
    }
}
