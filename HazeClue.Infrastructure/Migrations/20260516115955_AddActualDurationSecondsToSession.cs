using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazeClue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActualDurationSecondsToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualDurationSeconds",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDurationSeconds",
                table: "Sessions");
        }
    }
}
