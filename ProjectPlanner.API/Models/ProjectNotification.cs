using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class ProjectNotification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }

    }
}
