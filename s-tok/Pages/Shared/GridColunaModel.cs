namespace s_tok.Pages.Shared;

/// <summary>
/// Define uma coluna de uma listagem compartilhada.
/// </summary>
public sealed class GridColunaModel
{
    public string Titulo { get; set; } = string.Empty;
    public string Propriedade { get; set; } = string.Empty;
    public string? ClasseCss { get; set; }
    public string? Formato { get; set; }
}
