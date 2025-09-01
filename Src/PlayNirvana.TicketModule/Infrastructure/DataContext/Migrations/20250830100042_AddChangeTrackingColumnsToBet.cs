using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayNirvana.TicketModule.Infrastructure.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeTrackingColumnsToBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "tickets",
                table: "Bets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                schema: "tickets",
                table: "Bets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "tickets",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                schema: "tickets",
                table: "Bets");
        }
    }
}
