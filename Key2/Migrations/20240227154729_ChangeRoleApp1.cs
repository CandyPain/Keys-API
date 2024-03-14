using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key2.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleApp1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeanId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeanWorker",
                table: "User",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeanId",
                table: "appChangeRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "appChangeRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeanId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDeanWorker",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeanId",
                table: "appChangeRoles");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "appChangeRoles");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
