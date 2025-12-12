using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class NewMessageDTO
{
    [Required]
    public required string Text { get; set; }

    [Required]
    public required int SenderId { get; set; }

    [Required]
    public required int GroupId { get; set; }
}
