using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeSoftDeleteFksOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_ScreenId_SeatNumber",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances");

            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TicketUrl",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "ScreenId",
                table: "Seats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "SeatInstances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ScreenId_SeatNumber",
                table: "Seats",
                columns: new[] { "ScreenId", "SeatNumber" },
                unique: true,
                filter: "[ScreenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances",
                columns: new[] { "ShowId", "SeatId" },
                unique: true,
                filter: "[ShowId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "ScreenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_ScreenId_SeatNumber",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances");

            migrationBuilder.AlterColumn<int>(
                name: "ScreenId",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "SeatInstances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketUrl",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ScreenId_SeatNumber",
                table: "Seats",
                columns: new[] { "ScreenId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatInstances_ShowId_SeatId",
                table: "SeatInstances",
                columns: new[] { "ShowId", "SeatId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "ScreenId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
