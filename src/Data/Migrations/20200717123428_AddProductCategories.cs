using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations
{
    public partial class AddProductCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            // After we add the ProductCategoryId field into products,
            // we set all products to belong to the category 1
            // no need to update the Down method, as on rollback, the entire column will be deleted
            // Note! if there are side effects on your added code, that would not automatically be
            // rolledback on the Down function, the best practice is to add your rollback script there
            // to maintain data integrity and consistency. 
            // When the Down method is called, your database should remain in the exact same state that it 
            // had before the migration.
            migrationBuilder.Sql("UPDATE \"Products\" SET \"ProductCategoryId\"=1");

            migrationBuilder.CreateTable(
                name: "ProductCategories",
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
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            // After we created the ProductCategories table, we add some seed data into it
            // No need to delete it on Down method, as the entire table will be dropped there
            // After the data is added, we are able to let ef run the rest of the migration,
            // which actually creates the foreign key constraints and indices 
            List<string> productCategories = new List<string>() { "Beer", "SoftDrinks", "Sodas", "Others" };
            foreach (string categ in productCategories)
            {
                migrationBuilder.Sql($"INSERT INTO \"ProductCategories\" (\"Name\", \"DateCreated\", \"DateModified\", \"UniqueKey\") VALUES('{categ}', current_timestamp, current_timestamp, '{Guid.NewGuid()}')");
            }

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "Products");
        }
    }
}
