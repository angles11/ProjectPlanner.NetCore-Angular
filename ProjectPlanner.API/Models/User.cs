using Microsoft.AspNetCore.Identity;
using System;

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

    }
}
