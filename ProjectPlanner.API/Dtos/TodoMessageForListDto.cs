using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class TodoMessageForListDto
    {
        public string Message { get; set; }
        public string UserId { get; set; }
        public string UserKnownAs { get; set; }
        public string UserPhotoUrl { get; set; }
        public DateTime Created { get; set; }
    }
}
