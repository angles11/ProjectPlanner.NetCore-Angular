using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class Friendship
    {
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public string RecipientId { get; set; }
        public User Recipient { get; set; }
        public DateTime Since { get; set; }
        public Status Status { get; set; }

        public Friendship()
        {
            Since = DateTime.Now;
            Status = Status.Pending;
        }
    }
}
