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
    public class IssueControllerTest
    {
        [Fact]
        public void returnIndexPostShouldReturnViewType()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            Issue issue = new Issue();
            issue.Id = 1;
            issue.Title = "testTitle";
            issue.Description = "testDescription";
            issue.Assignee = "user1";
            issue.AssignedTo = "user2";
            issue.Date = DateTime.Now;
            issue.Duration = 2;
            issue.Project = "testProject";
            issue.State = "testState";

            data.Issue.Add(issue);
            data.SaveChanges();

            IssueIdStateModel issueIdStateModel = new IssueIdStateModel();
            issueIdStateModel.issue_id = "1";

            //Act
            var issuesController = new IssuesController(data, _userManager);
            var result = issuesController.IndexPost(issueIdStateModel);

            var resultType = Assert.IsType<IssueIdStateModel>(issueIdStateModel);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void EditChangeShouldReturnReightModelData()
        {
            // Arrange

            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            Issue issue = new Issue();
            issue.Id = 1;
            issue.Title = "testTitle";
            issue.Description = "testDescription";
            issue.Assignee = "user1";
            issue.AssignedTo = "user2";
            issue.Date = DateTime.Now;
            issue.Duration = 2;
            issue.Project = "testProject";
            issue.State = "testState";

            data.Issue.Add(issue);
            data.SaveChanges();

            issueModel IssueModelM = new issueModel();
            IssueModelM.Id = 1;
            IssueModelM.Title = "testTitle2";
            IssueModelM.Description = "testDescription2";
            IssueModelM.State = "New";
            IssueModelM.Duration = "11";
            IssueModelM.AssignedTo = "user3";

            // Act
            IssuesController issuesController = new IssuesController(data, _userManager);
            var result = issuesController.EditChange(IssueModelM);

            //Assert
            Assert.Equal("testTitle2", data.Issue.Find(1).Title);
            Assert.Equal("testDescription2", data.Issue.Find(1).Description);
        }

        [Fact]
        public void IndexChangeShouldReturnRightValue()
        {
            // Arrange

            using var data = DatabaseMock.Instance;

            List<ApplicationUser> _users = new List<ApplicationUser>
            {
                new ApplicationUser() { Id = "1" },
                new ApplicationUser() { Id = "2" }
            };

            var _userManager = MockUserManager.MockUserManagerMock<ApplicationUser>(_users).Object;

            Issue issue = new Issue();
            issue.Id = 1;
            issue.Title = "testTitle";
            issue.Description = "testDescription";
            issue.Assignee = "user1";
            issue.AssignedTo = "user2";
            issue.Date = DateTime.Now;
            issue.Duration = 2;
            issue.Project = "testProject";
            issue.State = "testState";

            data.Issue.Add(issue);
            data.SaveChanges();

            AssignedToIdValue assignedToIdValue = new AssignedToIdValue();
            assignedToIdValue.propVal_id = "1";
            assignedToIdValue.propVal_new_value = "user3";
            assignedToIdValue.propVal_Type = "AssigendTo";

            // Act

            var issueController = new IssuesController(data, _userManager);
            var result = issueController.IndexChange(assignedToIdValue);

            // Assert
            Assert.Equal("user3", data.Issue.Find(1).AssignedTo);
        }
    }
}
