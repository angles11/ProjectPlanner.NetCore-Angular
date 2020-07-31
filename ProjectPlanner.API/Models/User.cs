using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjectPlanner.API.Models
{
    public class User : IdentityUser
    {
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }      
        public string Position { get; set; }
        public string Employer { get; set; }
        public string Experience { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public virtual ICollection<Project> OwnedProjects { get; set; }

        //A new entity is required for a many to many relationship in EF Core.
        public virtual ICollection<Collaboration> CollaboratedProjects { get; set; }

        public virtual ICollection<Friendship> FriendshipSent { get; set; }

        public virtual ICollection<Friendship> FriendshipReceived { get; set; }

        public virtual ICollection<TodoMessage> TodoMessages { get; set; }

    }
}
