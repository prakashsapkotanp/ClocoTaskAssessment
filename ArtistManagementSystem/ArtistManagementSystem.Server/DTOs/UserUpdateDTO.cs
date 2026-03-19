using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.DTOs
{
    public class UserUpdateDTO
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        public string? Phone { get; set; }
        public string? Address { get; set; }
        
        [Required]
        public DateTime Dob { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        public string? Password { get; set; }
    }
}
