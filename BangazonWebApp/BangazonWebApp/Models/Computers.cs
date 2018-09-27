using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWebApp.Models
{
    public class Computers
    {
        public int Id { get; set; }

        [Display(Name = "Date Purchased")]
        public DateTime DatePurchased { get; set; }
        [Display(Name = "Decommissioned Date")]
        [Required]
        public DateTime DecommissionedDate { get; set; }
        [Display(Name = "Needs Repair?")]
        [Required]
        public bool Malfunctioned { get; set; }
    }
}
