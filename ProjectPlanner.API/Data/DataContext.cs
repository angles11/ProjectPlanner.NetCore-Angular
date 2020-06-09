﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        private readonly DbContextOptions<DataContext> _options;

        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            _options = options;
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(P => P.OwnerId);                

            builder.Entity<Collaborator>()
                  .HasKey(c => new { c.UserId, c.ProjectId });

            builder.Entity<Collaborator>()
                    .HasOne(c => c.User)
                    .WithMany(u => u.CollaboratedProjects)
                    .HasForeignKey(c => c.UserId);

            builder.Entity<Collaborator>()
                .HasOne(c => c.Project)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(c => c.ProjectId);

            builder.Entity<Friendship>()
                .HasKey(f => new { f.SenderId, f.RecipientId });

            builder.Entity<Friendship>()
                .HasOne(f => f.Sender)
                .WithMany(s => s.FriendshipSent)
                .HasForeignKey(f => f.SenderId);


            builder.Entity<Friendship>()
                .HasOne(f => f.Recipient)
                .WithMany(f => f.FriendshipReceived)
                .HasForeignKey(f => f.RecipientId);
                 
        }
    }
}
