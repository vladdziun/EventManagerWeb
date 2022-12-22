using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagerWeb.Data.Migrations
{
    public partial class AddEvenetAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Events");
        }
    }
}
