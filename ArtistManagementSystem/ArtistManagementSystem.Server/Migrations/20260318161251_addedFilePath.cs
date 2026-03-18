using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistManagementSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class addedFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "music",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "music");
        }
    }
}
