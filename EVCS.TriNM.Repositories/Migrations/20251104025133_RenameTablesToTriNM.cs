using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToTriNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Station",
                table: "Station");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charger",
                table: "Charger");

            migrationBuilder.RenameTable(
                name: "Station",
                newName: "StationTriNM");

            migrationBuilder.RenameTable(
                name: "Charger",
                newName: "ChargerTriNM");

            migrationBuilder.RenameIndex(
                name: "IX_Charger_StationID",
                table: "ChargerTriNM",
                newName: "IX_ChargerTriNM_StationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM",
                column: "StationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM",
                column: "ChargerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM");

            migrationBuilder.RenameTable(
                name: "StationTriNM",
                newName: "Station");

            migrationBuilder.RenameTable(
                name: "ChargerTriNM",
                newName: "Charger");

            migrationBuilder.RenameIndex(
                name: "IX_ChargerTriNM_StationID",
                table: "Charger",
                newName: "IX_Charger_StationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Station",
                table: "Station",
                column: "StationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charger",
                table: "Charger",
                column: "ChargerID");
        }
    }
}
