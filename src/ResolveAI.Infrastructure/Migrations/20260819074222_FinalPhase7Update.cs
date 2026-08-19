using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResolveAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalPhase7Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "KnowledgeArticles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "KnowledgeArticles");
        }
    }
}
