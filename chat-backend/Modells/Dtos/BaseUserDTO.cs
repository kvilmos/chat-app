namespace ChatApp.Models;

public class BaseUserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }

    public BaseUserDTO(User user)
    {
        Id = user.Id;
        Name = user.Name;
    }
}
