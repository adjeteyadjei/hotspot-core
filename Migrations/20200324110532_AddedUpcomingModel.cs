using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hotvenues.Migrations
{
    public partial class AddedUpcomingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpcomingEvents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<string>(nullable: true),
                    Media = table.Column<string>(nullable: true),
                    MediaExtension = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpcomingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UpcomingEvents_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UpcomingEvents_VendorId",
                table: "UpcomingEvents",
                column: "VendorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpcomingEvents");
        }
    }
}
