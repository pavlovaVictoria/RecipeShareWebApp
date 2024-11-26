using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class Attribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0904fcd8-6b81-45aa-9c9b-2c0b3bcaa2fd"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2f7c5b8a-91fa-4352-abcf-6079b15ef0c5"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f3434fa0-acf0-4d8a-abe4-109f7dd9d732"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("5314c82c-96b5-4754-9226-703ffb397842"), null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("886a75d6-62d2-4a20-9db4-af9515511abb"), null, "User", "USER" },
                    { new Guid("8af7116b-0850-411d-aeb1-cd8a9c32b68c"), null, "Moderator", "MODERATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5314c82c-96b5-4754-9226-703ffb397842"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("886a75d6-62d2-4a20-9db4-af9515511abb"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8af7116b-0850-411d-aeb1-cd8a9c32b68c"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0904fcd8-6b81-45aa-9c9b-2c0b3bcaa2fd"), null, "User", "USER" },
                    { new Guid("2f7c5b8a-91fa-4352-abcf-6079b15ef0c5"), null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("f3434fa0-acf0-4d8a-abe4-109f7dd9d732"), null, "Moderator", "MODERATOR" }
                });
        }
    }
}
