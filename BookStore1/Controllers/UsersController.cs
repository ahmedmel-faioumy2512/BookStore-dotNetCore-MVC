using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore1.Data;
using BookStore1.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace BookStore1.Controllers
{
    public class UsersController : Controller
    {
        private readonly BookStore1Context _context;

        public UsersController(BookStore1Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string na, string pa)
        {
            SqlConnection conn1 = new SqlConnection("Data Source=DESKTOP-G0F6A15;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            string sql;
            sql = "SELECT * FROM Users where Name ='" + na + "' and  Password ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string role = (string)reader["Role"];
                string id = Convert.ToString((int)reader["Id"]);
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("UserId", id);
                reader.Close();
                conn1.Close();
                if (role == "Customer")
                    return RedirectToAction("Catalogue", "Books");

                else
                    return RedirectToAction("Index", "Books");

            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,Email")] Users users)
        {
            users.Role = "Customer";
            _context.Add(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            var users = await _context.Users.FindAsync(id);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password,Role,Email")] Users users)
        {
            _context.Update(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
