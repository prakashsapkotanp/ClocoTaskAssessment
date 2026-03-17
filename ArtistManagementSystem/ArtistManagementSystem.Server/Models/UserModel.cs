using ArtistManagementSystem.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtistManagementSystem.Server.Models
{
    public class UserModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MaxLength(500)]
        public string Password { get; set; }

        public string? Phone { get; set; }

        public DateTime Dob { get; set; }

        public Gender Gender { get; set; }

        public string? Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public RoleType Role { get; set; }
    }

}
