using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleDocumentsAudits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Auditor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Conclusion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    IsConfidential = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DriverIdNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContainerNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SealNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SealVerified = table.Column<bool>(type: "bit", nullable: false),
                    BookingReference = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Searched = table.Column<bool>(type: "bit", nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Gate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditFindings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecurityAuditId = table.Column<int>(type: "int", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    CheckItem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditFindings_SecurityAudits_SecurityAuditId",
                        column: x => x.SecurityAuditId,
                        principalTable: "SecurityAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditFindings_SecurityAuditId",
                table: "AuditFindings",
                column: "SecurityAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAudits_Reference",
                table: "SecurityAudits",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAudits_Status",
                table: "SecurityAudits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDocuments_Category",
                table: "SecurityDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDocuments_Status",
                table: "SecurityDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAccesses_OccurredAt",
                table: "VehicleAccesses",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAccesses_Plate",
                table: "VehicleAccesses",
                column: "Plate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAccesses_Reference",
                table: "VehicleAccesses",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditFindings");

            migrationBuilder.DropTable(
                name: "SecurityDocuments");

            migrationBuilder.DropTable(
                name: "VehicleAccesses");

            migrationBuilder.DropTable(
                name: "SecurityAudits");
        }
    }
}
