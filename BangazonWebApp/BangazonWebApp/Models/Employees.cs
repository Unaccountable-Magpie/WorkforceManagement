//Author: Lauren Richert
//Purpose: To reference the Employees table and the values

using BangazonWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWebApp.Models
{
    public class Employees
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Is Supervisor")]
        public bool Supervisor { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentsId { get; set; }
        public Departments Departments { get; set; }

        [Required]
        [Display(Name = "Current Computer")]
        public int ComputersId { get; set; }
        public Computers Computers { get; set; }


      
        public List<TrainingPrograms> TrainingPrograms { get; set; } = new List<TrainingPrograms>();


    }
}

