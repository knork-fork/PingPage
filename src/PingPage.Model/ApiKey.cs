using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PingPage.Model
{
    public class ApiKey
    {
        [Key]
        public int ID { get; set; }

        public string KeyHash { get; set; }
        public string Type { get; set; }

        [ForeignKey(nameof(User))]
        public string UserID { get; set; }
        public User User { get; set; }
    }
}
