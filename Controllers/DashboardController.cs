using EventManagerWeb.Data;
using EventManagerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagerWeb.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext _dbContext;

        public DashboardController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("dashboard")]
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");
            //ValidateUserLoggedIn(userId);

            List<Event> AllEvents = _dbContext.Events
                .Include(w => w.Guests)
                .OrderBy(w => w.EventDate).ThenBy(w => w.EventTime)
                .ToList();
            ViewBag.UserId = userId;

            return View("Dashboard", AllEvents);
        }

        [Route("add/event")]
        [HttpGet]
        public IActionResult AddEvent()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            ValidateUserLoggedIn(userId);

            return View("AddEvent");
        }

        [Route("create/event")]
        [HttpPost]
        public IActionResult CreateEvent(Event newEvent)
        {
            var userId = HttpContext.Session.GetString("UserId");
            //ValidateUserLoggedIn(userId);
            var ll = _dbContext.Users.FirstOrDefault(w => w.Id == userId);

            if (ModelState.IsValid)
            {
                var oneUser = _dbContext.Users.FirstOrDefault(w => w.Id == userId);
                newEvent.CreatorName = oneUser.FirstName;
                newEvent.UserId = userId;
                _dbContext.Add(newEvent);
                _dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View("AddEvent");
        }

        [Route("join/event/{eventId}")]
        [HttpGet]
        public IActionResult JoinEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            //ValidateUserLoggedIn(userId);

            Event oneEvent = _dbContext.Events
                .Include(w => w.Guests)
                .ThenInclude(a => a.User)
                .FirstOrDefault(w => w.Id == eventId);

            User oneUser = _dbContext.Users
                .Include(u => u.UserEvents)
                .ThenInclude(a => a.Event)
                .FirstOrDefault(u => u.Id == userId);


            Association newAssociation = new Association()
            {
                UserId = userId,
                EventId = eventId
            };

            _dbContext.Associations.Add(newAssociation);
            oneEvent.Guests.Add(newAssociation);
            _dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [Route("leave/event/{eventId}")]
        [HttpGet]
        public IActionResult LeaveEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            //ValidateUserLoggedIn(userId);

            var oneAssociation = _dbContext.Associations
                .FirstOrDefault(a => a.EventId == eventId && a.UserId == userId);

            _dbContext.Associations.Remove(oneAssociation);
            _dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [Route("view/{eventId}")]
        [HttpGet]
        public IActionResult ViewEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            //ValidateUserLoggedIn(userId);

            Event oneEvent = _dbContext.Events
                .Include(w => w.Guests)
                .ThenInclude(a => a.User)
                .FirstOrDefault(w => w.Id == eventId);
            ViewBag.UserId = userId;

            return View("ViewEvent", oneEvent);
        }

        [Route("/delete/{eventId}")]
        [HttpGet]
        public IActionResult DeleteEvent(string eventId)
        {
            Event oneEvent = _dbContext.Events.FirstOrDefault(w => w.Id == eventId);
            _dbContext.Remove(oneEvent);
            _dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        private IActionResult ValidateUserLoggedIn(int? userId)
        {
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return NoContent();
        }
    }
}
