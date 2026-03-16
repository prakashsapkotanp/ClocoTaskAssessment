namespace ArtistManagementSystem.Server.DTOs
{
    public class AuthResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
