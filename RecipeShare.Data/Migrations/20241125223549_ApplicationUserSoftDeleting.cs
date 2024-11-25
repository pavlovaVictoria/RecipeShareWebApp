using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserSoftDeleting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the ApplicationUser is deleted or not -> Soft Deleting");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

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
    }
}
