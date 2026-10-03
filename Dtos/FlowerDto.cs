using System.ComponentModel.DataAnnotations;
namespace FlowerShop.Dtos
{
    public class FlowerDto
    {
        public int Id 
        { 
            get; 
            set; 
        }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string Name 
        { 
            get; 
            set; 
        }
        [Required]
        [Range(0.01, 100000)]
        public decimal Price 
        { 
            get; 
            set; 
        }
        public string CategoryName 
        { 
            get; 
            set; 
        }
        public string SupplierName 
        { 
            get; 
            set; 
        }
    }
    public class FlowerRequest
    {
        public string Name 
        { 
            get; 
            set; 
        }
        public decimal Price 
        { 
            get; 
            set; 
        }
        public int CategoryId 
        { 
            get; 
            set; 
        }
        public int SupplierId 
        { 
            get; 
            set; 
        }
    }
}