using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResolveAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeKnowledgeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "KnowledgeArticles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "KnowledgeArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "KnowledgeArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "KnowledgeArticles");
        }
    }
}
