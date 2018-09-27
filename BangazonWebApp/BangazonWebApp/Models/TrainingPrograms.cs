//Author: Lauren Richert
//Purpose: To reference the TrainingPrograms table and the values

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWebApp.Models
{
    public class TrainingPrograms
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program Name")]
        public string ProgramName { get; set; }

        [Required]
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


    }
}






