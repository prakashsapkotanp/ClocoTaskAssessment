using ArtistManagementSystem.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtistManagementSystem.Server.Models
{
    public class MusicModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArtistId { get; set; }

        [ForeignKey(nameof(ArtistId))]
        public ArtistModel Artist { get; set; } = null!;

        [Required]
        public string Title { get; set; }

        public string? AlbumName { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? FilePath { get; set; }


    }
}
