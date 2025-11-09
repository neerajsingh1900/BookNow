CREATE PROCEDURE [dbo].[GetMovieRevenueRawData]
    @MovieId INT
AS
BEGIN
    -- Essential for high-performance read queries
    SET NOCOUNT ON;
    
    -- Select the raw, aggregated amount, grouped by the geographical source (Country)
    -- and the currency used in the transaction.
    SELECT
        C.Name AS CountryName,           -- Output for reporting
        C.Code AS CountryCode,           -- Output for flag/geo visualization
        SUM(PT.Amount) AS TotalRawAmount, -- Aggregated sum of payments
        PT.Currency AS TransactionCurrency -- Currency used for conversion in C#
    FROM
        -- 1. Base Data: Payments
        [dbo].[PaymentTransactions] AS PT
    INNER JOIN
        -- 2. Link Payment to Booking
        [dbo].[Bookings] AS B ON PT.BookingId = B.BookingId
    INNER JOIN
        -- 3. Link Booking to Show
        [dbo].[Shows] AS S ON B.ShowId = S.ShowId
    INNER JOIN
        -- 4. Filter by the requested Movie (efficient filtering early)
        [dbo].[Movies] AS M ON S.MovieId = M.MovieId 
    INNER JOIN
        -- 5. Link Show to Screen
        [dbo].[Screens] AS SC ON S.ScreenId = SC.ScreenId
    INNER JOIN
        -- 6. Link Screen to Theatre (The physical location starts here)
        [dbo].[Theatres] AS T ON SC.TheatreId = T.TheatreId
    INNER JOIN
        -- 7. Link Theatre to City
        [dbo].[Cities] AS CI ON T.CityId = CI.CityId
    INNER JOIN
        -- 8. Link City to Country (The required grouping dimension)
        [dbo].[Countries] AS C ON CI.CountryId = C.CountryId
    WHERE
        -- Critical Filter: Only count successful payments
        PT.Status = @paymentstatus_success
        -- Critical Filter: Use the input parameter
        AND M.MovieId = @MovieId 
    GROUP BY
        C.Name,
        C.Code,
        PT.Currency -- Grouping by currency is vital for later conversion
    ORDER BY
        C.Name,
        PT.Currency;

END
GO