namespace ChatApp.Models;

public class GroupDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public BaseUserDTO? Creator { get; set; }
    public ICollection<BaseUserDTO> Members { get; set; } = new List<BaseUserDTO>();

    public GroupDTO(Group group)
    {
        Id = group.Id;
        Name = group.Name;

        if (group.Creator != null)
        {
            Creator = new BaseUserDTO(group.Creator);
        }

        if (group.Members != null)
        {
            Members = group.Members.Select(t => new BaseUserDTO(t)).ToList();
        }
    }
}
