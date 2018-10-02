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
                    "SELECT id, Name FROM Departments;"
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
            select
                s.Id,
                s.Name,
                s.Budget,                
            from Departments s
            WHERE s.Id = {id}";

            using (IDbConnection conn = Connection)
            {

                Departments departments = (await conn.QueryAsync<Departments>(sql)).ToList().Single();

                if (departments == null)
                {
                    return NotFound();
                }

                return View(departments);
            }
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create([Bind ("Name, Budget")]Departments departments)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Departments
                        ( Name, Budget)
                        VALUES
                        ( '{departments.Name}'
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