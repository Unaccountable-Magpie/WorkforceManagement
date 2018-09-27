using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Workforce.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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
            from Department s
            WHERE s.Id = {id}";

            using (IDbConnection conn = Connection)
            {

                Department department = (await conn.QueryAsync<Department>(sql)).ToList().Single();

                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Department
                        ( Name, Budget)
                        VALUES
                        ( 
                             '{department.Name}'
                            , '{department.Budget}'
                            
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


        }


    }
}