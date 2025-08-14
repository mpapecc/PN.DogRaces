using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayNirvana.Bll.Migrations
{
    /// <inheritdoc />
    public partial class AlterSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BetStatus",
                table: "Bets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetStatus",
                table: "Bets");
        }
    }
}
