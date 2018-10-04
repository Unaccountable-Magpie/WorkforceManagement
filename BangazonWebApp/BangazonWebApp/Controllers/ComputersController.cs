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
using System;

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
        public IActionResult Index()
        {
            return View();
        }

















        public async Task<IActionResult> DeleteConfirm(int? id)
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
                    c.Malfunctioned
                from Computers c
                WHERE c.Id = {id}";

            using (IDbConnection conn = Connection)
            {

                Computers computers = (await conn.QueryAsync<Computers>(sql)).ToList().Single();

                if (computers == null)
                {
                    return NotFound();
                }

                return View(computers);
            }
        }




        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            string sql = $@"DELETE FROM Computers WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                throw new Exception("No rows affected");
            }
        }


    }
}