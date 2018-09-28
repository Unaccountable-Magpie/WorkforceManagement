//Author: Austin Gorman
//Purpose: To reference the Computers table and the values


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWebApp.Models
{
    public class Computers
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Date Purchased")]
        [Required]
        public DateTime DatePurchased { get; set; }

        [Display(Name = "Decommissioned Date")]
        public DateTime DecommissionedDate { get; set; }
        [Display(Name = "Needs Repair?")]
        [Required]
        public bool Malfunctioned { get; set; }
    }
}
