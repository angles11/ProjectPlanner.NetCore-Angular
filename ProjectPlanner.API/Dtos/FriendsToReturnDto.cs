using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class FriendsToReturnDto
    {
        public ICollection<UserForListDto> PendingFriends { get; set; }
        public ICollection<UserForListDto> AcceptedFriends { get; set; }
        public ICollection<UserForListDto> BlockedFriends { get; set; }
    }
}
