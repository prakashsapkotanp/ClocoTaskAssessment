using System.ComponentModel.DataAnnotations;

namespace ArtistManagementSystem.Server.DTOs
{
    public class MusicDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? AlbumName { get; set; }

        /// <summary>Rnb, Country, Classic, Rock, Jazz</summary>
        [Required]
        public string Genre { get; set; } = string.Empty;

        public string? FilePath { get; set; }
    }
}
