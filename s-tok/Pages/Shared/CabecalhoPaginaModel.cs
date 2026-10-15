namespace s_tok.Pages.Shared;

/// <summary>
/// Dados para o cabeçalho padronizado das páginas de listagem e cadastro.
/// </summary>
public sealed class CabecalhoPaginaModel
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? PaginaNovo { get; set; }
    public string TextoBotaoNovo { get; set; } = "Novo cadastro";
}
