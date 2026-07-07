using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Visitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Visitors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VesselCalls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "VesselCalls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VesselCalls",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VehicleAccesses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "VehicleAccesses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VehicleAccesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ShiftBriefings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ShiftBriefings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShiftBriefings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SecurityDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "SecurityDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SecurityDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SecurityAudits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "SecurityAudits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SecurityAudits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RestrictedZones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "RestrictedZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RestrictedZones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "NonConformities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "NonConformities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "NonConformities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "KpiTableRows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "KpiTableRows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "KpiTableRows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "KpiCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "KpiCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "KpiCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Habilitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Habilitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Habilitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExternalContacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ExternalContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExternalContacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Exercises",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExerciseRexes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ExerciseRexes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExerciseRexes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Checkpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Checkpoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Checkpoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Cameras",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cameras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Agents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AccessPasses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "AccessPasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AccessPasses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_IsDeleted",
                table: "Visitors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VesselCalls_IsDeleted",
                table: "VesselCalls",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAccesses_IsDeleted",
                table: "VehicleAccesses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBriefings_IsDeleted",
                table: "ShiftBriefings",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDocuments_IsDeleted",
                table: "SecurityDocuments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAudits_IsDeleted",
                table: "SecurityAudits",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedZones_IsDeleted",
                table: "RestrictedZones",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_NonConformities_IsDeleted",
                table: "NonConformities",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_KpiTableRows_IsDeleted",
                table: "KpiTableRows",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_KpiCards_IsDeleted",
                table: "KpiCards",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IsDeleted",
                table: "Incidents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Habilitations_IsDeleted",
                table: "Habilitations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalContacts_IsDeleted",
                table: "ExternalContacts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_IsDeleted",
                table: "Exercises",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseRexes_IsDeleted",
                table: "ExerciseRexes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_IsDeleted",
                table: "Checkpoints",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_IsDeleted",
                table: "Cameras",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_IsDeleted",
                table: "Agents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AccessPasses_IsDeleted",
                table: "AccessPasses",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visitors_IsDeleted",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_VesselCalls_IsDeleted",
                table: "VesselCalls");

            migrationBuilder.DropIndex(
                name: "IX_VehicleAccesses_IsDeleted",
                table: "VehicleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_ShiftBriefings_IsDeleted",
                table: "ShiftBriefings");

            migrationBuilder.DropIndex(
                name: "IX_SecurityDocuments_IsDeleted",
                table: "SecurityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_SecurityAudits_IsDeleted",
                table: "SecurityAudits");

            migrationBuilder.DropIndex(
                name: "IX_RestrictedZones_IsDeleted",
                table: "RestrictedZones");

            migrationBuilder.DropIndex(
                name: "IX_NonConformities_IsDeleted",
                table: "NonConformities");

            migrationBuilder.DropIndex(
                name: "IX_KpiTableRows_IsDeleted",
                table: "KpiTableRows");

            migrationBuilder.DropIndex(
                name: "IX_KpiCards_IsDeleted",
                table: "KpiCards");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_IsDeleted",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Habilitations_IsDeleted",
                table: "Habilitations");

            migrationBuilder.DropIndex(
                name: "IX_ExternalContacts_IsDeleted",
                table: "ExternalContacts");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_IsDeleted",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseRexes_IsDeleted",
                table: "ExerciseRexes");

            migrationBuilder.DropIndex(
                name: "IX_Checkpoints_IsDeleted",
                table: "Checkpoints");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_IsDeleted",
                table: "Cameras");

            migrationBuilder.DropIndex(
                name: "IX_Agents_IsDeleted",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_AccessPasses_IsDeleted",
                table: "AccessPasses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VesselCalls");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "VesselCalls");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VesselCalls");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VehicleAccesses");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "VehicleAccesses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VehicleAccesses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ShiftBriefings");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ShiftBriefings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShiftBriefings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SecurityDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "SecurityDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SecurityDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SecurityAudits");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "SecurityAudits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SecurityAudits");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RestrictedZones");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "RestrictedZones");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RestrictedZones");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "NonConformities");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "NonConformities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "NonConformities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "KpiTableRows");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "KpiTableRows");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "KpiTableRows");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "KpiCards");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "KpiCards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "KpiCards");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Habilitations");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Habilitations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Habilitations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExternalContacts");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ExternalContacts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExternalContacts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExerciseRexes");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ExerciseRexes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExerciseRexes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Checkpoints");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Checkpoints");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Checkpoints");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AccessPasses");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "AccessPasses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AccessPasses");
        }
    }
}
