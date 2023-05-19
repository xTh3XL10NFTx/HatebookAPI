using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hatebook.Migrations
{
    /// <inheritdoc />
    public partial class GroupAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5efe55dc-8154-465a-9373-687f3c438085");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4746cb2-614b-4d7d-a5c3-1fddd0256cc4");

            migrationBuilder.CreateTable(
                name: "GroupAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0f6e7ab7-3285-43b3-ac35-44d7a42bb64e", "8d5ac8f1-10a9-4da3-9938-edbcee12fe23", "Administrator", "ADMINISTRATOR" },
                    { "345adaaf-e73d-4833-9060-d0e21b8086ac", "8f618ef5-bb6d-4983-942f-9ad827083a0a", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_GroupId",
                table: "GroupAdmins",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_UserId",
                table: "GroupAdmins",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdmins");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f6e7ab7-3285-43b3-ac35-44d7a42bb64e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "345adaaf-e73d-4833-9060-d0e21b8086ac");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5efe55dc-8154-465a-9373-687f3c438085", "c0376f40-cda3-4a6c-a12d-92d0ae7ca507", "Administrator", "ADMINISTRATOR" },
                    { "e4746cb2-614b-4d7d-a5c3-1fddd0256cc4", "cbc83cb7-8d87-483c-9550-0015da4bd7f3", "User", "USER" }
                });
        }
    }
}
