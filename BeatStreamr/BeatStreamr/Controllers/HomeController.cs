using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeatStreamr.Models;
using Microsoft.EntityFrameworkCore;

namespace BeatStreamr.Controllers
{
    public class HomeController : Controller
    {
        private readonly BeatStreamrContext _context;

        public HomeController(BeatStreamrContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = (from u in _context.User
                         select u).Take(4);

            return View(await users.ToListAsync());
                                       
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
