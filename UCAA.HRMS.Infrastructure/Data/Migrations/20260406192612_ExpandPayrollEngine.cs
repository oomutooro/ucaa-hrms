using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCAA.HRMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPayrollEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayrollRecords_EmployeeId",
                table: "PayrollRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PayrollRecords",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Deductions",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "BasicSalary",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Allowances",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossPay",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HousingAllowance",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanDeduction",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetPay",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAllowance",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherDeduction",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAtUtc",
                table: "PayrollRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PayeTax",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PensionDeduction",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PayrollRecords",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "TransportAllowance",
                table: "PayrollRecords",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"
                UPDATE ""PayrollRecords""
                SET
                    ""TransportAllowance"" = 0,
                    ""HousingAllowance"" = 0,
                    ""OtherAllowance"" = ""Allowances"",
                    ""PayeTax"" = 0,
                    ""PensionDeduction"" = 0,
                    ""LoanDeduction"" = 0,
                    ""OtherDeduction"" = ""Deductions"",
                    ""GrossPay"" = ""BasicSalary"" + ""Allowances"",
                    ""NetPay"" = ""BasicSalary"" + ""Allowances"" - ""Deductions"",
                    ""Status"" = 1
            ");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRecords_EmployeeId_PayPeriod",
                table: "PayrollRecords",
                columns: new[] { "EmployeeId", "PayPeriod" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayrollRecords_EmployeeId_PayPeriod",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "GrossPay",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "HousingAllowance",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "LoanDeduction",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "NetPay",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "OtherAllowance",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "OtherDeduction",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "PayeTax",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "PensionDeduction",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PayrollRecords");

            migrationBuilder.DropColumn(
                name: "TransportAllowance",
                table: "PayrollRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PayrollRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Deductions",
                table: "PayrollRecords",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BasicSalary",
                table: "PayrollRecords",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Allowances",
                table: "PayrollRecords",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRecords_EmployeeId",
                table: "PayrollRecords",
                column: "EmployeeId");
        }
    }
}
