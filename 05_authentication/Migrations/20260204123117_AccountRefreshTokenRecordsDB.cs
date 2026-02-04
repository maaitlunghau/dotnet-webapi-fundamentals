using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _05_authentication.Migrations
{
    /// <inheritdoc />
    public partial class AccountRefreshTokenRecordsDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokenRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenJti = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReplaceByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokeAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokenRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokenRecords_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokenRecords_AccountId",
                table: "RefreshTokenRecords",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokenRecords_Id",
                table: "RefreshTokenRecords",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokenRecords");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
