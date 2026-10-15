using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s_tok.Models;

/// <summary>
/// Produto cadastrado no catálogo do S-TOK.
/// Categoria e fornecedor são textos até os módulos próprios serem implementados.
/// </summary>
public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    [Display(Name = "Nome do produto")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o código do produto.")]
    [StringLength(40, ErrorMessage = "O código deve ter no máximo 40 caracteres.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "A categoria deve ter no máximo 80 caracteres.")]
    [Display(Name = "Categoria")]
    public string? Categoria { get; set; }

    [StringLength(150, ErrorMessage = "O fornecedor deve ter no máximo 150 caracteres.")]
    [Display(Name = "Fornecedor")]
    public string? Fornecedor { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Informe um preço de custo válido.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Preço de custo")]
    public decimal PrecoCusto { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Informe um preço de venda válido.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Preço de venda")]
    public decimal PrecoVenda { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    [Display(Name = "Quantidade em estoque")]
    public int QuantidadeEstoque { get; set; }
}
