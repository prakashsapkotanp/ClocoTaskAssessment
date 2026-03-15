using ArtistManagementSystem.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.Models
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public RoleType RoleName { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserRoleModel>? UserRoles { get; set; }
    }
}
