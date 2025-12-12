namespace ChatApp.Models;

public class MessageDTO
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset? Date { get; set; }
    public BaseUserDTO? Sender { get; set; }
    public BaseGroupDTO? Group { get; set; }

    public MessageDTO(Message message)
    {
        Id = message.Id;
        Text = message.Text;
        Date = message.Date;
        Sender = new BaseUserDTO(message.Sender);
        Group = new BaseGroupDTO(message.Group);
    }
}
