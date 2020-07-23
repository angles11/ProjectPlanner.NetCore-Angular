using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class TodoForListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string Status { get; set; }
        public string ProjectId { get; set; }
        public ProjectForListDto Project { get; set; }
        public ICollection<TodoMessageForListDto> Messages { get; set; }
    }
}
