using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeShare.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecipeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the Recipe is approved by the moderator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Recipes");
        }
    }
}
