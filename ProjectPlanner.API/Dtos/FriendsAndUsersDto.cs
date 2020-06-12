using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class FriendsAndUsersDto
    {
       public ICollection<UserForListDto> Friends { get; set; }
        public ICollection<UserForListDto> Users { get; set; }
    }
}
