using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public string? Phone { get; set; }
        public string? Address { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        [Required]
        public DateTime Dob { get; set; }
        
        [Required]
        public string Role { get; set; }
    }
}
