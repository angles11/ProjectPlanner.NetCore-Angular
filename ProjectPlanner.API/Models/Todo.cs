using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime EstimatedDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        //a new model is required for a many to many relationship in EF Core.
        public Todo()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
        }
    }
}
