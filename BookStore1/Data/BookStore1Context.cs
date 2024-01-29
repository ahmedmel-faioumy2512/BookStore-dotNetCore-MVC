using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore1.Models;

namespace BookStore1.Data
{
    public class BookStore1Context : DbContext
    {
        public BookStore1Context (DbContextOptions<BookStore1Context> options)
            : base(options)
        {
        }

        public DbSet<BookStore1.Models.Books> Books { get; set; }

        public DbSet<BookStore1.Models.Users> Users { get; set; }

        public DbSet<BookStore1.Models.Orders> Orders { get; set; }

        public DbSet<BookStore1.Models.Report> Report { get; set; }
    }
}
