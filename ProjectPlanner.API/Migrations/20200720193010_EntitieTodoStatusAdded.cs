using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectPlanner.API.Migrations
{
    public partial class EntitieTodoStatusAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Todos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Todos");
        }
    }
}
