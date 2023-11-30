using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public ICollection<OrderProduct>? OrderProducts { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
