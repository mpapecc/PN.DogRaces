using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlayNirvana.RoundModule.Infrastructure.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class RacingDogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RacingDogs",
                schema: "rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacingDogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "rounds",
                table: "RacingDogs",
                columns: new[] { "Id", "Name", "Number" },
                values: new object[,]
                {
                    { 1, "Dogo1", 1 },
                    { 2, "Dogo2", 2 },
                    { 3, "Dogo3", 3 },
                    { 4, "Dogo4", 4 },
                    { 5, "Dogo5", 5 },
                    { 6, "Dogo6", 6 },
                    { 7, "Dogo7", 7 },
                    { 8, "Dogo8", 8 },
                    { 9, "Dogo9", 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceDogResults_RacingDogId",
                schema: "rounds",
                table: "RaceDogResults",
                column: "RacingDogId");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceDogResults_RacingDogs_RacingDogId",
                schema: "rounds",
                table: "RaceDogResults",
                column: "RacingDogId",
                principalSchema: "rounds",
                principalTable: "RacingDogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceDogResults_RacingDogs_RacingDogId",
                schema: "rounds",
                table: "RaceDogResults");

            migrationBuilder.DropTable(
                name: "RacingDogs",
                schema: "rounds");

            migrationBuilder.DropIndex(
                name: "IX_RaceDogResults_RacingDogId",
                schema: "rounds",
                table: "RaceDogResults");
        }
    }
}
