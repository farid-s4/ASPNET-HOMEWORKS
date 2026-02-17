using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_16._TaskFlow_Ownership.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityToTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TaskItems");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
