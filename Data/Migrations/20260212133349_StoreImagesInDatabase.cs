using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChirpSocial.Migrations
{
    /// <inheritdoc />
    public partial class StoreImagesInDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ChirpImages",
                newName: "ContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "ChirpImages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "ChirpImages");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "ChirpImages",
                newName: "FilePath");
        }
    }
}
