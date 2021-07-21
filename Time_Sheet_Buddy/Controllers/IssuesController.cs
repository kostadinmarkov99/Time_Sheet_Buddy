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

            if (newState == "open")
                newState = "New";
            else if (newState == "in-progress")
                newState = "In Progress";
            else if (newState == "resolved")
                newState = "Active";
            else if (newState == "closed")
                newState = "Closed";

            issue.State = newState;
            if (newState == "Closed")
                issue.Duration = 0;

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
            issue.Assignee = "sm";

            _context.Add(issue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateNewTask()
        {
            Issue issue = new Issue();

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

            ViewBag.Users = new SelectList(_context.Users, "Email", "Email");


            ViewBag.Projects = new SelectList(_context.Projectcs, "Title", "Title");

            //ViewData["Users"] = users;

            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Duration,Assignie,Date,State,Project")] Issue issue)
        {
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
