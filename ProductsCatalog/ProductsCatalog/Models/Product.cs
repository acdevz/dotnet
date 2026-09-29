using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductsCatalog.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity {  get; set; }

        [DataType(DataType.DateTime)]
        public DateTime MfgDate { get; set; }

    }
}
