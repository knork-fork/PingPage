using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PingPage.Model
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<Friendship> FriendshipsSent { get; set; }
        [InverseProperty("Receiver")]
        public virtual ICollection<Friendship> FriendshipsReceived { get; set; }

        public virtual ICollection<Ping> Pings { get; set; }
        public virtual ICollection<ApiKey> ApiKeys { get; set; }
        public virtual ICollection<InviteLink> InviteLinks { get; set; }
    }
}
