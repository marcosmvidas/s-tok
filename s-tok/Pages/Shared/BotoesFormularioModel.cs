namespace s_tok.Pages.Shared;

/// <summary>
/// Configuração dos botões padrão dos formulários de cadastro e edição.
/// </summary>
public sealed class BotoesFormularioModel
{
    public string TextoSalvar { get; set; } = "Salvar";
    public string TextoCancelar { get; set; } = "Cancelar";
    public string PaginaCancelar { get; set; } = "/Index";
    public string? RotaIdCancelar { get; set; }
}
