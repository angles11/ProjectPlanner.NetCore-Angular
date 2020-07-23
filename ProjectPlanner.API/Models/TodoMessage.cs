using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class TodoMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int TodoId { get; set; }
        public Todo Todo { get; set; }
        public DateTime Created { get; set; }

        public TodoMessage()
        {
            Created = DateTime.Now;
        }
    }
}
