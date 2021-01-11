using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations
{
    public partial class AddProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    UniqueKey = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
            List<string[]> products = new List<string[]>();
            products.Add(new string[] { "Ursus beer", "100" });
            products.Add(new string[] { "Tuborg beer", "100" });
            products.Add(new string[] { "Stella Artois beer", "100" });
            products.Add(new string[] { "Staropramen beer", "100" });
            foreach (string[] beer in products)
            {
                migrationBuilder.Sql($"INSERT INTO \"Products\" (\"Name\", \"Price\", \"DateCreated\", \"DateModified\", \"UniqueKey\") VALUES('{beer[0]}', {beer[1]}, current_timestamp, current_timestamp, '{Guid.NewGuid()}')");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
