using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class Discount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountEnd",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountStart",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "Prosucts_IsDiscountActive_DiscountStart_DiscountEnd",
                table: "Products",
                columns: new[] { "IsDiscountActive", "DiscountStart", "DiscountEnd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Prosucts_IsDiscountActive_DiscountStart_DiscountEnd",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountEnd",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountStart",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDiscountActive",
                table: "Products");
        }
    }
}
