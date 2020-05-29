using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The username must be between 3 and 15 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 20 characters.")]
        public string Password { get; set; }
    }
}
