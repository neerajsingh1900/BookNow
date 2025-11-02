using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_GetShowsForMovieAndCity_Procedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE PROCEDURE [dbo].[GetShowsForMovieAndCity]
            @MovieId INT,
            @CityId INT,
            @StartDate DATETIME,
            @EndDate DATETIME
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT 
                s.ShowId,
                s.MovieId,
                s.ScreenId,
                s.StartTime,
                s.EndTime,
                scr.ScreenId,
                scr.ScreenNumber,  
                t.TheatreId,
                t.TheatreName,
                t.Address
            FROM Shows s
            INNER JOIN Screens scr ON s.ScreenId = scr.ScreenId
            INNER JOIN Theatres t ON scr.TheatreId = t.TheatreId
            WHERE s.MovieId = @MovieId
              AND t.CityId = @CityId
              AND s.StartTime BETWEEN @StartDate AND @EndDate
            ORDER BY s.StartTime;
        END
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetShowsForMovieAndCity];");
        }
    }
}
