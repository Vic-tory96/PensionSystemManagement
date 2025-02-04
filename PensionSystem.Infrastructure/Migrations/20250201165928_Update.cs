using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Benefits_Member_MemberId",
                table: "Benefits");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_Member_MemberId",
                table: "Contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_Member_Employers_EmployerId",
                table: "Member");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_Benefits_BenefitId",
                table: "TransactionHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.RenameTable(
                name: "TransactionHistory",
                newName: "TransactionHistories");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "Members");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionHistory_BenefitId",
                table: "TransactionHistories",
                newName: "IX_TransactionHistories_BenefitId");

            migrationBuilder.RenameIndex(
                name: "IX_Member_EmployerId",
                table: "Members",
                newName: "IX_Members_EmployerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistories",
                table: "TransactionHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Benefits_Members_MemberId",
                table: "Benefits",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_Members_MemberId",
                table: "Contributions",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Employers_EmployerId",
                table: "Members",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Benefits_BenefitId",
                table: "TransactionHistories",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Benefits_Members_MemberId",
                table: "Benefits");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_Members_MemberId",
                table: "Contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Employers_EmployerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Benefits_BenefitId",
                table: "TransactionHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistories",
                table: "TransactionHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.RenameTable(
                name: "TransactionHistories",
                newName: "TransactionHistory");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Member");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionHistories_BenefitId",
                table: "TransactionHistory",
                newName: "IX_TransactionHistory_BenefitId");

            migrationBuilder.RenameIndex(
                name: "IX_Members_EmployerId",
                table: "Member",
                newName: "IX_Member_EmployerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Benefits_Member_MemberId",
                table: "Benefits",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_Member_MemberId",
                table: "Contributions",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Employers_EmployerId",
                table: "Member",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_Benefits_BenefitId",
                table: "TransactionHistory",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
