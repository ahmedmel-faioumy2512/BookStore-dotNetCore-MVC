using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore1.Data;
using BookStore1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace BookStore1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookStore1Context _context;

        public OrdersController(BookStore1Context context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(int? id)
        {
            var book = await _context.Books.FindAsync(id);

            return View(book);
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId, int quantity)
        {
            Orders order = new Orders();
            order.BookId = bookId;
            order.Quantity = quantity;
            order.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            order.Orderdate = DateTime.Today;

            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-G0F6A15;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            string sql;
            sql = "UPDATE Books SET BookQuantity = BookQuantity - '" + order.Quantity + "'  where (Id ='" + order.BookId + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();


            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> MyOrders()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var orItems = await _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE UserId = '" + id + "'  ").ToListAsync();
            return View(orItems);
        }

        public async Task<IActionResult> CustomerReport()
        {
            var orItems = await _context.Report.FromSqlRaw("SELECT Users.Id as Id, Name as CustomerName, sum (Quantity * Price)  as Total FROM Books, Orders, Users  WHERE Users.Id = Orders.UserId  and BookId= Books.Id group by Name,Users.id ").ToListAsync();
            return View(orItems);

        }

        public async Task<IActionResult> CustomerOrders(int? id)
        {
            var orItems = await _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE UserId = '" + id + "'  ").ToListAsync();
            return View(orItems);

        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,Quantity,Orderdate")] Orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(orders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
