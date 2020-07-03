using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class FriendToReturnDto
    {
        public string FriendId { get; set; }
        public string KnownAs { get; set; }
        public DateTime LastActive { get; set; }
        public string Position { get; set; }
        public string Employer { get; set; }
        public string Experience { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public Status Status { get; set; }
    }
}
