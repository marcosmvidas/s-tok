using System.Globalization;
using System.Reflection;

namespace s_tok.Pages.Shared;

/// <summary>
/// Configuração de uma grid simples. Cada módulo fornece seus próprios itens,
/// colunas e rotas, mantendo sua lógica de consulta independente.
/// </summary>
public sealed class GridListagemModel
{
    public string Titulo { get; set; } = "Registros";
    public string? Descricao { get; set; }
    public string MensagemVazia { get; set; } = "Nenhum registro encontrado.";
    public string? PaginaNovo { get; set; }
    public string TextoBotaoNovo { get; set; } = "Novo cadastro";

    public string PropriedadeId { get; set; } = "Id";
    public string PropriedadeNome { get; set; } = "Nome";

    public string? PaginaDetalhes { get; set; }
    public string? PaginaAlterar { get; set; }
    public string? PaginaExcluir { get; set; }

    public IReadOnlyList<GridColunaModel> Colunas { get; set; } = [];
    public IEnumerable<object> Itens { get; set; } = [];

    public string ObterId(object item) =>
        ObterPropriedade(item, PropriedadeId)?.ToString() ?? string.Empty;

    public string ObterNome(object item) =>
        ObterPropriedade(item, PropriedadeNome)?.ToString() ?? string.Empty;

    public string ObterValorFormatado(object item, GridColunaModel coluna)
    {
        var valor = ObterPropriedade(item, coluna.Propriedade);

        if (valor is null)
            return "—";

        if (valor is bool booleano)
            return booleano ? "Sim" : "Não";

        if (valor is DateTime data)
            return data.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));

        if (valor is DateTimeOffset dataComFuso)
            return dataComFuso.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));

        if (valor is decimal decimalValue && string.Equals(coluna.Formato, "moeda", StringComparison.OrdinalIgnoreCase))
            return decimalValue.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        return Convert.ToString(valor, CultureInfo.GetCultureInfo("pt-BR")) ?? "—";
    }

    private static object? ObterPropriedade(object item, string nomePropriedade)
    {
        if (string.IsNullOrWhiteSpace(nomePropriedade))
            return null;

        PropertyInfo? propriedade = item.GetType().GetProperty(
            nomePropriedade,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        return propriedade?.GetValue(item);
    }
}
