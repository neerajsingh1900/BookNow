using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.DataAccess.Migrations
{
    /// <inheritdoc />
    // This migration registers the keyless DTO (RawRevenueDto) mapping configured in ApplicationDbContext.
    public partial class RegisterRawRevenueDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🌟 CRITICAL FIX: Leave Up() empty. 🌟
            // The Keyless Entity configuration (modelBuilder.Entity<RawRevenueDto>().HasNoKey()) 
            // does NOT result in database changes, so the migration should be empty.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🌟 CRITICAL FIX: Leave Down() empty. 🌟
        }
    }
}