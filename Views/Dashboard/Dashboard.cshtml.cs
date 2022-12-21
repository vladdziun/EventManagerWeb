using EventManagerWeb.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace EventManagerWeb.Views.Dashboard
{
    public class DashboardModel: PageModel
    {
        private ApplicationDbContext _dbContext;

        public DashboardModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnPostAsync()
        {
            var searchString = Request.Form["searchString"];

            var events = await _dbContext.Events.Where(e => e.EventTitle.Contains(searchString)).ToListAsync();
        }

    }

}
