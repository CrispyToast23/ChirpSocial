using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChirpSocial.Migrations
{
    /// <inheritdoc />
    public partial class AddMentions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mentions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChirpId = table.Column<int>(type: "int", nullable: false),
                    MentionedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mentions_AspNetUsers_MentionedUserId",
                        column: x => x.MentionedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mentions_Chirps_ChirpId",
                        column: x => x.ChirpId,
                        principalTable: "Chirps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_ChirpId",
                table: "Mentions",
                column: "ChirpId");

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_MentionedUserId",
                table: "Mentions",
                column: "MentionedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mentions");
        }
    }
}
