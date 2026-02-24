using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceManager.Migrations
{
    /// <inheritdoc />
    public partial class FixedInvoiceRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "InvoiceRows",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "InvoiceRows");
        }
    }
}
