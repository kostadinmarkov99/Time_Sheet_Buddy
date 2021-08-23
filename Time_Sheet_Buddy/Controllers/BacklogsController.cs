using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Time_Sheet_Buddy.Data;
using Time_Sheet_Buddy.Entities;
using Time_Sheet_Buddy.Models;

namespace Time_Sheet_Buddy.Controllers
{
    public class BacklogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public BacklogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Backlogs
        [Authorize]
        [HttpGet]
        [Route("index/")]
        public async Task<IActionResult> Index()
        {
            var selectedValue = "Show All";
            try
            {
                selectedValue = Request.Form["id_filter_list"];
            }
            catch (Exception e) { }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (selectedValue != null && selectedValue != "Show All")
                selectedValue = selectedValue.Split(' ')[0];

            if (selectedValue != "Show All")
            {
                var users1 = _context.Users;
                SelectList list1 = new SelectList(users1);
                ViewBag.Users = list1.ToList();
                ViewData["Users"] = users1.ToList();
                var filteredIssues = _context.Issue
                    .Where(i => i.AssignedTo.Equals(selectedValue))
                    .OrderBy(i => i.Title)
                    .Select(i => new Issue
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        Duration = i.Duration,
                        AssignedTo = i.AssignedTo,
                        Date = i.Date,
                        State = i.State,
                        Project = i.Project
                    })
                    .ToList();

                ViewData["LastChoice"] = selectedValue;
                ViewData["CurrentUser"] = userId;

                return View(filteredIssues);
            }

            //ViewData["LastChoice"] = selectedValue;
            ViewData["CurrentUser"] = userId;

            var users = _context.Users;
            var projects = _context.Projectcs;
            SelectList list = new SelectList(users);
            SelectList projectList = new SelectList(projects);
            ViewBag.ProjectsList = projectList.ToList();
            ViewData["Users"] = users.ToList();
            var modelIssue = _context.Projectcs.ToList();

            Dictionary<string, int> backlogItemsCount = new Dictionary<string, int>();

            var allBacklogs = _context.Backlogs;

            foreach (var backlog in allBacklogs)
            {
                string backlogName = backlog.Name;
                int backlogId = backlog.Id;

                var currentBacklogIssues = _context.Backlogs
                .Where(b => b.Id.Equals(backlogId)).SelectMany(i => i.Issues).ToList();

                var backlogItmIssuesCount = currentBacklogIssues.Count;

                backlogItemsCount.Add(backlogName, backlogItmIssuesCount);
            }

            ViewBag.BacklogItemsCount = backlogItemsCount;

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var themaPictureId = applicationUser.ThemaImage;

            byte[] themaToSend = new byte[5];

            if(themaPictureId != null)
            {
                var themaPictureIdToInt = int.Parse(themaPictureId);

                var thema = _context.Themas.Find(themaPictureIdToInt);

                themaToSend = thema.ThemesPicture;
            }

            ViewBag.ThemaToShow = themaToSend;

            return View(modelIssue);
        }

        public async void ProjectBacklogGet([FromBody] SelectedProject selectedProject)
        {
            
            var proj = selectedProject.selected_project;

            //var modelIssue = _context.Issue.Where(i => i.Project == proj).ToList();

            await ProjectBacklog(proj);
        }

        private async Task<byte[]> getThemaId()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            var userThemaId = "23";

            if (applicationUser != null)
            {
                userThemaId = applicationUser.ThemaImage;
            }

            byte[] themaToSend = new byte[5];

            var themaPictureIdToInt = int.Parse(userThemaId);

            var thema = _context.Themas.Find(themaPictureIdToInt);

            themaToSend = thema.ThemesPicture;

            return themaToSend;
        }

        [Authorize]
        public async Task<IActionResult> ProjectBacklog(string proj = "", string selectedFilter = "")
        {
            byte[] themaToSend = await getThemaId();

            ViewBag.ThemaToShow = themaToSend;

            var selectedValue = "Show All";
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (selectedValue != null && selectedValue != "Show All")
                selectedValue = selectedValue.Split(' ')[0];

            var users = _context.Users;
            SelectList list = null;

            if (selectedFilter != "" && selectedFilter != "Show All")
            {
                if (selectedFilter.Contains("(Me)"))
                {
                    var neededIndex = selectedFilter.IndexOf(" (Me)");
                    selectedFilter = selectedFilter.Substring(0, neededIndex);
                }

                    var users1 = _context.Users;
                SelectList list1 = new SelectList(users1);
                ViewBag.Users = list1.ToList();
                ViewData["Users"] = users1.ToList();
                var filteredIssues = _context.Issue
                    .Where(i => i.AssignedTo == selectedFilter)
                    .OrderBy(i => i.Title)
                    .ToList();

                ViewData["LastChoice"] = selectedValue;
                ViewData["CurrentUser"] = userId;

                users = _context.Users;
                list = new SelectList(users);
                ViewBag.Users = list.ToList();
                ViewData["Users"] = users.ToList();

                //ViewData["LastChoice"] = selectedValue;
                ViewData["CurrentUser"] = userId;
                ViewData["CurrentProject"] = proj;

                return View(filteredIssues);
            }

            //ViewData["LastChoice"] = selectedValue;
            ViewData["CurrentUser"] = userId;
            ViewData["CurrentProject"] = proj;

            users = _context.Users;
            list = new SelectList(users);
            ViewBag.Users = list.ToList();
            ViewData["Users"] = users.ToList();
            
            var modelIssue = _context.Issue.Where(i => i.Project == proj).ToList();

            var currentBacklog = _context.Backlogs
                .Where(b => b.Name.Equals(proj)).Any();

            if(!currentBacklog)
            {
                var newProject = new Projectcs();
                newProject.Title = proj;
                
                var newBacklog = new Backlog();
                newBacklog.Name = proj;

                var newBacklogIssues = new List<Issue>();

                foreach (var issue in modelIssue)
                    newBacklogIssues.Add(issue);

                newBacklog.Issues = newBacklogIssues;

                _context.Add(newBacklog);
                _context.Add(newProject);
                await _context.SaveChangesAsync();
            }


            return View(modelIssue);
        }

        [IgnoreAntiforgeryToken]
        public ActionResult BacklogCreation([FromBody] NewBacklogCreation model)
        {
            string modelProp = model.newBacklogName;

           var newProject = new Projectcs();
            newProject.Title = modelProp;
            _context.Projectcs.Add(newProject);

            var newBacklog = new Backlog();
            newBacklog.Name = modelProp;
            _context.Add(newBacklog);

            _context.SaveChangesAsync();

            return RedirectToAction("ProjectBacklog", "Backlogs", new { proj = modelProp });
        }

        public async Task<IActionResult> CreateNewTask(string projectName)
        {
            Issue issue = new Issue();

            issue.Title = "New Task";

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userItem = _context.Users.Find(currentUserId);

            string userName = userItem.UserName;

            issue.Assignee = userName;
            issue.Project = projectName;
            
            issue.State = "New";
            _context.Add(issue);

            Backlog backLg = _context.Backlogs
                .Where(b => b.Name.Equals(projectName)).SingleOrDefault();


            var currentBacklog = _context.Backlogs
                .Where(b => b.Name.Equals(projectName)).SelectMany(i => i.Issues).ToList();


            var elements = currentBacklog.Count();

            currentBacklog.Add(issue);

            backLg.Issues = currentBacklog;

            _context.Update(backLg);
            await _context.SaveChangesAsync();
            elements = currentBacklog.Count();

            //currentBacklog.Issues.Add(issue);

            await _context.SaveChangesAsync();

            var proj = projectName;

            return RedirectToAction("ProjectBacklog", "Backlogs", new { proj });
            //RedirectToAction("ProjectBacklog");
            //ProjectBacklog(projectName);
            //return RedirectToAction("Index");
        }        

        private bool BacklogExists(int id)
        {
            return _context.Backlogs.Any(e => e.Id == id);
        }
    }
}
