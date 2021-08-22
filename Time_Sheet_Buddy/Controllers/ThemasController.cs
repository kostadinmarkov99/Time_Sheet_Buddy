using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Time_Sheet_Buddy.Data;
using Time_Sheet_Buddy.Entities;
using Time_Sheet_Buddy.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Time_Sheet_Buddy.Controllers
{
    public class ThemasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public ThemasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Themas
        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var userThemaId = applicationUser.ThemaImage;

            if (userThemaId == null)
            {
                userThemaId = "23";
            }

            byte[] themaToSend = new byte[5];

            var themaPictureIdToInt = int.Parse(userThemaId);

            var thema = _context.Themas.Find(themaPictureIdToInt);

            themaToSend = thema.ThemesPicture;

            ViewBag.ThemaToShow = themaToSend;

            return View(await _context.Themas.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageAsync()
        {
            foreach (var file in Request.Form.Files)
            {
                Themas thema = new Themas();
                //themas.ThemesPicture = file.FileName;

                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                thema.ThemesPicture = ms.ToArray();

                ms.Close();
                ms.Dispose();

                _context.Themas.Add(thema);
                _context.SaveChanges();
            }

            //ViewBag.Message = "Image(s) stored in database!";
            return View("Index", _context.Themas.AsNoTracking().ToList());
        }

        public async Task<IActionResult> ThemaChoosed(string themaId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);// will give the user's userId
            //var userName = User.FindFirstValue(ClaimTypes.Name);// will give the user's userName

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            applicationUser.ThemaImage = themaId;

            _context.Users.Update(applicationUser);
            _context.SaveChanges();

            byte[] themaToSend = new byte[5];

            var themaPictureIdToInt = int.Parse(themaId);

            var thema = _context.Themas.Find(themaPictureIdToInt);

            themaToSend = thema.ThemesPicture;
            //}


            ViewBag.ThemaToShow = themaToSend;

            return View("Index", _context.Themas.AsNoTracking().ToList());
        }

        // GET: Themas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var themas = await _context.Themas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (themas == null)
            {
                return NotFound();
            }

            return View(themas);
        }

        // GET: Themas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Themas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Color")] Themas themas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(themas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(themas);
        }

        // GET: Themas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var themas = await _context.Themas.FindAsync(id);
            if (themas == null)
            {
                return NotFound();
            }
            return View(themas);
        }

        // POST: Themas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Color")] Themas themas)
        {
            if (id != themas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(themas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThemasExists(themas.Id))
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
            return View(themas);
        }

        // GET: Themas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var themas = await _context.Themas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (themas == null)
            {
                return NotFound();
            }

            return View(themas);
        }

        // POST: Themas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var themas = await _context.Themas.FindAsync(id);
            _context.Themas.Remove(themas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThemasExists(int id)
        {
            return _context.Themas.Any(e => e.Id == id);
        }
    }
}
