using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key2.Migrations
{
    /// <inheritdoc />
    public partial class KeyBuilding1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Building",
                table: "keys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Building",
                table: "keys");
        }
    }
}
