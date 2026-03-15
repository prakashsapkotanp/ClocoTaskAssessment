using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtistManagementSystem.Server.Models
{
    public class UserRoleModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserModel? User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public RoleModel? Role { get; set; }
    }
}
