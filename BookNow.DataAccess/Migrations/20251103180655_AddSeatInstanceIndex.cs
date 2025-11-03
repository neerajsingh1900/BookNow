using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatInstanceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatInstances_ShowId",
                table: "SeatInstances");

            migrationBuilder.CreateIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances",
                columns: new[] { "ShowId", "SeatId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances");

            migrationBuilder.CreateIndex(
                name: "IX_SeatInstances_ShowId",
                table: "SeatInstances",
                column: "ShowId");
        }
    }
}
