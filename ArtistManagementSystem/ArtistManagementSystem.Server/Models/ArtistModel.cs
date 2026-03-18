using ArtistManagementSystem.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.Models
{
    public class ArtistModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        public Gender Gender { get; set; }

        public string? Address { get; set; }

        public int FirstReleaseYear { get; set; }

        public int NoOfAlbumsReleased { get; set; }

        public int? UserId { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }
    }
}
