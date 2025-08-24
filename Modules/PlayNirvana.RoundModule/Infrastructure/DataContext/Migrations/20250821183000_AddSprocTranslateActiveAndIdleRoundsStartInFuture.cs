using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayNirvana.RoundModule.Infrastructure.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddSprocTranslateActiveAndIdleRoundsStartInFuture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR ALTER   PROCEDURE [rounds].[sproc_TranslateActiveAndIdleRoundsStartInFuture]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @nowUtc datetime2(0) = SYSUTCDATETIME();

                    DECLARE @nextWholeMinuteUtc datetime2(0) =
                        DATEADD(minute, 1, DATEADD(minute, DATEDIFF(minute, 0, @nowUtc), 0));

                    DECLARE @nextWholeEvenMinuteUtc datetime2(0) =
                        DATEADD(minute,
                                CASE WHEN DATEPART(minute, @nextWholeMinuteUtc) % 2 = 1 THEN 1 ELSE 0 END,
                                @nextWholeMinuteUtc);

                    ;WITH numbered AS (
                        SELECT
                            e.Id,
                            rn = ROW_NUMBER() OVER (ORDER BY e.Id) - 1
                        FROM [rounds].[Rounds] AS e
                        WHERE
                            e.RoundStatus = 0 OR e.RoundStatus = 1
                    )

                    UPDATE e
                        SET e.Start = DATEADD(minute, 2 * n.rn, @nextWholeEvenMinuteUtc)
                    FROM [rounds].[Rounds] AS e
                    INNER JOIN numbered AS n ON n.Id = e.Id;
                END
                """
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
