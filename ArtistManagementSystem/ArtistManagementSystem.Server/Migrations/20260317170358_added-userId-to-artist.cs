using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistManagementSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class addeduserIdtoartist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "artist",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "artist");
        }
    }
}
