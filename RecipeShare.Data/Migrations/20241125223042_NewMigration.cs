using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the Comment is deleted or not -> Soft Deleting");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("46648a15-3f51-428d-9752-01bd7817b822"), null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("6d08dfe6-bfc4-443f-b475-b56607f00ef3"), null, "User", "USER" },
                    { new Guid("b1512246-afef-49a8-a925-908ea1746b67"), null, "Moderator", "MODERATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("46648a15-3f51-428d-9752-01bd7817b822"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6d08dfe6-bfc4-443f-b475-b56607f00ef3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b1512246-afef-49a8-a925-908ea1746b67"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comments");

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
    }
}
