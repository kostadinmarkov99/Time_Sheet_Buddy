using Microsoft.EntityFrameworkCore.Migrations;

namespace Time_Sheet_Buddy.Migrations
{
    public partial class PublicIssuesinBacklog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BacklogId",
                schema: "Identity",
                table: "Issue",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issue_BacklogId",
                schema: "Identity",
                table: "Issue",
                column: "BacklogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Backlogs_BacklogId",
                schema: "Identity",
                table: "Issue",
                column: "BacklogId",
                principalSchema: "Identity",
                principalTable: "Backlogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Backlogs_BacklogId",
                schema: "Identity",
                table: "Issue");

            migrationBuilder.DropIndex(
                name: "IX_Issue_BacklogId",
                schema: "Identity",
                table: "Issue");

            migrationBuilder.DropColumn(
                name: "BacklogId",
                schema: "Identity",
                table: "Issue");
        }
    }
}
