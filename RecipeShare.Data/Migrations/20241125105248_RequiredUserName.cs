using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequiredUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("48fec5fa-7892-4af1-8e23-2169856f3591"), null, "Moderator", "MODERATOR" },
                    { new Guid("7004ffe8-1203-47b5-b8d2-4be13bc0f0cc"), null, "User", "USER" },
                    { new Guid("a766d043-0e70-407e-8dbf-02395de8bd98"), null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("48fec5fa-7892-4af1-8e23-2169856f3591"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7004ffe8-1203-47b5-b8d2-4be13bc0f0cc"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a766d043-0e70-407e-8dbf-02395de8bd98"));

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

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
    }
}
