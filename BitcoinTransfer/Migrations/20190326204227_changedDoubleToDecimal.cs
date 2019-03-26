using Microsoft.EntityFrameworkCore.Migrations;

namespace BitcoinTransfer.Migrations
{
    public partial class changedDoubleToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ConfirmedBalance",
                table: "Wallets",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 1L,
                column: "ConfirmedBalance",
                value: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ConfirmedBalance",
                table: "Wallets",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 1L,
                column: "ConfirmedBalance",
                value: 0.0);
        }
    }
}
