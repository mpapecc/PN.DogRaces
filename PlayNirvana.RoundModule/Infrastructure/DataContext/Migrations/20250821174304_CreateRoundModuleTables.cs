using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayNirvana.RoundModule.Infrastructure.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class CreateRoundModuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rounds");

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoundStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceDogResults",
                schema: "rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RacingDogId = table.Column<int>(type: "int", nullable: false),
                    RoundId = table.Column<int>(type: "int", nullable: false),
                    Place = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceDogResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceDogResults_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "rounds",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceDogResults_RoundId",
                schema: "rounds",
                table: "RaceDogResults",
                column: "RoundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceDogResults",
                schema: "rounds");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "rounds");
        }
    }
}
