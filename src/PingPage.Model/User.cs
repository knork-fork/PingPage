using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PingPage.Model
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<Friendship> FriendshipsSent { get; set; }
        [InverseProperty("Receiver")]
        public virtual ICollection<Friendship> FriendshipsReceived { get; set; }
    }
}
