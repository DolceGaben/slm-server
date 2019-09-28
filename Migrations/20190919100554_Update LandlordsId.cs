using Microsoft.EntityFrameworkCore.Migrations;

namespace SLM.Migrations
{
    public partial class UpdateLandlordsId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LandlordsId",
                table: "Houses",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LandlordsId",
                table: "Houses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
