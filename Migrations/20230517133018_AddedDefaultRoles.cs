using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hatebook.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5efe55dc-8154-465a-9373-687f3c438085", "c0376f40-cda3-4a6c-a12d-92d0ae7ca507", "Administrator", "ADMINISTRATOR" },
                    { "e4746cb2-614b-4d7d-a5c3-1fddd0256cc4", "cbc83cb7-8d87-483c-9550-0015da4bd7f3", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5efe55dc-8154-465a-9373-687f3c438085");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4746cb2-614b-4d7d-a5c3-1fddd0256cc4");
        }
    }
}
