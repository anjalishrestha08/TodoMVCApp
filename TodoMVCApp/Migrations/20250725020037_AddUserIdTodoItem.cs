using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoMVCApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Todos");
        }
    }
}
