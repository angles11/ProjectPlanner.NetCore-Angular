using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<Todo> Todos { get; set; }

        public DbSet<TodoMessage> TodoMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(P => P.OwnerId);

            builder.Entity<Project>()
                .HasMany(p => p.Todos)
                .WithOne(t => t.Project);

            builder.Entity<Todo>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Todos)
                .HasForeignKey(t => t.ProjectId);

            builder.Entity<Todo>()
                .HasMany(t => t.Messages)
                .WithOne(td => td.Todo);

            builder.Entity<TodoMessage>()
                .HasOne(tm => tm.Todo)
                .WithMany(t => t.Messages)
                .HasForeignKey(t => t.TodoId);

            builder.Entity<TodoMessage>()
            .HasOne(tm => tm.User)
            .WithMany(u => u.TodoMessages)
            .HasForeignKey(tm => tm.UserId);

            builder.Entity<Collaboration>()
                  .HasKey(c => new { c.UserId, c.ProjectId }); //Composite key

            builder.Entity<Collaboration>()
                    .HasOne(c => c.User)
                    .WithMany(u => u.CollaboratedProjects)
                    .HasForeignKey(c => c.UserId);

            builder.Entity<Collaboration>()
                .HasOne(c => c.Project)
                .WithMany(p => p.Collaborations)
                .HasForeignKey(c => c.ProjectId);

            builder.Entity<Friendship>()
                .HasKey(f => new { f.SenderId, f.RecipientId }); //Composite key

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
