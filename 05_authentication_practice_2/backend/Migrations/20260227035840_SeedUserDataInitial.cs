using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserDataInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokenRecords_Id",
                table: "RefreshTokenRecords");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAtUTC", "Email", "Name", "Password", "Role", "UpdatedAtUTC" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 27, 3, 58, 39, 655, DateTimeKind.Utc).AddTicks(3850), "trunghau@mstsoftware.vn", "Mai Trung Hau", "$2a$11$sUjZJZ3Bc3y.mS.h0qEhFee7nP0dH8.uAH3TV6WJB39Of2B/G0YyC", "admin", new DateTime(2026, 2, 27, 3, 58, 39, 655, DateTimeKind.Utc).AddTicks(3850) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 2, 27, 3, 58, 39, 753, DateTimeKind.Utc).AddTicks(2990), "john@example.com", "John Doe", "$2a$11$TY2UPwObEPY4Hz.7Bb7MFeGYfBL8t1nLdrhk/n6HDsTq9xh3NswRi", "user", new DateTime(2026, 2, 27, 3, 58, 39, 753, DateTimeKind.Utc).AddTicks(2990) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 2, 27, 3, 58, 39, 853, DateTimeKind.Utc).AddTicks(9800), "jane@example.com", "Jane Smith", "$2a$11$AthO.7/844G2T0U1x3f4VeVhWiecgqNmxC/YBkzjy2pttm2A.884y", "user", new DateTime(2026, 2, 27, 3, 58, 39, 853, DateTimeKind.Utc).AddTicks(9800) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 2, 27, 3, 58, 39, 950, DateTimeKind.Utc).AddTicks(2120), "bob@example.com", "Bob Johnson", "$2a$11$BXhLlh2MYdAYSaF6kh.bK.QmYpiPxRF.wlbMa9sm2AxtG2B58QSJO", "user", new DateTime(2026, 2, 27, 3, 58, 39, 950, DateTimeKind.Utc).AddTicks(2120) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 2, 27, 3, 58, 40, 46, DateTimeKind.Utc).AddTicks(6810), "alice@example.com", "Alice Williams", "$2a$11$4qSfBzrs/FgqvDMu9UmCq.Zm5Luk/xypXTxW.DDcA.m5SGQwlGt5e", "user", new DateTime(2026, 2, 27, 3, 58, 40, 46, DateTimeKind.Utc).AddTicks(6810) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokenRecords_Id",
                table: "RefreshTokenRecords",
                column: "Id",
                unique: true);
        }
    }
}
