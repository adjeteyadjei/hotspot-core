using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hotvenues.Migrations
{
    public partial class AddedCommentsandLikesModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusComment");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "LiveStatuses");

            migrationBuilder.CreateTable(
                name: "StatusComments",
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
                    table.PrimaryKey("PK_StatusComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusComments_LiveStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "LiveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusLikes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<long>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusLikes_LiveStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "LiveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusComments_StatusId",
                table: "StatusComments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusComments_UserId",
                table: "StatusComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusLikes_StatusId",
                table: "StatusLikes",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusLikes_UserId",
                table: "StatusLikes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusComments");

            migrationBuilder.DropTable(
                name: "StatusLikes");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "LiveStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StatusComment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "IX_StatusComment_StatusId",
                table: "StatusComment",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusComment_UserId",
                table: "StatusComment",
                column: "UserId");
        }
    }
}
