﻿using System;
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
    public class EmployeesController : Controller
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


        public async Task<IActionResult> Index()
        {

            string sql = @"
                select
                     e.Id,
                     e.FirstName,
                     e.LastName,
                     e.DepartmentsId,
        	d.Id,
        	d.Name
                 from Employees e
        JOIN Departments d ON e.DepartmentsId = d.Id;
             ";

            using (IDbConnection conn = Connection)
            {
                Dictionary<int, Employees> employeeList = new Dictionary<int, Employees>();

                var EmployeeQuerySet = await conn.QueryAsync<Employees, Departments, Employees>(
                        sql,
                        (employee, department) =>
                        {
                            if (!employeeList.ContainsKey(employee.Id))
                            {
                                employeeList[employee.Id] = employee;
                            }
                            employeeList[employee.Id].Departments = department;
                            return employee;
                        }
                    );
                return View(employeeList.Values);

            }
        }

        //details view


        public async Task<IActionResult> Details(int? id)
        {


            string sql = $@"
        select
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentsId,
                d.Id,
                d.Name,
                d.Budget,
                c.Id,
                c.DatePurchased,
                c.DecommissionedDate,
                c.Malfunctioned,
                t.Id,
                t.ProgramName,
                t.MaxAttendees,
                t.StartDate,
                t.EndDate
            FROM Employees e
            JOIN Departments d ON e.DepartmentsId = d.Id
            JOIN EmployeeComputers ec ON ec.EmployeesId = e.Id
            JOIN Computers c ON ec.ComputersId = c.Id
            JOIN EmployeeTrainingPrograms et ON et.EmployeesId = e.Id
            JOIN TrainingPrograms t ON et.TrainingProgramsId = t.Id
            WHERE e.Id = {id}";




            using (IDbConnection conn = Connection)
            {

                var employeesDictionary = new Dictionary<int, Employees>();


                var list = conn.Query<Employees, Departments,Computers,TrainingPrograms, Employees>(
                    sql,
                    (employees, departments, computers,trainingPrograms) =>
                    {
                        Employees employeesEntry;

                        if (!employeesDictionary.TryGetValue(employees.Id, out employeesEntry))
                        {
                            employeesEntry = employees;
                            employeesEntry.Computers = computers;
                            employeesEntry.Departments = departments;
                            employeesEntry.TrainingPrograms = new List<TrainingPrograms>();
                            employeesDictionary.Add(employeesEntry.Id, employeesEntry);
                        }

                        employeesEntry.TrainingPrograms.Add(trainingPrograms);
                        return employeesEntry;
                    })
                .Distinct()
                .First();

                return View(list);

            }
        }




        //get a single employee
        //[HttpGet("{id}", Name = "GetEmployees")]
        //public async Task<IActionResult> Get([FromRoute] int id)
        //{
        //    using (IDbConnection conn = Connection)
        //    {
        //        string sql = $"SELECT * FROM Employees WHERE Id = {id}";

        //        var theSingleEmployee = (await conn.QueryAsync<Employees>(sql)).Single();
        //        return Ok(theSingleEmployee);
        //    }
        //}

        // POST: api/Employees
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employees Employees)
        {
            string sql = $@"INSERT INTO Employees
            ( FirstName, LastName, Supervisor, DepartmentsId, Computers )
            VALUES
            ('{Employees.FirstName}','{Employees.LastName}', '{Employees.Supervisor}', '{Employees.DepartmentsId}', '{Employees.Computers}' );
            select MAX(Id) from Employees";

            using (IDbConnection conn = Connection)
            {
                var newEmployeeId = (await conn.QueryAsync<int>(sql)).Single();
                Employees.Id = newEmployeeId;
                return CreatedAtRoute("GetEmployees", new { id = newEmployeeId }, Employees);
            }

        }

    }
}