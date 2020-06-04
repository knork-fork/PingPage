using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingPage.DAL;
using PingPage.Model;

namespace PingPage.Controllers
{
    public class PingController : BaseController
    {
        private PingPageDbContext _dbContext;
        private UserManager<User> _userManager;

        public PingController(PingPageDbContext dbContext, UserManager<User> userManager) : base(userManager)
        {
            this._dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var user = this._dbContext.Users.Include(p => p.Pings).Where(u => u.Id == UserId).First();

            // Last ping
            var ping = user.Pings.OrderBy(p => p.PingTimestamp).LastOrDefault();
            DateTime? timestamp;

            if (ping != null)
                timestamp = ping.PingTimestamp;
            else
                timestamp = null;

            var model = new BasicUserData { Id = user.Id, FullName = user.FullName, LastPing = timestamp };

            return View(model);
        }

        [Authorize]
        public IActionResult Ping()
        {
            var ping = new Ping();
            ping.UserID = UserId;
            ping.PingTimestamp = DateTime.Now;

            this._dbContext.Pings.Add(ping);
            this._dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }

    public class BasicUserData
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime? LastPing { get; set; }
    }
}