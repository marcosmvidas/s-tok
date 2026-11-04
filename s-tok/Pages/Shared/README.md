# Pages/Shared

Esta pasta reúne os componentes Razor reutilizados pelas páginas do S-TOK e os modelos de configuração usados por esses componentes.

## Layout geral

- `_Layout.cshtml`: estrutura principal da aplicação (cabeçalho, navegação, conteúdo e scripts globais).

## Listagens

- `_Index.cshtml`: estrutura da tabela de listagem. Recebe um `GridListagemModel`.
- `_IndexCadastro.cshtml`: composição usada nas listagens dos módulos; exibe alertas de TempData e depois renderiza `_Index`.
- `_CabecalhoPagina.cshtml`: título, descrição e botão para criar um novo registro.
- `_BotoesAcoes.cshtml`: ações por linha (detalhes, alterar e excluir).
- `_AlertasTempData.cshtml`: mensagens de sucesso e erro.

## Detalhes

- `_Detail.cshtml`: apresentação padronizada dos campos de um registro. Recebe um `ResumoDetalhesModel`.

## Formulários

- `_Registro.cshtml`: layout único dos formulários. As páginas definem os campos; o layout fornece cabeçalho, painel, validação e botões de salvar/cancelar.
- `_BotoesFormulario.cshtml`: botões de envio e cancelamento.
- `_ValidationScriptsPartial.cshtml`: scripts de validação do lado do cliente.

Quando `ViewData["ModoComplementar"]` estiver definido como `true`, `_Registro` não cria um formulário POST nem inclui os botões padrão. Esse modo é usado para páginas que exibem formulários complementares próprios.

## Dados complementares de Pessoa

- `_DadosComplementaresPessoa.cshtml`: interface compartilhada para gerenciar Contatos, Endereços e Documentos vinculados à Pessoa.
- `DadosComplementaresPessoaModel.cs`: modelo de configuração dessa interface.

## Exclusão

- `_ConfirmacaoExclusao.cshtml`: confirmação padronizada antes de excluir um registro.

## Modelos de apoio

Os arquivos `*Model.cs` definem os dados que os componentes recebem. Eles não são layouts nem partials:

- `AcoesListagemModel.cs`
- `BotoesFormularioModel.cs`
- `CabecalhoPaginaModel.cs`
- `CampoDetalheModel.cs`
- `ConfirmacaoExclusaoModel.cs`
- `DadosComplementaresPessoaModel.cs`
- `GridColunaModel.cs`
- `GridListagemModel.cs`
- `ResumoDetalhesModel.cs`

## Convenção

- Cada página de listagem deve preferir `_IndexCadastro` (ou `_Index` quando não precisar dos alertas).
- Cada página de detalhes deve chamar `_Detail` diretamente.
- Cada página de cadastro/alteração deve usar `Layout = "_Registro"`.
- Evite criar um novo layout ou wrapper que apenas repasse conteúdo para outro componente. Antes de criar um partial, verifique se a responsabilidade já existe nesta pasta.
