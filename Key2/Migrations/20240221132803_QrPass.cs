using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key2.Migrations
{
    /// <inheritdoc />
    public partial class QrPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeanId",
                table: "keys",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeat",
                table: "apps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "administrators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "administrators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QRPass",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QRPas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    keyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRPass", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRPass");

            migrationBuilder.DropColumn(
                name: "DeanId",
                table: "keys");

            migrationBuilder.DropColumn(
                name: "IsRepeat",
                table: "apps");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "administrators");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "administrators");
        }
    }
}
