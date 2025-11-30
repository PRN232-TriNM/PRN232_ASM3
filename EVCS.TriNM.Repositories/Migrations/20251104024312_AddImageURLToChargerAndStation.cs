using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddImageURLToChargerAndStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Station",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Charger",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Charger");
        }
    }
}
