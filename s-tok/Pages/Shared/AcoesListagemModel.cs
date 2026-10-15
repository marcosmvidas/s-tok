namespace s_tok.Pages.Shared
{
    /// <summary>
    /// Configuração dos links e do formulário de ações usado nas grids.
    /// Cada módulo informa suas próprias Razor Pages.
    /// </summary>
    public class AcoesListagemModel
    {
        public string Id { get; set; } = string.Empty;
        public string DetalhesPage { get; set; } = string.Empty;
        public string AlterarPage { get; set; } = string.Empty;
        public string ExcluirPage { get; set; } = string.Empty;
        public string? NomeRegistro { get; set; }
    }
}
