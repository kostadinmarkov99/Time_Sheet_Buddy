using Microsoft.EntityFrameworkCore.Migrations;

namespace Time_Sheet_Buddy.Migrations
{
    public partial class UpdateleftandtopstyleofIdeasentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeftStyle",
                schema: "Identity",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopStyle",
                schema: "Identity",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftStyle",
                schema: "Identity",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "TopStyle",
                schema: "Identity",
                table: "Ideas");
        }
    }
}
