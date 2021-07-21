using Microsoft.EntityFrameworkCore.Migrations;

namespace Time_Sheet_Buddy.Migrations
{
    public partial class UpdatetheDatabsewiththeright : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Assignie",
                schema: "Identity",
                table: "Issue",
                newName: "Assignee");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                schema: "Identity",
                table: "Issue",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Stetes",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stetes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stetes",
                schema: "Identity");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                schema: "Identity",
                table: "Issue");

            migrationBuilder.RenameColumn(
                name: "Assignee",
                schema: "Identity",
                table: "Issue",
                newName: "Assignie");
        }
    }
}
