using Microsoft.EntityFrameworkCore.Migrations;

namespace TRPGGame.Migrations
{
    public partial class AddBootsIconUri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconUris_BootsIconUri",
                table: "PlayerEntities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUris_BootsIconUri",
                table: "PlayerEntities");
        }
    }
}
