using Microsoft.EntityFrameworkCore.Migrations;

namespace TRPGGame.Migrations
{
    public partial class ChangeIconUris : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconUris",
                table: "PlayerEntities",
                newName: "IconUris_RightHandIconUri");

            migrationBuilder.AddColumn<string>(
                name: "IconUris_BaseIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_BodyIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_CloakIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_ExtraIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_GlovesIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_HairIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_HeadIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_LeftHandIconUri",
                table: "PlayerEntities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUris_LegsIconUri",
                table: "PlayerEntities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUris_BaseIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_BodyIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_CloakIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_ExtraIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_GlovesIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_HairIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_HeadIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_LeftHandIconUri",
                table: "PlayerEntities");

            migrationBuilder.DropColumn(
                name: "IconUris_LegsIconUri",
                table: "PlayerEntities");

            migrationBuilder.RenameColumn(
                name: "IconUris_RightHandIconUri",
                table: "PlayerEntities",
                newName: "IconUris");
        }
    }
}
