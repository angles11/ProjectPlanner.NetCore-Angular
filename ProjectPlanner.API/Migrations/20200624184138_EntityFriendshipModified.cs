using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectPlanner.API.Migrations
{
    public partial class EntityFriendshipModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionUserId",
                table: "Friendships",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ActionUserId",
                table: "Friendships",
                column: "ActionUserId");

         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ActionUserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ActionUserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "ActionUserId",
                table: "Friendships");
        }
    }
}
