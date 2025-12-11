namespace ChatApp.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<BaseGroupDTO> Groups { get; set; } = new List<BaseGroupDTO>();

    public UserDTO(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Groups = user.Groups.Select(g => new BaseGroupDTO(g)).ToList();
    }
}
