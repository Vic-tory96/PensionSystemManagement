using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MonthlyContributionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEligibleForBenefit",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Interest",
                table: "Contributions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MonthlyContributionReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalContributions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsEligibleForBenefit = table.Column<bool>(type: "bit", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyContributionReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyContributionReports");

            migrationBuilder.DropColumn(
                name: "IsEligibleForBenefit",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Interest",
                table: "Contributions");
        }
    }
}
