using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.DTOs
{
    public class ArtistDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime Dob { get; set; }

        /// <summary>m = Male, f = Female, o = Other</summary>
        public string Gender { get; set; } = string.Empty;

        public string? Address { get; set; }

        public int FirstReleaseYear { get; set; }

        public int NoOfAlbumsReleased { get; set; }
 
        public int? UserId { get; set; }
    }
}
