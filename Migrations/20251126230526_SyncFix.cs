using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 23, 5, 25, 851, DateTimeKind.Utc).AddTicks(2494));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 23, 5, 25, 851, DateTimeKind.Utc).AddTicks(2677));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 26, 23, 5, 26, 33, DateTimeKind.Utc).AddTicks(3243), "$2a$11$8BREIRy4p3f67/l.xXmyv.kFvrbatdJ0sJAj3yBur8CdFAToyosUC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 22, 33, 36, 985, DateTimeKind.Utc).AddTicks(1085));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 22, 33, 36, 985, DateTimeKind.Utc).AddTicks(1262));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 26, 22, 33, 37, 179, DateTimeKind.Utc).AddTicks(5249), "$2a$11$PbRNEiHCfBfzUVDVHrFmQOvrzjXBaGLBeNOKK4pQazHJGcxviUkFu" });
        }
    }
}
