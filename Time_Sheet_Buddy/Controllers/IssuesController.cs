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
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace Time_Sheet_Buddy.Controllers
{
    public class IssuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Issues
        [Authorize(Roles = "Administrator")]
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
            SelectList list = new SelectList(users);
            ViewBag.Users = list.ToList();
            ViewData["Users"] = users.ToList();
            var modelIssue = _context.Issue.ToList();
            return View(modelIssue);
        }

        // POST: IssueTrackers/Index
        // To save every Issue, that is not as it saved in the DataBase
        [IgnoreAntiforgeryToken]
        public ActionResult IndexPost([FromBody] IssueIdStateModel model)
        {
            string issueId = model.issue_id;
            string newState = model.issue_new_state;

            int issueIdInt = int.Parse(issueId);

            var issue = _context.Issue.Find(issueIdInt);

            var issueProject = issue.Project;

            if (newState == "open")
                newState = "New";
            else if (newState == "in-progress")
                newState = "In Progress";
            else if (newState == "resolved")
                newState = "Active";
            else if (newState == "closed")
            {
                newState = "Closed";

                issue.State = newState;
                _context.SaveChanges();
                var currentBacklog = _context.Backlogs
                .Where(b => b.Name.Equals(issueProject)).SelectMany(i => i.Issues).ToList();

                var backlogIssues = currentBacklog.Count;

                var tempCounter = 0;
                foreach(var iss in currentBacklog)
                {
                    if (iss.State == "Closed")
                        tempCounter++;
                }

                if(tempCounter == backlogIssues)
                {
                    Backlog backLg = _context.Backlogs
               .Where(b => b.Name.Equals(issueProject)).SingleOrDefault();

                    _context.Backlogs.Remove(backLg);
                     _context.SaveChanges();

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

                    //return Redirect("~/Backlogs/Index");
                    return View("~/Baklogs/Index.cshtml", modelIssue);

                }
            }
            else if(newState == "deletion-tray")
            {
                _context.Issue.Remove(issue);
                _context.SaveChanges();

                var currentBacklog = _context.Backlogs
                .Where(b => b.Name.Equals(issueProject)).SelectMany(i => i.Issues).ToList();

                var backlogIssues = currentBacklog.Count;

                var tempCounter = 0;
                foreach (var iss in currentBacklog)
                {
                    if (iss.State == "Closed")
                        tempCounter++;
                }
                if (tempCounter == backlogIssues)
                {
                    Backlog backLg = _context.Backlogs
               .Where(b => b.Name.Equals(issueProject)).SingleOrDefault();

                    _context.Backlogs.Remove(backLg);
                    _context.SaveChanges();

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

                    //return Redirect("~/Backlogs/Index");
                    return View("~/Baklogs/Index.cshtml", modelIssue);

                }
            }

            issue.State = newState;
            if (newState == "Closed")
                issue.Duration = 0;

            _context.SaveChanges();
            return RedirectToAction("Index");
            //View("Index");
            //return View();   
        }

        // POST: IssueTrackers/Index
        // To save every Issue, that is not as it saved in the DataBase
        [IgnoreAntiforgeryToken]
        public ActionResult IndexChange([FromBody] AssignedToIdValue model)
        {
            string issueId = model.propVal_id;
            string valueTypeValue = model.propVal_new_value;
            string valueTypeTypeValue = model.propVal_Type;

            int issueIdInt = int.Parse(issueId);

            var issue = _context.Issue.Find(issueIdInt);

            if(valueTypeTypeValue == "AssigendTo")
                issue.AssignedTo = valueTypeValue;
            else if(valueTypeTypeValue == "Project")
                issue.Project = valueTypeValue;

            _context.SaveChanges();
            return RedirectToAction("Index");
            //View("Index");
            //return View();   
        }

        // GET: Issues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // GET: Issues/Create
        public IActionResult Create()
        {
            var users = _context.Users;
            //SelectList list = new SelectList(users);


            //ViewBag.Users = new SelectList(_context.Users, "Title", "Title");

            /* var states = from r in db.States orderby r.Title select r;
             SelectList stateList = new SelectList(states);*/
            ViewBag.States = new SelectList(_context.Stetes, "Title", "Title");
            ViewBag.Projects = new SelectList(_context.Projectcs, "Title", "Title");

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userItem = _context.Users.Find(currentUserId);

            string userName = userItem.UserName;

            ViewData["Assignee"] = userName;

            ViewData["Users"] = users;
            return View();
        }

        // POST: Issues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Duration,AssignedTo, Assignee,Date,State,Project")] Issue issue)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userItem = _context.Users.Find(currentUserId);

            string userName = userItem.UserName;

            issue.Assignee = userName;

            _context.Add(issue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateNewTask()
        {
            Issue issue = new Issue();

            issue.Title = "New Task";

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userItem = _context.Users.Find(currentUserId);

            string userName = userItem.UserName;

            issue.Assignee = userName;

            issue.State = "New";
            _context.Add(issue);
            await _context.SaveChangesAsync();

            string id = issue.Id.ToString();

            return RedirectToAction("Index");
        }

        // GET: Issues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Issue issues = _context.Issue.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issue.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            var users1 = _context.Users;
            SelectList list1 = new SelectList(users1);
            ViewBag.Users = list1.ToList();
            ViewData["Users"] = users1.ToList();

            /* var states = _context.Stetes;
             SelectList states1 = new SelectList(states);
             ViewBag.States = states1.ToList();*/
            ViewData["States"] = _context.Stetes.ToList();

            //ViewBag.Users = new SelectList(_context.Users, "Email", "Email");


            //ViewBag.Projects = new SelectList(_context.Projectcs, "Title", "Title");

            //ViewData["Users"] = users;

            return View(issue);
        }

        // To save every Issue, that is not as it saved in the DataBase
        [IgnoreAntiforgeryToken]
        //[Route("Issues/Details")]
        public async Task EditChange([FromBody] issueModel model)
        {
            int modelId = model.Id;
            string modelTitle = model.Title;
            string modelDescription = model.Description;
            string modelState = model.State.ToLower();
            double modelDuration = model.Duration;
            string modelAssignedTo = model.AssignedTo;

            string newState = "New";
            if (modelState == "open")
                newState = "New";
            else if (modelState == "in progress")
                newState = "In Progress";
            else if (modelState == "active")
                newState = "Active";
            else if (modelState == "close")
                newState = "Closed";

            Issue issues = _context.Issue.Find(modelId);

            if (modelId == 0)
            {
                return;
                //return NotFound();
            }

            var issue = _context.Issue.Find(modelId);
            if (issue == null)
            {

                return;
                //return NotFound();
            }
            issue.Title = modelTitle;
            issue.Duration = modelDuration;
            issue.State = newState;
            if (newState == "Closed")
                issue.Duration = 0;
            issue.Description = modelDescription;
            issue.AssignedTo = modelAssignedTo;

            _context.Update(issue);
             await _context.SaveChangesAsync();
            //var issueReturn = await _context.Issue
                //.FirstOrDefaultAsync(m => m.Id == modelId);
            //return View("./Details", issueReturn);
            //return Redirect("Issues/Details?" + id);
            //return RedirectToActionPermanent("Details", "Issues", new { id = modelId });
            //return RedirectToAction("Details",  new { id = modelId });
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Duration,Assignie,Date,State,Project")] Issue issue)
        {
            var users1 = _context.Users;
            SelectList list1 = new SelectList(users1);
            ViewBag.Users = list1.ToList();
            ViewData["Users"] = users1.ToList();

            /* var states = _context.Stetes;
             SelectList states1 = new SelectList(states);
             ViewBag.States = states1.ToList();*/
            ViewData["States"] = _context.Stetes.ToList();

            if (id != issue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(issue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IssueExists(issue.Id))
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
            return View(issue);
        }

        // GET: Issues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issue.FindAsync(id);
            _context.Issue.Remove(issue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IssueExists(int id)
        {
            return _context.Issue.Any(e => e.Id == id);
        }
    }
}
