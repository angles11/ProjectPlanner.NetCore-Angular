using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class ProjectForListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime EstimatedDate { get; set; }
        public int  CompletedPercentage { get; set; }
        public string OwnerId { get; set; }
        public UserForListDto Owner { get; set; }
        public ICollection<UserForListDto> Collaborators { get; set; }

        public ICollection<TodoForListDto> Todos { get; set; }

        public TodoMessageForListDto LastMessage { get; set; }

    }
}
