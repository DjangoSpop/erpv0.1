using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace erpv0._1.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductNameToProductTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "production",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "QRCode",
                schema: "production",
                table: "products",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "production",
                table: "products",
                newName: "UpdatedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "production",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                schema: "production",
                table: "products",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "production",
                table: "products",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "production",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ImageData",
                schema: "production",
                table: "products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "production",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                schema: "production",
                table: "products",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "production",
                table: "products",
                newName: "QRCode");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "production",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
