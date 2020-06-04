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
using PingPage.Models;

namespace PingPage.Controllers
{
    public class FriendController : BaseController
    {
        private PingPageDbContext _dbContext;
        private UserManager<User> _userManager;

        public FriendController(PingPageDbContext dbContext, UserManager<User> userManager) : base(userManager)
        {
            this._dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewData["inviteLink"] = this.GetInviteLink();

            var allUsers = this._dbContext.Users
                .Include(u => u.Pings)
                .Include(u => u.FriendshipsReceived)
                .Include(u => u.FriendshipsSent)
                .ToList();

            List<User> users = new List<User>();
            foreach (var user in allUsers)
            {
                var friendships = this._dbContext.Friendships.Where(f => f.Receiver.Id == user.Id || f.Sender.Id == user.Id);

                if (friendships.Count() == 0)
                    continue;

                // Skip yourself
                if (user.Id == UserId)
                    continue;

                users.Add(user);
            }

            var model = users.ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult IndexAjax(UserFilterModel filter)
        {
            var userQuery = this._dbContext.Users.Include(u => u.Pings).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
                userQuery = userQuery.Where(p => (p.FirstName + " " + p.LastName).Contains(filter.FullName));

            var allUsers = userQuery.ToList();

            List<User> users = new List<User>();
            foreach (var user in allUsers)
            {
                var friendships = this._dbContext.Friendships.Where(f => f.Receiver.Id == user.Id || f.Sender.Id == user.Id);

                if (friendships.Count() == 0)
                    continue;

                // Skip yourself
                if (user.Id == UserId)
                    continue;

                users.Add(user);
            }

            var model = users.ToList();

            return PartialView("_FriendList", model);
        }

        [Authorize]
        public IActionResult Feed()
        {
            var totalPings = this._dbContext.Pings
                .Include(p => p.User)
                .Include(p => p.User.FriendshipsReceived)
                .Include(p => p.User.FriendshipsSent)
                .ToList();

            List<Ping> pings = new List<Ping>();
            foreach (var ping in totalPings)
            {
                var received = ping.User.FriendshipsReceived.Where(f => f.Receiver.Id == UserId || f.Sender.Id == UserId);
                var sent = ping.User.FriendshipsSent.Where(f => f.Receiver.Id == UserId || f.Sender.Id == UserId);

                if (received.Count() == 0 && sent.Count() == 0)
                    continue;

                // Skip yourself
                if (ping.UserID == UserId)
                    continue;

                pings.Add(ping);
            }

            var model = pings.Select(p => new FeedUser
            {
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Ping = p.PingTimestamp
            });

            model = model.OrderByDescending(m => m.Ping);

            return View(model.ToList());
        }

        public IActionResult Generate()
        {
            // Generate random 12-character string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var link = new string(Enumerable.Repeat(chars, 12)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

            var inviteLink = new InviteLink();
            inviteLink.UserID = UserId;
            inviteLink.Link = link;

            this._dbContext.InviteLinks.Add(inviteLink);
            this._dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(FriendshipAdderModel model)
        {
            ViewData["inviteLink"] = this.GetInviteLink();

            if (model.Link == null)
            {
                ViewData["error"] = "Invalid invite link";
                return View("Index");
            }

            // Check if already friend
            var friendships = this._dbContext.Friendships.Where(f => f.Receiver.Id == UserId || f.Sender.Id == UserId);

            if (friendships.Count() != 0)
            {
                // Friendship already exists, return
                ViewData["error"] = "Invalid invite link - already a friend";
                return View("Index");
            }

            var receiverLink = this._dbContext.InviteLinks.Where(l => l.Link.Equals(model.Link)).LastOrDefault();
            if (receiverLink == null)
            {
                ViewData["error"] = "Invalid invite link - invalid link";
                return View("Index");
            }

            var sender = this._dbContext.Users.Where(u => u.Id == UserId).First();
            var receiver = this._dbContext.Users.Include(p => p.InviteLinks).Where(u => u.InviteLinks.Contains(receiverLink)).LastOrDefault();
            if (receiver == null)
            {
                ViewData["error"] = "Invalid invite link - invalid receiver";
                return View("Index");
            }

            // Add friend
            var friendship = new Friendship();
            friendship.Sender = sender;
            friendship.Receiver = receiver;

            this._dbContext.Friendships.Add(friendship);
            this._dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            var friendships = this._dbContext.Friendships.Where(f => f.Receiver.Id == UserId)
                .Include(f => f.Sender)
                .ToList();
            foreach (var friendship in friendships)
            {
                if (friendship.Sender.Id == id)
                {
                    this._dbContext.Remove(friendship);
                    this._dbContext.SaveChanges();
                }
            }

            friendships = this._dbContext.Friendships.Where(f => f.Sender.Id == UserId)
                .Include(f => f.Receiver)
                .ToList();
            foreach (var friendship in friendships)
            {
                if (friendship.Receiver.Id == id)
                {
                    this._dbContext.Remove(friendship);
                    this._dbContext.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private string GetInviteLink()
        {
            var user = this._dbContext.Users.Include(p => p.InviteLinks).Where(u => u.Id == UserId).First();

            // Last invite link
            var linkObj = user.InviteLinks.OrderBy(p => p.ID).LastOrDefault();

            string link;
            if (linkObj != null)
                link = linkObj.Link;
            else
                link = null;

            return link;
        }
    }
}