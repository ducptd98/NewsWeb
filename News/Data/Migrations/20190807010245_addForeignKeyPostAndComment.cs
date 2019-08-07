using Microsoft.EntityFrameworkCore.Migrations;

namespace ContructorNews.Data.Migrations
{
    public partial class addForeignKeyPostAndComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PostId",
                table: "ParentComments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ParentComments_PostId",
                table: "ParentComments",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentComments_Posts_PostId",
                table: "ParentComments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentComments_Posts_PostId",
                table: "ParentComments");

            migrationBuilder.DropIndex(
                name: "IX_ParentComments_PostId",
                table: "ParentComments");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "ParentComments");
        }
    }
}
