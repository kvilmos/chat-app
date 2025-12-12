using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class NewUserDTO
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Password { get; set; }
}
