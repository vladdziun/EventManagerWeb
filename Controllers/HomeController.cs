using EventManagerWeb.Data;
using EventManagerWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EventManagerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, PasswordHasher<User> passwordHasher, 
            SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId))
            {
                HttpContext.Session.SetString("UserId", userId);
                return RedirectToAction("Dashboard", "Dashboard");

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                if (userInDb != null)
                {
                    ModelState.AddModelError("Email", "This email already taken");
                    return View("Index");
                }

                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, newUser.PasswordHash);
                _dbContext.Add(newUser);
                _dbContext.SaveChanges();
                var userToLogIn = _dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                HttpContext.Session.SetString("Id", userToLogIn.Id);
                return View("Index");

            }
            else
            {
                return View("Index");
            }
        }
    }
}