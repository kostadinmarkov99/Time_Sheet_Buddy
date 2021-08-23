using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Time_Sheet_Buddy.Data;
using Time_Sheet_Buddy.Entities;
using Time_Sheet_Buddy.Models;

namespace Time_Sheet_Buddy.Controllers
{
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public IdeasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // GET: Ideas
        public async Task<IActionResult> Index()
        {
            byte[] themaToSend = await getThemaId();

            ViewBag.ThemaToShow = themaToSend;

            return View(await _context.Ideas.ToListAsync());
        }

        public void SaveSticky([FromBody] StickyModel stickyModel)
        {
            var title = stickyModel.sticky_title;
            var description = stickyModel.sticky_description;

            Ideas newIdea = new Ideas();

            newIdea.Title = title;
            newIdea.Description = description;

            _context.Ideas.Add(newIdea);
            _context.SaveChanges();
        }



        public void SaveStickyCoordinates([FromBody] StickyCoordinates stickyCoordinates)
        {
            if (stickyCoordinates == null) return;
            var stickyId = stickyCoordinates.sticky_id; 
            var leftStyle = stickyCoordinates.left_style;
            var topStyle = stickyCoordinates.top_style;

            List<char> charsToRemove = new List<char>() { 'p', 'x', ' ', '.', ',', '_' };
            
            foreach(var c in charsToRemove)
            {
                leftStyle = leftStyle.Replace(c.ToString(), String.Empty);
                topStyle = topStyle.Replace(c.ToString(), String.Empty);
            }

            Ideas idea = _context.Ideas.Find(stickyId);
            idea.LeftStyle = leftStyle;
            idea.TopStyle = topStyle;

            _context.Ideas.Update(idea);
            _context.SaveChanges();
        }

        // GET: Ideas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideas = await _context.Ideas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ideas == null)
            {
                return NotFound();
            }

            return View(ideas);
        }

        // GET: Ideas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ThemesPicture")] Ideas ideas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ideas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ideas);
        }

        // GET: Ideas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideas = await _context.Ideas.FindAsync(id);
            if (ideas == null)
            {
                return NotFound();
            }
            return View(ideas);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ThemesPicture")] Ideas ideas)
        {
            if (id != ideas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ideas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdeasExists(ideas.Id))
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
            return View(ideas);
        }

        public void DeleteSticky([FromBody] StickyCoordinates stickyCoordinates)
        {
            var stickyId = stickyCoordinates.sticky_id;

            Ideas idea = _context.Ideas.Find(stickyId);

            _context.Ideas.Remove(idea);

            try { _context.SaveChanges(); }
            catch(Exception ex) { }

            return;
        }

        // GET: Ideas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideas = await _context.Ideas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ideas == null)
            {
                return NotFound();
            }

            return View(ideas);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ideas = await _context.Ideas.FindAsync(id);
            _context.Ideas.Remove(ideas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeasExists(int id)
        {
            return _context.Ideas.Any(e => e.Id == id);
        }
    }
}
