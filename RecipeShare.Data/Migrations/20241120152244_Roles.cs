using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("067573b2-1946-4d4c-94f3-38953c6ec9b0"), null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("c501b36e-81d6-47ac-94dd-c165baa72733"), null, "User", "USER" },
                    { new Guid("eaa98676-9400-410f-a96d-7914a1a0f4c3"), null, "Moderator", "MODERATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("067573b2-1946-4d4c-94f3-38953c6ec9b0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c501b36e-81d6-47ac-94dd-c165baa72733"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("eaa98676-9400-410f-a96d-7914a1a0f4c3"));
        }
    }
}
