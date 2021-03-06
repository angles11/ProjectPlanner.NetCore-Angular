﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Dtos
{
    public class ProjectForCreationDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "The title must be between 5 and 20 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "The short description must be between 10 and 50 characters.")]
        public string ShortDescription { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 20, ErrorMessage = "The long description must be between 20 and 300 characters.")]
        public string LongDescription { get; set; }
        [Required]
        public DateTime EstimatedDate { get; set; }
        public string OwnerId { get; set; }

    }
}
