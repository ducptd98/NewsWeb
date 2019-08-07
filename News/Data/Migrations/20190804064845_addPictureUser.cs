using Microsoft.EntityFrameworkCore.Migrations;

namespace ContructorNews.Data.Migrations
{
    public partial class addPictureUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "AspNetUsers");
        }
    }
}
