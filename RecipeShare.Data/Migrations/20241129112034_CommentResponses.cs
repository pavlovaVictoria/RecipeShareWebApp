using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true,
                comment: "If the comment is a response -> shows which is the Id of the parent comment");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3d7d3cfd-80a9-42f8-a7c0-0533ee9673ba"), null, "Moderator", "MODERATOR" },
                    { new Guid("7be0dbb0-3b3d-4ca3-a37b-84cc2e1381e3"), null, "User", "USER" },
                    { new Guid("92865ff1-22a3-4bcf-8434-f86a777c3473"), null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

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

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

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
    }
}
