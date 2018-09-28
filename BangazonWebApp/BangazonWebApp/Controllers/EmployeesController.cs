using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWebApp.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BangazonWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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

        // GET: api/Employees/5
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = "SELECT * FROM Employees";

                var AllEmployees = await conn.QueryAsync<Employees>(sql);
                return Ok(AllEmployees);
            }

        }

        //get a single employee
        [HttpGet("{id}", Name = "GetEmployees")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $"SELECT * FROM Employees WHERE Id = {id}";

                var theSingleEmployee = (await conn.QueryAsync<Employees>(sql)).Single();
                return Ok(theSingleEmployee);
            }
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employees Employees)
        {
            string sql = $@"INSERT INTO Employees
            ( FirstName, LastName, DepartmentsId, ComputersId, TrainingProgramsId)
            VALUES
            ('{Employees.FirstName}','{Employees.LastName}', '{Employees.DepartmentsId}', '{Employees.ComputersId}', 
        {Employees.TrainingProgramsId});
            select MAX(Id) from Employees";

            using (IDbConnection conn = Connection)
            {
                var newEmployeeId = (await conn.QueryAsync<int>(sql)).Single();
                Employees.Id = newEmployeeId;
                return CreatedAtRoute("GetEmployees", new { id = newEmployeeId }, Employees);
            }

        }
        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Employees Employees)
        {
            string sql = $@"
            UPDATE Employees
            SET 
                FirstName = '{Employees.FirstName}',
                LastName = '{Employees.LastName}',
                DepartmentsId = {Employees.DepartmentsId},
                ComputersId = '{Employees.ComputersId},
                TrainingProgramsId = {Employees.TrainingProgramsId}
            WHERE Id = {id}";

            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {
                if (!EmployeesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            string sql = $@"DELETE FROM EmployeeComputers WHERE EmployeesId = {id};
               DELETE FROM Employees WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                throw new Exception("No rows affected");
            }

        }
        private bool EmployeesExists(int id)
        {
            string sql = $"SELECT DepartmentsId, FirstName, LastName, Supervisor FROM Employees WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Employees>(sql).Count() > 0;
            }
        }
    }
}