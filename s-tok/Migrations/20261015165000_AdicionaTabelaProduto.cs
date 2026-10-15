using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using s_tok.Models;

#nullable disable

namespace s_tok.Migrations;

[DbContext(typeof(S_tokDbContext))]
[Migration("20261015165000_AdicionaTabelaProduto")]
public partial class AdicionaTabelaProduto : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Produtos",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Nome = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Codigo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                Categoria = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                Fornecedor = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                PrecoCusto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PrecoVenda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                QuantidadeEstoque = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Produtos", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Produtos_Codigo",
            table: "Produtos",
            column: "Codigo",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Produtos");
    }
}
