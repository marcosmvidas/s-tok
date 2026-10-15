namespace s_tok.Pages.Shared;

/// <summary>
/// Representa um campo apresentado na visualização de detalhes de um registro.
/// </summary>
public sealed class CampoDetalheModel
{
    public string Rotulo { get; set; } = string.Empty;
    public string? Valor { get; set; }
}
