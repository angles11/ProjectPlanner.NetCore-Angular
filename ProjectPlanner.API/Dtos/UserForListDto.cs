using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class UserForListDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public DateTime LastActive{ get; set; }
        public string PhotoUrl { get; set; }
        public string Position { get; set; }
        public string Employer { get; set; }
        public string Experience { get; set; }
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }

    }
}
