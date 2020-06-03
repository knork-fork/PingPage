﻿using Microsoft.EntityFrameworkCore;
using PingPage.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingPage.DAL
{
    public class PingPageDbContext : DbContext
    {
        protected PingPageDbContext()
        { 
        }

        public PingPageDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Ping> Pings { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<InviteLink> InviteLinks { get; set; }
    }
}
