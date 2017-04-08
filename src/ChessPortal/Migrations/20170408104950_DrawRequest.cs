using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChessPortal.Migrations
{
    public partial class DrawRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrawRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChallengeId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawRequests_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrawRequests_AspNetUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrawRequests_ChallengeId",
                table: "DrawRequests",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawRequests_PlayerId",
                table: "DrawRequests",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawRequests");
        }
    }
}
