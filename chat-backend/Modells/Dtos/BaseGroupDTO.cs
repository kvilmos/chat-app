namespace ChatApp.Models;

public class BaseGroupDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public BaseGroupDTO(Group group)
    {
        Id = group.Id;
        Name = group.Name;
    }
}
