using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingPage.Models
{
    public class UserFilterModel
    {
        public string FullName { get; set; }
    }

    public class FeedUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Ping { get; set; }

        public string GetMessage { get { return GetRandomMessage(); } }

        private string GetRandomMessage()
        {
            return "pinged on";
        }
    }
}
