using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hotvenues.Migrations
{
    public partial class AddedLiveStatusModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiveStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<string>(nullable: true),
                    Media = table.Column<string>(nullable: true),
                    Caption = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveStatuses_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusComment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<long>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusComment_LiveStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "LiveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusComment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveStatuses_VendorId",
                table: "LiveStatuses",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusComment_StatusId",
                table: "StatusComment",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusComment_UserId",
                table: "StatusComment",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusComment");

            migrationBuilder.DropTable(
                name: "LiveStatuses");
        }
    }
}
