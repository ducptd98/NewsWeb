using Microsoft.EntityFrameworkCore.Migrations;

namespace ContructorNews.Data.Migrations
{
    public partial class updatePost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Posts");
        }
    }
}
