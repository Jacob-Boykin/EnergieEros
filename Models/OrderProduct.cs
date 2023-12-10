using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    public class OrderProduct
    {
        public int OrderProductId { get; set; } // Primary key for the OrderProduct entity

        public int OrderId { get; set; } // Foreign key to Order
        public Order? Order { get; set; }

        public int ProductId { get; set; } // Foreign key to Product
        public Product? Product { get; set; }


    }
}
