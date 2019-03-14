using Microsoft.EntityFrameworkCore.Migrations;

namespace TRPGGame.Migrations
{
    public partial class WorldEntityIconList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUri",
                table: "PlayerEntities");

            migrationBuilder.AddColumn<string>(
                name: "IconUris",
                table: "PlayerEntities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUris",
                table: "PlayerEntities");

            migrationBuilder.AddColumn<string>(
                name: "IconUri",
                table: "PlayerEntities",
                nullable: true);
        }
    }
}
