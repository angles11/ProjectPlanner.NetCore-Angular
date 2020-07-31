using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class Collaboration
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

    }
}
