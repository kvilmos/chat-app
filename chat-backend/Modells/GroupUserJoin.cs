using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    [Table("group_user_j")]
    public class GroupUserJoin
    {
        [Required]
        public required int GroupId { get; set; }

        public Group? Group { get; set; }

        [Required]
        public required int UserId { get; set; }

        public User? User { get; set; }
    }
}