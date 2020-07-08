using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class Friend
    {
        public User UserFriend { get; set; }
        public DateTime Since { get; set; }
        public Status Status { get; set; }
        public string  SentById { get; set; }


    }

}

