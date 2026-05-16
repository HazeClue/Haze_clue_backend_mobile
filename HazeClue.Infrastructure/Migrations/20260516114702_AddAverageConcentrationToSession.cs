using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazeClue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAverageConcentrationToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageConcentration",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageConcentration",
                table: "Sessions");
        }
    }
}
