using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectPlanner.API.Migrations
{
    public partial class TodoMessageModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    TodoId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoMessages_Todos_TodoId",
                        column: x => x.TodoId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoMessages_TodoId",
                table: "TodoMessages",
                column: "TodoId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoMessages_UserId",
                table: "TodoMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoMessages");
        }
    }
}
