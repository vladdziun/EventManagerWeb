using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagerWeb.Data.Migrations
{
    public partial class AddEvenetPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventPhoto",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPhoto",
                table: "Events");
        }
    }
}
