using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    [Table("group_t")]
    public class Group
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? CreatorId { get; set; }
        [Required]
        public User? Creator { get; set; }
        public ICollection<User> Members { get; } = new List<User>();
        public ICollection<GroupUserJoin> MembersJoined { get; } = new List<GroupUserJoin>();

        public Group(NewGroupDTO newGroup)
        {
            Name = newGroup.Name;
            CreatorId = newGroup.CreatorId;
        }

        public Group() { }
    }
}
