using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class NewUserDTO
{
    [Required]
    public required string Name { get; set; }
    public ICollection<int> GroupIds { get; set; } = new List<int>();
}
