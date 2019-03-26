using Microsoft.EntityFrameworkCore.Migrations;

namespace BitcoinTransfer.Migrations
{
    public partial class fixedAmountDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "Wallets");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "WalletId");

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "WalletId", "Address", "ConfirmedBalance", "PrivateKey" },
                values: new object[] { 1L, "miudc6pyYwdVTxuDYCNdsJDESxUZz3wGKq", 0.0, "cPK1Gssv5UD79shik4yoCqpXUuYXoMArwEGjPPfzXHVnzZCRZErm" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 1L);

            migrationBuilder.RenameTable(
                name: "Wallets",
                newName: "Blogs");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "WalletId");
        }
    }
}
