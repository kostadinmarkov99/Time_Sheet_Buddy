using Microsoft.EntityFrameworkCore.Migrations;

namespace Time_Sheet_Buddy.Migrations
{
    public partial class UpdateIdeaEntityandControllers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThemesPicture",
                schema: "Identity",
                table: "Ideas",
                newName: "IdeaPicture");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdeaPicture",
                schema: "Identity",
                table: "Ideas",
                newName: "ThemesPicture");
        }
    }
}
