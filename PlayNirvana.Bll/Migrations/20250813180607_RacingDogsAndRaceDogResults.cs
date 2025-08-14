using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlayNirvana.Bll.Migrations
{
    /// <inheritdoc />
    public partial class RacingDogsAndRaceDogResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RacingDogs",
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

            migrationBuilder.CreateTable(
                name: "RaceDogResults",
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
                        name: "FK_RaceDogResults_RacingDogs_RacingDogId",
                        column: x => x.RacingDogId,
                        principalTable: "RacingDogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaceDogResults_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
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
                table: "RaceDogResults",
                column: "RacingDogId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceDogResults_RoundId",
                table: "RaceDogResults",
                column: "RoundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceDogResults");

            migrationBuilder.DropTable(
                name: "RacingDogs");
        }
    }
}
