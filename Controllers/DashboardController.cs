using EventManagerWeb.Controllers.Attributes;
using EventManagerWeb.Data;
using EventManagerWeb.DTO;
using EventManagerWeb.Models;
using EventManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;

namespace EventManagerWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly GeocoderService _geocoderService;

        private readonly string wwwrootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");


        public DashboardController(ApplicationDbContext dbContext, EmailService emailService, 
            IConfiguration configuration, GeocoderService geocoderService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _configuration = configuration;
            _geocoderService = geocoderService;
        }

        [Route("dashboard")]
        [HttpGet]
        [AuthorizeUser]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");

            // find top 5 upcoming events
            List<Event> AllEvents = _dbContext.Events
                .Include(w => w.Guests)
                .Where(e => e.EventDate > DateTime.Now)
                .OrderBy(w => w.EventDate).ThenBy(w => w.EventTime)
                .Take(5)
                .ToList();

            ViewBag.UserId = userId;

            return View("Dashboard", AllEvents);
        }

        [Route("add/event")]
        [AuthorizeUser]
        [HttpGet]
        public IActionResult AddEvent()
        {
            var userId = HttpContext.Session.GetString("UserId");

            return View("AddEvent");
        }

        [Route("create/event")]
        [AuthorizeUser]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(FileUpload file)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (ModelState.IsValid)
            {
                var oneUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
                file.Event.CreatorName = oneUser.FirstName;
                file.Event.UserId = userId;


                if (file.FileToUpload != null)
                {
                    var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileToUpload.FileName);
                    var path = Path.Combine(wwwrootDir, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.FileToUpload.CopyToAsync(stream);
                    }

                    file.Event.EventPhoto = Path.Combine("\\uploads", fileName);
                }
                _dbContext.Add(file.Event);
                _dbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View("AddEvent");
        }

        [AuthorizeUser]
        [HttpPost]
        public async Task<IActionResult> SearchEvent()
        {
            var searchString = Request.Form["searchString"];

            var events = await _dbContext.Events
                .Include(w => w.Guests)
                .Where(e => e.EventTitle
                .Contains(searchString))
                .ToListAsync();

            return View("Dashboard", events);
        }

        [Route("join/event/{eventId}")]
        [AuthorizeUser]
        [HttpGet]
        public IActionResult JoinEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            var oneEvent = _dbContext.Events
                .Include(w => w.Guests)
                .ThenInclude(a => a.User)
                .FirstOrDefault(w => w.Id == eventId);

            var oneUser = _dbContext.Users
                .Include(u => u.UserEvents)
                .ThenInclude(a => a.Event)
                .FirstOrDefault(u => u.Id == userId);
            if (oneEvent == null)
            {
                throw new Exception($"Event with {eventId} can't be found!");
            }

            var newAssociation = new Association()
            {
                UserId = userId,
                EventId = eventId
            };

            _dbContext.Associations.Add(newAssociation);
            oneEvent?.Guests?.Add(newAssociation);
            _dbContext.SaveChanges();

            var request = new EmailDto()
            {
                To = oneUser.Email,
                Subject = $"Confirmation for {oneEvent.EventTitle}",
                Body = $"You signed up for {oneEvent.EventTitle} at on {oneEvent.EventDate} at {oneEvent.EventTime}"
            };
            _emailService.SendEmail(request);

            return RedirectToAction("Dashboard");
        }

        [Route("leave/event/{eventId}")]
        [HttpGet]
        [AuthorizeUser]
        public IActionResult LeaveEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            var oneAssociation = _dbContext.Associations
                .FirstOrDefault(a => a.EventId == eventId && a.UserId == userId);

            _dbContext.Associations.Remove(oneAssociation);
            _dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [Route("view/{eventId}")]
        [HttpGet]
        [AuthorizeUser]
        public IActionResult ViewEvent(string eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.UserId = userId;
            var oneEvent = _dbContext.Events
                .Include(w => w.Guests)
                .ThenInclude(a => a.User)
                .FirstOrDefault(w => w.Id == eventId);

            var geoResponse = _geocoderService.GetLongitudeAndLatitudeFromAddress(oneEvent.Address);
            ViewBag.Latitude = geoResponse.Latitude;
            ViewBag.Longitude = geoResponse.Longitude;

            return View("ViewEvent", oneEvent);
        }

        [Route("/delete/{eventId}")]
        [HttpGet]
        [AuthorizeUser]
        public IActionResult DeleteEvent(string eventId)
        {
            var oneEvent = _dbContext.Events.FirstOrDefault(e => e.Id == eventId);
            _dbContext.Remove(oneEvent);
            _dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(FileUpload file)
        {
            if (file.FileToUpload != null)
            {
                var path = Path.Combine(wwwrootDir, DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileToUpload.FileName));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.FileToUpload.CopyToAsync(stream);
                }

                ViewBag.filePath = path;
                return View("AddEvent");
            }

            return View("AddEvent");
        }
    }
}
