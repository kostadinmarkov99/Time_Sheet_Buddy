using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public BacklogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Backlogs
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

            return View(modelIssue);
        }
        
        public async void ProjectBacklogGet([FromBody] SelectedProject selectedProject)
        {
            
            var proj = selectedProject.selected_project;

            //var modelIssue = _context.Issue.Where(i => i.Project == proj).ToList();

            await ProjectBacklog(proj);
        }

        public async Task<IActionResult> ProjectBacklog(string proj)
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
            ViewData["CurrentProject"] = proj;

            var users = _context.Users;
            SelectList list = new SelectList(users);
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

            var currentBacklog = _context.Backlogs
                .Where(b => b.Name.Equals(projectName)).SelectMany(i => i.Issues).ToList();

            var elements = currentBacklog.Count();

            currentBacklog.Add(issue);
                        
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

        // GET: Backlogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backlog = await _context.Backlogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backlog == null)
            {
                return NotFound();
            }

            return View(backlog);
        }

        // GET: Backlogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Backlogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Backlog backlog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(backlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(backlog);
        }

        // GET: Backlogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backlog = await _context.Backlogs.FindAsync(id);
            if (backlog == null)
            {
                return NotFound();
            }
            return View(backlog);
        }

        // POST: Backlogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Backlog backlog)
        {
            if (id != backlog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(backlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BacklogExists(backlog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(backlog);
        }

        // GET: Backlogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backlog = await _context.Backlogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backlog == null)
            {
                return NotFound();
            }

            return View(backlog);
        }

        // POST: Backlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var backlog = await _context.Backlogs.FindAsync(id);
            _context.Backlogs.Remove(backlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BacklogExists(int id)
        {
            return _context.Backlogs.Any(e => e.Id == id);
        }
    }
}
