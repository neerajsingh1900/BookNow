CREATE OR ALTER PROCEDURE GetShowsForMovieAndCity
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
        th.TheatreId,
        th.TheatreName,
        th.Address,
        sc.ScreenNumber
    FROM Shows s
    INNER JOIN Screens sc ON s.ScreenId = sc.ScreenId
    INNER JOIN Theatres th ON sc.TheatreId = th.TheatreId
    WHERE 
        s.MovieId = @MovieId AND
        th.CityId = @CityId AND
        s.StartTime BETWEEN @StartDate AND @EndDate
    ORDER BY s.StartTime;
END;
