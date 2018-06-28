using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    Assignee = table.Column<string>(nullable: false),
                    Requester = table.Column<string>(nullable: false),
                    Superior = table.Column<string>(nullable: true),
                    From = table.Column<DateTime>(nullable: true),
                    To = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    CorrelationId = table.Column<int>(nullable: false),
                    Assignee = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Author = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    HolidayId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayMessage_Holiday_HolidayId",
                        column: x => x.HolidayId,
                        principalTable: "Holiday",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayMessage_HolidayId",
                table: "HolidayMessage",
                column: "HolidayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidayMessage");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Holiday");
        }
    }
}
