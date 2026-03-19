using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.DTOs
{
    public class ArtistDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime Dob { get; set; }

        /// <summary>m = Male, f = Female, o = Other</summary>
        public string Gender { get; set; } = string.Empty;

        public string? Address { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? Phone { get; set; }

        public int FirstReleaseYear { get; set; }

        public int NoOfAlbumsReleased { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MinLength(6)]
        public string? Password { get; set; }
    }
}
