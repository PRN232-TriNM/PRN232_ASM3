using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RenamePrimaryKeysToTriNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Charger_Station",
                table: "ChargerTriNM");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Charger",
                table: "ChargingTransaction");

            // Drop primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM");

            // Rename tables
            migrationBuilder.RenameTable(
                name: "StationTriNM",
                newName: "Station");

            migrationBuilder.RenameTable(
                name: "ChargerTriNM",
                newName: "Charger");

            // Rename columns
            migrationBuilder.RenameColumn(
                name: "StationID",
                table: "Station",
                newName: "StationTriNMId");

            migrationBuilder.RenameColumn(
                name: "ChargerID",
                table: "Charger",
                newName: "ChargerTriNMId");

            migrationBuilder.RenameColumn(
                name: "StationID",
                table: "Charger",
                newName: "StationTriNMId");

            migrationBuilder.RenameColumn(
                name: "ChargerID",
                table: "ChargingTransaction",
                newName: "ChargerTriNMId");

            // Drop and recreate indexes
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChargerTriNM_StationID' AND object_id = OBJECT_ID('Charger'))
                    DROP INDEX [IX_ChargerTriNM_StationID] ON [Charger];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Charger_StationTriNMId",
                table: "Charger",
                column: "StationTriNMId");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChargingTransaction_ChargerID' AND object_id = OBJECT_ID('ChargingTransaction'))
                    DROP INDEX [IX_ChargingTransaction_ChargerID] ON [ChargingTransaction];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingTransaction_ChargerTriNMId",
                table: "ChargingTransaction",
                column: "ChargerTriNMId");

            // Add primary keys back
            migrationBuilder.AddPrimaryKey(
                name: "PK_Station",
                table: "Station",
                column: "StationTriNMId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charger",
                table: "Charger",
                column: "ChargerTriNMId");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Charger_Station",
                table: "Charger",
                column: "StationTriNMId",
                principalTable: "Station",
                principalColumn: "StationTriNMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Charger",
                table: "ChargingTransaction",
                column: "ChargerTriNMId",
                principalTable: "Charger",
                principalColumn: "ChargerTriNMId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Charger_Station",
                table: "Charger");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Charger",
                table: "ChargingTransaction");

            // Drop primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_Station",
                table: "Station");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charger",
                table: "Charger");

            // Rename columns back
            migrationBuilder.RenameColumn(
                name: "StationTriNMId",
                table: "Station",
                newName: "StationID");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMId",
                table: "Charger",
                newName: "ChargerID");

            migrationBuilder.RenameColumn(
                name: "StationTriNMId",
                table: "Charger",
                newName: "StationID");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMId",
                table: "ChargingTransaction",
                newName: "ChargerID");

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_Charger_StationTriNMId",
                table: "Charger",
                newName: "IX_ChargerTriNM_StationID");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingTransaction_ChargerTriNMId",
                table: "ChargingTransaction",
                newName: "IX_ChargingTransaction_ChargerID");

            // Rename tables back
            migrationBuilder.RenameTable(
                name: "Station",
                newName: "StationTriNM");

            migrationBuilder.RenameTable(
                name: "Charger",
                newName: "ChargerTriNM");

            // Add primary keys back
            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM",
                column: "StationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM",
                column: "ChargerID");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Charger_Station",
                table: "ChargerTriNM",
                column: "StationID",
                principalTable: "StationTriNM",
                principalColumn: "StationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Charger",
                table: "ChargingTransaction",
                column: "ChargerID",
                principalTable: "ChargerTriNM",
                principalColumn: "ChargerID");
        }
    }
}
