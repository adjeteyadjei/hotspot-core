using Microsoft.EntityFrameworkCore.Migrations;

namespace hotvenues.Migrations
{
    public partial class AddedMediaExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaExtension",
                table: "LiveStatuses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaExtension",
                table: "LiveStatuses");
        }
    }
}
