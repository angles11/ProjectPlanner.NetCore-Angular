using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class UserForListDto
    {
        public string Id { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
    }
}
