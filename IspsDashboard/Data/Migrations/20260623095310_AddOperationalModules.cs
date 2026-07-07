using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadgeNumber",
                table: "Agents",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Agents",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HiredAt",
                table: "Agents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Agents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Agents",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Agents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Agents",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Agents",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TargetIntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habilitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ObtainedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habilitations_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarsecLevelChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromLevel = table.Column<int>(type: "int", nullable: false),
                    ToLevel = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DecisionSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DecidedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarsecLevelChanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdentifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosureEvidence = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdDocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VehiclePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScheduledArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Host = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EscortedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BadgeIssued = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckInAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedInBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CheckedOutBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatrolScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    AgentLabel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    AnomalyType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatrolScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatrolScans_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PatrolScans_Checkpoints_CheckpointId",
                        column: x => x.CheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarsecChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarsecLevelChangeId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarsecChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarsecChecklistItems_MarsecLevelChanges_MarsecLevelChangeId",
                        column: x => x.MarsecLevelChangeId,
                        principalTable: "MarsecLevelChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_Code",
                table: "Checkpoints",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habilitations_AgentId",
                table: "Habilitations",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Habilitations_ExpiresAt",
                table: "Habilitations",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarsecChecklistItems_MarsecLevelChangeId",
                table: "MarsecChecklistItems",
                column: "MarsecLevelChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_NonConformities_DueDate",
                table: "NonConformities",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_NonConformities_Reference",
                table: "NonConformities",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonConformities_Status",
                table: "NonConformities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PatrolScans_AgentId",
                table: "PatrolScans",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_PatrolScans_CheckpointId_ScannedAt",
                table: "PatrolScans",
                columns: new[] { "CheckpointId", "ScannedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_Reference",
                table: "Visitors",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_ScheduledArrival",
                table: "Visitors",
                column: "ScheduledArrival");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_Status",
                table: "Visitors",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habilitations");

            migrationBuilder.DropTable(
                name: "MarsecChecklistItems");

            migrationBuilder.DropTable(
                name: "NonConformities");

            migrationBuilder.DropTable(
                name: "PatrolScans");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.DropTable(
                name: "MarsecLevelChanges");

            migrationBuilder.DropTable(
                name: "Checkpoints");

            migrationBuilder.DropColumn(
                name: "BadgeNumber",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "HiredAt",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Agents");
        }
    }
}
