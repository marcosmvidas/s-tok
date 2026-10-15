namespace s_tok.Pages.Shared;

/// <summary>
/// Configuração para exibir os dados de um registro em uma página de detalhes.
/// Os dados e as permissões continuam sendo definidos por cada módulo.
/// </summary>
public sealed class ResumoDetalhesModel
{
    public string Titulo { get; set; } = "Detalhes";
    public string? Descricao { get; set; }
    public string Id { get; set; } = string.Empty;
    public string? PaginaVoltar { get; set; }
    public string? PaginaAlterar { get; set; }
    public IReadOnlyList<CampoDetalheModel> Campos { get; set; } = [];
}
