using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations
{
    public partial class AddProductCategoryTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryTypeId",
                table: "ProductCategories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"ProductCategories\" SET \"ProductCategoryTypeId\"=1");

            migrationBuilder.CreateTable(
                name: "ProductCategoryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    UniqueKey = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryTypes", x => x.Id);
                });

            List<string> productCategoryTypes = new List<string>() { "Beverages", "Foods", "Electronics" };
            foreach (string type in productCategoryTypes)
            {
                migrationBuilder.Sql($"INSERT INTO \"ProductCategoryTypes\" (\"Name\", \"DateCreated\", \"DateModified\", \"UniqueKey\") VALUES('{type}', current_timestamp, current_timestamp, '{Guid.NewGuid()}')");
            }

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductCategoryTypeId",
                table: "ProductCategories",
                column: "ProductCategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ProductCategoryTypes_ProductCategoryTypeId",
                table: "ProductCategories",
                column: "ProductCategoryTypeId",
                principalTable: "ProductCategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ProductCategoryTypes_ProductCategoryTypeId",
                table: "ProductCategories");

            migrationBuilder.DropTable(
                name: "ProductCategoryTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ProductCategoryTypeId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ProductCategoryTypeId",
                table: "ProductCategories");
        }
    }
}
