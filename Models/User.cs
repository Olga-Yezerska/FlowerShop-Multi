using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models;

public class User
{
    public int Id 
    { 
        get; 
        set; 
    }

    [Required]
    public string Username 
    { 
        get; 
        set; 
    } = string.Empty;

    [Required]
    public string PasswordHash 
    { 
        get; 
        set; 
    } = string.Empty;

    [Required]
    public string Role 
    { 
        get; 
        set; 
    } = "User";
}