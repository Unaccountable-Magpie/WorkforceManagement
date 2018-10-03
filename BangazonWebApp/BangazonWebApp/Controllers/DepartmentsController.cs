//Author: Michael Roberts
//Purpose: To list the employees to their departments when you select the Details of the Departments.

using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using BangazonWebApp.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using BangazonWebApp.Models;

namespace Workforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Departments> departments = await conn.QueryAsync<Departments>(
                    "SELECT Id, Name FROM Departments;"
                );
                return View(departments);
            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
           Select
               d.Id,
               d.Name,
               d.Budget,
               e.Id,
               e.FirstName,
               e.LastName,
               e.DepartmentsId
            from Departments d
			JOIN Employees e ON d.Id = e.DepartmentsId
            WHERE d.Id = {id}
            ";
            using (IDbConnection conn = Connection)
            {

                var departmentsDictionary = new Dictionary<int, Departments>();


                var list = await conn.QueryAsync<Departments, Employees, Departments>(
                sql,
                (departments, employees) =>
                {
                    Departments departmentsEntry;

                    if (!departmentsDictionary.TryGetValue(departments.Id, out departmentsEntry))
                    {
                        departmentsEntry = departments;
                        departmentsEntry.EmployeesList = new List<Employees>();
                        departmentsDictionary.Add(departmentsEntry.Id, departmentsEntry);
                    }

                    departmentsEntry.EmployeesList.Add(employees);
                    return departmentsEntry;

                });
                return View(list.Distinct().First());
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([Bind ("DepartmentId, Name, Budget")]Departments departments)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Departments
                        ( Name, Budget)
                        VALUES
                        ( null,
                             '{departments.Name}'
                            , '{departments.Budget}'
                            
                        )
                    ";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(departments);


        }


    }
}