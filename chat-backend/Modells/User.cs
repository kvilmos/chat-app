using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    [Table("user_t")]
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<GroupUserJoin> GroupsJoined { get; set; } = new List<GroupUserJoin>();

        public User() { }

        public User(NewUserDTO newUser)
        {
            Name = newUser.Name;
        }
    }
}
