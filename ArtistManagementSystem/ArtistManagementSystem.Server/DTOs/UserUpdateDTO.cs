namespace ArtistManagementSystem.Server.DTOs
{
    public class UserUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
    }
}
