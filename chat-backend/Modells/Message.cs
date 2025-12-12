using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{

    [Table("message_t")]

    public class Message
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Text { get; set; }
        public DateTimeOffset Date { get; set; } = DateTime.UtcNow;
        [Required]
        public int? SenderId { get; set; }
        [Required]
        public int? GroupId { get; set; }
        public User? Sender { get; set; }
        public Group? Group { get; set; }

        public Message() { }

        public Message(NewMessageDTO newMessage)
        {
            Text = newMessage.Text;
            SenderId = newMessage.SenderId;
            GroupId = newMessage.GroupId;
        }
    }
}
