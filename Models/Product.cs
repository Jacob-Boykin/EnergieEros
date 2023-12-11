using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergieEros.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        public string? Category { get; set; }
    }
}