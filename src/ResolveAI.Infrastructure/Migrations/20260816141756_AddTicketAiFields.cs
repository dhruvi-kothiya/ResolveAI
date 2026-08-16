using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResolveAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAiProcessed",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionSummary",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAiProcessed",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ResolutionSummary",
                table: "Tickets");
        }
    }
}
