using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class RenamePlanMonthlyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MontlyFee",
                table: "Plans",
                newName: "MonthlyFee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlyFee",
                table: "Plans",
                newName: "MontlyFee");
        }
    }
}
