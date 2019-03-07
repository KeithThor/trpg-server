using Microsoft.EntityFrameworkCore.Migrations;

namespace TRPGGame.Migrations
{
    public partial class WorldEntityNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PlayerEntities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "PlayerEntities");
        }
    }
}
