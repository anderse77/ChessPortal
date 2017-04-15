using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChessPortal.Migrations
{
    public partial class ChessProblem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfProblemsFailed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfProblemsSolved",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChessProblems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChessProblemId = table.Column<string>(nullable: true),
                    PlayerId = table.Column<string>(nullable: true),
                    moveOffsetNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessProblems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChessProblems_AspNetUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChessProblems_PlayerId",
                table: "ChessProblems",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChessProblems");

            migrationBuilder.DropColumn(
                name: "NumberOfProblemsFailed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfProblemsSolved",
                table: "AspNetUsers");
        }
    }
}
