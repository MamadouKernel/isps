using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictedZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestrictedZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProtectionMeasures = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AuthorizedPersonnel = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ZoneManager = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RequiresEscort = table.Column<bool>(type: "bit", nullable: false),
                    RequiresClearance = table.Column<bool>(type: "bit", nullable: false),
                    CctvMonitored = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedZones_AccessLevel",
                table: "RestrictedZones",
                column: "AccessLevel");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedZones_Code",
                table: "RestrictedZones",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestrictedZones");
        }
    }
}
