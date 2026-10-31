using System.ComponentModel.DataAnnotations;
namespace FlowerShop.Dtos
{
    public class AuthResponse
    {
        public string Token 
        { 
            get; 
            set; 
        }
        public string Username 
        { 
            get; 
            set; 
        }
        public string Role 
        { 
            get; 
            set; 
        }
        public int UserId
        {
            get;
            set;
        }
    }
}