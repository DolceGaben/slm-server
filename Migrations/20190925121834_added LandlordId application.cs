using Microsoft.EntityFrameworkCore.Migrations;

namespace SLM.Migrations
{
    public partial class addedLandlordIdapplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LandlordId",
                table: "Applications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandlordId",
                table: "Applications");
        }
    }
}
