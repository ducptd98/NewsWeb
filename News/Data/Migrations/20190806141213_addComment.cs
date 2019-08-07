using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContructorNews.Data.Migrations
{
    public partial class addComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentComments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChildComments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ParentCommentId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildComments_ParentComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "ParentComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildComments_ParentCommentId",
                table: "ChildComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildComments_UserId",
                table: "ChildComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentComments_UserId",
                table: "ParentComments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildComments");

            migrationBuilder.DropTable(
                name: "ParentComments");
        }
    }
}
