using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore1.Models
{
    public class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Info { get; set; }
        public int BookQuantity { get; set; }
        public int Price { get; set; }
        public int CataId { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }

    }
}
