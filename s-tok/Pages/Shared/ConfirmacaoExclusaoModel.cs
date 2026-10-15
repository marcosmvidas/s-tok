namespace s_tok.Pages.Shared;

/// <summary>
/// Dados da confirmação de exclusão reutilizável.
/// A autorização e a exclusão efetiva continuam sob responsabilidade da página.
/// </summary>
public sealed class ConfirmacaoExclusaoModel
{
    public string Titulo { get; set; } = "Excluir registro";
    public string? Descricao { get; set; }
    public string NomeRegistro { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string PaginaVoltar { get; set; } = "/Index";
    public string TextoConfirmar { get; set; } = "Confirmar exclusão";
    public string Handler { get; set; } = "Delete";
}
