using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVesselsCamerasContactsBriefingsRex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCheckedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseRexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PositivePoints = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImprovementPoints = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FollowUp = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    WrittenBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WrittenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseRexes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseRexes_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmergencyPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RadioChannel = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsEmergency24x7 = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBriefings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    OutgoingAgent = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IncomingAgent = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CurrentMarsecLevel = table.Column<int>(type: "int", nullable: false),
                    EventsSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AttentionPoints = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StandingOrders = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AcknowledgedByIncoming = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBriefings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VesselCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VesselName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ImoNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CallSign = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ShipIspsLevel = table.Column<int>(type: "int", nullable: false),
                    Eta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Etd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Berth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SecurityNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LastTenPorts = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CrewCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselCalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CameraMaintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResultingStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraMaintenances_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalContactId = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HandledBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactInteractions_ExternalContacts_ExternalContactId",
                        column: x => x.ExternalContactId,
                        principalTable: "ExternalContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclarationsOfSecurity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VesselCallId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PortLevel = table.Column<int>(type: "int", nullable: false),
                    ShipLevel = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedByPfso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SignedByShip = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreedMeasures = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclarationsOfSecurity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclarationsOfSecurity_VesselCalls_VesselCallId",
                        column: x => x.VesselCallId,
                        principalTable: "VesselCalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CameraMaintenances_CameraId",
                table: "CameraMaintenances",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_Code",
                table: "Cameras",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_Status",
                table: "Cameras",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInteractions_ExternalContactId",
                table: "ContactInteractions",
                column: "ExternalContactId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationsOfSecurity_Reference",
                table: "DeclarationsOfSecurity",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationsOfSecurity_VesselCallId",
                table: "DeclarationsOfSecurity",
                column: "VesselCallId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseRexes_ExerciseId",
                table: "ExerciseRexes",
                column: "ExerciseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalContacts_Type",
                table: "ExternalContacts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBriefings_ShiftDate_Slot",
                table: "ShiftBriefings",
                columns: new[] { "ShiftDate", "Slot" });

            migrationBuilder.CreateIndex(
                name: "IX_VesselCalls_Eta",
                table: "VesselCalls",
                column: "Eta");

            migrationBuilder.CreateIndex(
                name: "IX_VesselCalls_Reference",
                table: "VesselCalls",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VesselCalls_Status",
                table: "VesselCalls",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CameraMaintenances");

            migrationBuilder.DropTable(
                name: "ContactInteractions");

            migrationBuilder.DropTable(
                name: "DeclarationsOfSecurity");

            migrationBuilder.DropTable(
                name: "ExerciseRexes");

            migrationBuilder.DropTable(
                name: "ShiftBriefings");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "ExternalContacts");

            migrationBuilder.DropTable(
                name: "VesselCalls");
        }
    }
}
