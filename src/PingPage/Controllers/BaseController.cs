using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingPage.Model;

namespace PingPage.Controllers
{
    public class BaseController : Controller
    {
        private UserManager<User> _userManager;

        public BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string UserId { get { return this._userManager.GetUserId(base.User); } }
    }
}