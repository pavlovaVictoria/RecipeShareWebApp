using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentVirtual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3d7d3cfd-80a9-42f8-a7c0-0533ee9673ba"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7be0dbb0-3b3d-4ca3-a37b-84cc2e1381e3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("92865ff1-22a3-4bcf-8434-f86a777c3473"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7134a18f-a655-45dc-9738-018a913d0bb6"), null, "User", "USER" },
                    { new Guid("a0598eac-ad1a-4c31-8076-6851577c7c96"), null, "Moderator", "MODERATOR" },
                    { new Guid("b1e95e55-5f02-4fc8-a9ca-e985f2bc7f1f"), null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7134a18f-a655-45dc-9738-018a913d0bb6"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a0598eac-ad1a-4c31-8076-6851577c7c96"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b1e95e55-5f02-4fc8-a9ca-e985f2bc7f1f"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3d7d3cfd-80a9-42f8-a7c0-0533ee9673ba"), null, "Moderator", "MODERATOR" },
                    { new Guid("7be0dbb0-3b3d-4ca3-a37b-84cc2e1381e3"), null, "User", "USER" },
                    { new Guid("92865ff1-22a3-4bcf-8434-f86a777c3473"), null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
