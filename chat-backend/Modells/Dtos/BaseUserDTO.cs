namespace ChatApp.Models;

public class BaseUserDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public BaseUserDTO(User user)
    {
        Id = user.Id;
        Name = user.Name;
    }
}
