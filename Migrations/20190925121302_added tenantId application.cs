using Microsoft.EntityFrameworkCore.Migrations;

namespace SLM.Migrations
{
    public partial class addedtenantIdapplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LandlordId",
                table: "Applications",
                newName: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Applications",
                newName: "LandlordId");
        }
    }
}
