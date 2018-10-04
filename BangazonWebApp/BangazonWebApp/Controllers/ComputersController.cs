using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWebApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWebApp.Controllers
{
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
                IEnumerable<Computers> Computers = await conn.QueryAsync<Computers>(
                    "SELECT id FROM Computers;"
                );
                return View(Computers);
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
                c.Id,
                c.DatePurchased,
                c.DecommissionedDate,
                c.Manufacturer,
                c.Make
                from Computers c
            WHERE c.Id = {id}";

            using (IDbConnection conn = Connection)
            {

                Computers Computers = (await conn.QueryAsync<Computers>(sql)).ToList().Single();

                if (Computers == null)
                {
                    return NotFound();
                }

                return View(Computers);
            }
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, DatePurchased, Manufacturer, Make ")]Computers Computers)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Computers
                        ( Id, DatePurchased, Manufacturer, Make)
                        VALUES
                        ( '{Computers.Id}'
                        , '{Computers.DatePurchased}'
                        , '{Computers.Manufacturer}'
                        , '{Computers.Make}'
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

            return View(Computers);


        }


    }
}