using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayNirvana.TicketModule.Infrastructure.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class PaymentReservationIdOnTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentReservationId",
                schema: "tickets",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReservationId",
                schema: "tickets",
                table: "Tickets");
        }
    }
}
