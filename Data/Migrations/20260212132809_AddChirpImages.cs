using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChirpSocial.Migrations
{
    /// <inheritdoc />
    public partial class AddChirpImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChirpImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChirpId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChirpImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChirpImages_Chirps_ChirpId",
                        column: x => x.ChirpId,
                        principalTable: "Chirps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChirpImages_ChirpId",
                table: "ChirpImages",
                column: "ChirpId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChirpImages");
        }
    }
}
