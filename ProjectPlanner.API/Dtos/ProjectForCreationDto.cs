using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class ProjectForCreationDto
    {

        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string OwnerId { get; set; }

    }
}
