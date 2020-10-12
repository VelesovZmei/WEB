using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WEB.Migrations
{
    public partial class Rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Afinns",
                columns: table => new
                {
                    Word = table.Column<string>(maxLength: 64, nullable: false),
                    Culture = table.Column<string>(fixedLength: true, maxLength: 5, nullable: false),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afinns", x => new { x.Word, x.Culture });
                });

           

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<string>(fixedLength: true, maxLength: 36, nullable: false),
                    Head = table.Column<string>(maxLength: 4000, nullable: false),
                    Text = table.Column<string>(nullable: false),
                    SourceURL = table.Column<string>(maxLength: 1024, nullable: true),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    DatePosted = table.Column<long>(nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });


            migrationBuilder.CreateIndex(
                name: "IX_News_DatePosted",
                table: "News",
                column: "DatePosted");


            migrationBuilder.CreateIndex(
                name: "IX_News_Score",
                table: "News",
                column: "Score");

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Afinns");

          
            migrationBuilder.DropTable(
                name: "News");

           
        }
    }
}
