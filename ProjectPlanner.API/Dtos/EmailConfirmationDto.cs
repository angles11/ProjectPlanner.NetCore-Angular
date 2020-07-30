using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class EmailConfirmationDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
