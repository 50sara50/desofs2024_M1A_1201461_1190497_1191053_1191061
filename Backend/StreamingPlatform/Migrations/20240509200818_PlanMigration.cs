using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class PlanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanName",
                table: "Plans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_PlanName",
                table: "Plans",
                column: "PlanName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plans_PlanName",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "PlanName",
                table: "Plans");
        }
    }
}
