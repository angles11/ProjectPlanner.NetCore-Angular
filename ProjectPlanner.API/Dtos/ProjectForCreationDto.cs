using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class ProjectForCreationDto
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string OwnerId { get; set; }

    }
}
