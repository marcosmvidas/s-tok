namespace s_tok.Pages.Shared;

public sealed class DadosComplementaresPessoaModel
{
    public int PessoaId { get; set; }
    public string NomePessoa { get; set; } = string.Empty;
    public string PaginaConcluir { get; set; } = "/Cliente/Index";
    public bool PossuiDocumento { get; set; }
    public int QuantidadeContatos { get; set; }
    public int QuantidadeEnderecos { get; set; }

    /// <summary>
    /// Exibe somente os cards informativos, com os botões desabilitados,
    /// enquanto o cadastro principal ainda não foi salvo.
    /// </summary>
    public bool SomenteVisualizacao { get; set; }
}
