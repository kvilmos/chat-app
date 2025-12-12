using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class NewGroupDTO
{
    [Required]
    public required string Name { get; set; }
    public ICollection<int> UserIds { get; set; } = new List<int>();
}
