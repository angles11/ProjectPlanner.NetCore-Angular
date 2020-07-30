using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class UserForEditDto
    {

        [Required]
        public string Gender { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "The knownAs must be between 8 and 15 characters.")]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]

        public string Country { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Employer { get; set; }
        [Required]
        public string Experience { get; set; }

    }
}
