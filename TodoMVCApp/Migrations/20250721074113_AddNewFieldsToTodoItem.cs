using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoMVCApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Todos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "InProcess",
                table: "Todos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "InProcess",
                table: "Todos");
        }
    }
}
