using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hotvenues.Migrations
{
    public partial class AddedLikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "LiveStatuses");

            migrationBuilder.CreateTable(
                name: "StatusLike",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LikeId = table.Column<long>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusLike_LiveStatuses_LikeId",
                        column: x => x.LikeId,
                        principalTable: "LiveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusLike_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusLike_LikeId",
                table: "StatusLike",
                column: "LikeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusLike_UserId",
                table: "StatusLike",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusLike");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "LiveStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
