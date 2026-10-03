using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models;
public class Flower
{
    public int Id { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters allowed")]
    public string Name { get; set; }
    [Required]
    [Range(0.01, 100000)]
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
