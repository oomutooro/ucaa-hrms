using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCAA.HRMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeJobLevelAndCbaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobLevel",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobLevel",
                table: "Employees");
        }
    }
}
