using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class FriendshipForReturnDto
    {
        public string SenderId { get; set; }
        public UserForListDto Sender { get; set; }
        public string RecipientId { get; set; }
        public UserForListDto Recipient { get; set; }
        public DateTime Since { get; set; }
        public Status Status { get; set; }
    }
}
