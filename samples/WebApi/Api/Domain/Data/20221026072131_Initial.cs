using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Api.Domain.Data
{
  public partial class Initial : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Holiday",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Type = table.Column<string>(type: "TEXT", nullable: false),
            State = table.Column<string>(type: "TEXT", nullable: false),
            Assignee = table.Column<string>(type: "TEXT", nullable: false),
            Requester = table.Column<string>(type: "TEXT", nullable: false),
            Superior = table.Column<string>(type: "TEXT", nullable: true),
            From = table.Column<DateTime>(type: "TEXT", nullable: true),
            To = table.Column<DateTime>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Holiday", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Issue",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Type = table.Column<string>(type: "TEXT", nullable: false),
            State = table.Column<string>(type: "TEXT", nullable: false),
            Creator = table.Column<string>(type: "TEXT", nullable: false),
            Assignee = table.Column<string>(type: "TEXT", nullable: false),
            Title = table.Column<string>(type: "TEXT", nullable: false),
            Description = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Issue", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Stepper",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Type = table.Column<string>(type: "TEXT", nullable: false),
            State = table.Column<string>(type: "TEXT", nullable: false),
            Assignee = table.Column<string>(type: "TEXT", nullable: false),
            Creator = table.Column<string>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Stepper", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Workflow",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Type = table.Column<string>(type: "TEXT", nullable: false),
            State = table.Column<string>(type: "TEXT", nullable: false),
            CorrelationId = table.Column<int>(type: "INTEGER", nullable: false),
            Assignee = table.Column<string>(type: "TEXT", nullable: true),
            Started = table.Column<DateTime>(type: "TEXT", nullable: false),
            Completed = table.Column<DateTime>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Workflow", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "WorkItem",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            TriggerName = table.Column<string>(type: "TEXT", nullable: false),
            EntityId = table.Column<int>(type: "INTEGER", nullable: false),
            WorkflowType = table.Column<string>(type: "TEXT", nullable: false),
            Retries = table.Column<int>(type: "INTEGER", nullable: false),
            Error = table.Column<string>(type: "TEXT", nullable: true),
            DueDate = table.Column<DateTime>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_WorkItem", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "HolidayMessage",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Author = table.Column<string>(type: "TEXT", nullable: false),
            Message = table.Column<string>(type: "TEXT", nullable: false),
            HolidayId = table.Column<int>(type: "INTEGER", nullable: false)
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

      migrationBuilder.CreateTable(
          name: "WorkflowHistory",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Created = table.Column<DateTime>(type: "TEXT", nullable: false),
            FromState = table.Column<string>(type: "TEXT", nullable: false),
            ToState = table.Column<string>(type: "TEXT", nullable: false),
            UserName = table.Column<string>(type: "TEXT", nullable: true),
            WorkflowId = table.Column<int>(type: "INTEGER", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_WorkflowHistory", x => x.Id);
            table.ForeignKey(
                      name: "FK_WorkflowHistory_Workflow_WorkflowId",
                      column: x => x.WorkflowId,
                      principalTable: "Workflow",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "WorkflowVariable",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Type = table.Column<string>(type: "TEXT", nullable: false),
            Content = table.Column<string>(type: "TEXT", nullable: false),
            WorkflowId = table.Column<int>(type: "INTEGER", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_WorkflowVariable", x => x.Id);
            table.ForeignKey(
                      name: "FK_WorkflowVariable_Workflow_WorkflowId",
                      column: x => x.WorkflowId,
                      principalTable: "Workflow",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_HolidayMessage_HolidayId",
          table: "HolidayMessage",
          column: "HolidayId");

      migrationBuilder.CreateIndex(
          name: "IX_WorkflowHistory_WorkflowId",
          table: "WorkflowHistory",
          column: "WorkflowId");

      migrationBuilder.CreateIndex(
          name: "IX_WorkflowVariable_WorkflowId",
          table: "WorkflowVariable",
          column: "WorkflowId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "HolidayMessage");

      migrationBuilder.DropTable(
          name: "Issue");

      migrationBuilder.DropTable(
          name: "Stepper");

      migrationBuilder.DropTable(
          name: "WorkflowHistory");

      migrationBuilder.DropTable(
          name: "WorkflowVariable");

      migrationBuilder.DropTable(
          name: "WorkItem");

      migrationBuilder.DropTable(
          name: "Holiday");

      migrationBuilder.DropTable(
          name: "Workflow");
    }
  }
}