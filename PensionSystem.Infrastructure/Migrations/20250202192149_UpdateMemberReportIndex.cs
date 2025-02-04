using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberReportIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyContributionReports_MemberId",
                table: "MonthlyContributionReports");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyContributionReports_MemberId_ReportDate",
                table: "MonthlyContributionReports",
                columns: new[] { "MemberId", "ReportDate" },
                unique: true,
                filter: "[MemberId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyContributionReports_MemberId_ReportDate",
                table: "MonthlyContributionReports");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyContributionReports_MemberId",
                table: "MonthlyContributionReports",
                column: "MemberId",
                unique: true,
                filter: "[MemberId] IS NOT NULL");
        }
    }
}
