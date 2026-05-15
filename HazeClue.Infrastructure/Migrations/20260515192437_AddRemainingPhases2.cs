using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazeClue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingPhases2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EligibilityStatus",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                schema: "Security",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiry",
                schema: "Security",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                schema: "Security",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "Sessions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Intensity",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConsentRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConsentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthAssessments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssessmentDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EligibilityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlagsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthAssessments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleResults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CompletionTimeSeconds = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuzzleResults_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeviceId",
                table: "Sessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_UserId",
                table: "ConsentRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthAssessments_UserId",
                table: "HealthAssessments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleResults_SessionId",
                table: "PuzzleResults",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Devices_DeviceId",
                table: "Sessions",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Devices_DeviceId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "ConsentRecords");

            migrationBuilder.DropTable(
                name: "HealthAssessments");

            migrationBuilder.DropTable(
                name: "PuzzleResults");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_DeviceId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "EligibilityStatus",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnboardingCompleted",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpCode",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpiry",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "Sessions");
        }
    }
}
