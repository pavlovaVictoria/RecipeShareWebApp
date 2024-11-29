using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsResponse",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the comment is a Response");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c15114b0-fe2c-425f-a763-a1ed36ecf522"), null, "Moderator", "MODERATOR" },
                    { new Guid("e43c8a16-3f28-4b90-add2-10a64985989a"), null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("e9df4bc0-2970-44c1-bd40-a68fad020c30"), null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c15114b0-fe2c-425f-a763-a1ed36ecf522"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e43c8a16-3f28-4b90-add2-10a64985989a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e9df4bc0-2970-44c1-bd40-a68fad020c30"));

            migrationBuilder.DropColumn(
                name: "IsResponse",
                table: "Comments");

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
    }
}
