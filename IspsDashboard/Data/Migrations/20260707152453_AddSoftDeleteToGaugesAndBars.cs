using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IspsDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToGaugesAndBars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProgressBars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ProgressBars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProgressBars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Gauges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Gauges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Gauges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressBars_IsDeleted",
                table: "ProgressBars",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Gauges_IsDeleted",
                table: "Gauges",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProgressBars_IsDeleted",
                table: "ProgressBars");

            migrationBuilder.DropIndex(
                name: "IX_Gauges_IsDeleted",
                table: "Gauges");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProgressBars");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ProgressBars");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProgressBars");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Gauges");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Gauges");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Gauges");
        }
    }
}
