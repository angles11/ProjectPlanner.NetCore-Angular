using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }

        // A new entity is required for a many to many relationship in EF Core.
        public ICollection<Collaboration> Collaborations { get; set; }

        public ICollection<Todo> Todos { get; set; }

        public Project()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
            Collaborations = new List<Collaboration>();
        }
    }
}
