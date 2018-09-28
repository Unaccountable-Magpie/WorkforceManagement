//Authored By: Michael Roberts
//Purpose: The Departments model will allow a user to pull up a list of Departments.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BangazonWebApp.Controllers
{
    public class Departments
    {

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        public string Budget { get; set; }

        List<Employees> EmployeesList = new List<Employees>();

    }


}

