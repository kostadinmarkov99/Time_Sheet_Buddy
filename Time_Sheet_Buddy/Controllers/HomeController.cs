using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Time_Sheet_Buddy.Data;
using Time_Sheet_Buddy.Models;

namespace Time_Sheet_Buddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
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

            ViewBag.ThemaToShow = themaToSend;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
