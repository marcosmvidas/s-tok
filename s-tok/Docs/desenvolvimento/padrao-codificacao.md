# S-TOK — Padrão de Codificação

**Versão:** 1.0
**Status:** Inicial
**Projeto:** S-TOK
**Tecnologia base:** ASP.NET Core / C#

---

## 1. Objetivo

Este documento define os padrões e princípios utilizados no desenvolvimento do S-TOK.

Seu objetivo é manter o código:

* organizado;
* legível;
* previsível;
* fácil de manter;
* reutilizável;
* testável;
* evolutivo.

Este documento deve ser considerado uma referência permanente do projeto, independentemente da quantidade de chats utilizados durante o desenvolvimento.

---

# 2. Princípios fundamentais

O S-TOK seguirá, sempre que aplicável, os seguintes princípios:

## 2.1 KISS — Keep It Simple

Preferir soluções simples e claras.

Não criar abstrações, classes, interfaces ou camadas apenas porque podem ser úteis no futuro.

> Se uma solução simples resolve corretamente o problema atual, ela é a primeira opção.

Complexidade deve existir quando houver uma necessidade real.

---

## 2.2 DRY — Don't Repeat Yourself

Evitar duplicação de:

* regras de negócio;
* validações;
* consultas;
* código de acesso a dados;
* configurações;
* funções;
* componentes.

Quando uma mesma regra aparecer em vários pontos, avaliar se ela deve ser centralizada.

Entretanto, não criar abstrações prematuramente apenas para eliminar pequenas repetições.

---

## 2.3 SOLID

O código deve buscar os princípios SOLID.

### S — Single Responsibility

Uma classe, método ou componente deve possuir uma responsabilidade clara.

Evitar classes que façam simultaneamente:

* acesso ao banco;
* validação;
* regra de negócio;
* apresentação;
* comunicação externa.

### O — Open/Closed

Preferir estruturas que possam ser estendidas sem alterar excessivamente código já estabilizado.

### L — Liskov Substitution

Implementações derivadas devem respeitar o comportamento esperado de suas abstrações.

### I — Interface Segregation

Interfaces devem ser pequenas e específicas.

Evitar interfaces gigantes contendo métodos que nem todas as implementações utilizam.

### D — Dependency Inversion

Componentes de alto nível não devem depender diretamente de implementações concretas quando uma abstração fizer sentido.

Utilizar injeção de dependência do ASP.NET Core quando apropriado.

---

# 3. Responsabilidade das camadas

Cada componente deve possuir uma responsabilidade definida.

Como regra geral:

```text
Interface / UI
      ↓
Aplicação / Serviços
      ↓
Domínio / Regras
      ↓
Persistência / Infraestrutura
```

Uma camada não deve assumir responsabilidades pertencentes a outra.

Por exemplo:

* a interface não deve conter regras complexas de negócio;
* o acesso ao banco não deve ficar espalhado pelas páginas;
* serviços externos não devem ser chamados diretamente de qualquer lugar;
* regras de negócio não devem depender da interface visual.

A estrutura definitiva das camadas será definida no documento de arquitetura do S-TOK.

---

# 4. Nomenclatura

Utilizar nomes claros e descritivos.

## Classes

Utilizar `PascalCase`.

```csharp
ClienteService
ProdutoModel
EstoqueRepository
```

## Métodos

Utilizar `PascalCase`.

```csharp
BuscarCliente()
SalvarProduto()
ConsultarEstoque()
```

## Variáveis

Utilizar `camelCase`.

```csharp
cliente
produto
quantidadeAtual
```

## Constantes

Utilizar `PascalCase`, salvo convenção específica da tecnologia adotada.

```csharp
MaxTentativas
TempoExpiracao
```

---

# 5. Métodos

Métodos devem ser pequenos e possuir uma responsabilidade clara.

Preferir:

```csharp
BuscarCliente()
ValidarCliente()
SalvarCliente()
```

a um método gigantesco responsável por todas essas operações.

Quando um método começar a concentrar muitas responsabilidades, avaliar sua divisão.

---

# 6. Código assíncrono

Operações de I/O devem utilizar programação assíncrona quando apropriado.

Exemplo:

```csharp
public async Task<ClienteModel?> BuscarAsync(int id)
{
    ...
}
```

Evitar bloquear operações assíncronas utilizando `.Result` ou `.Wait()` sem necessidade.

Preferir:

```csharp
await servico.BuscarAsync();
```

---

# 7. Tratamento de erros

Erros devem ser tratados de maneira explícita e coerente.

Não utilizar `try/catch` indiscriminadamente.

O `catch` deve existir quando houver uma ação adequada a ser tomada, como:

* registrar o erro;
* converter uma exceção técnica em uma resposta apropriada;
* executar uma recuperação;
* informar uma falha de integração.

Evitar:

```csharp
try
{
    ...
}
catch
{
}
```

Exceções nunca devem ser simplesmente ignoradas.

---

# 8. Logs

O S-TOK deverá utilizar o sistema de logging do ASP.NET Core.

Preferir `ILogger<T>`.

Exemplo:

```csharp
private readonly ILogger<ClienteService> _logger;
```

Os logs devem ajudar a responder:

* o que aconteceu;
* onde aconteceu;
* quando aconteceu;
* qual operação estava sendo executada;
* qual foi o resultado.

Não registrar informações sensíveis desnecessariamente.

---

# 9. Acesso a dados

Código relacionado ao banco de dados deve permanecer concentrado nas estruturas responsáveis por persistência.

Evitar consultas SQL ou operações de banco espalhadas pela aplicação.

A implementação definitiva será definida na documentação de arquitetura e banco de dados.

---

# 10. Validação

Validações devem ocorrer no nível apropriado.

Devemos diferenciar:

### Validação de entrada

Verifica se os dados fornecidos são aceitáveis.

Exemplos:

* campo obrigatório;
* formato;
* tamanho;
* tipo;
* valores permitidos.

### Regra de negócio

Verifica se uma operação é permitida dentro das regras do S-TOK.

Exemplo:

```text
Não permitir saída de estoque
quando a quantidade disponível for insuficiente.
```

A validação de entrada não deve substituir a validação das regras de negócio.

---

# 11. Configurações

Valores configuráveis não devem ser espalhados pelo código.

Evitar:

```csharp
var timeout = 30;
```

quando `30` representar uma configuração que pode mudar.

Utilizar os mecanismos de configuração do ASP.NET Core quando apropriado.

Arquivos de configuração:

```text
appsettings.json
appsettings.Development.json
```

Segredos e credenciais não devem ser armazenados no Git.

---

# 12. Comentários

O código deve ser escrito de forma suficientemente clara para reduzir a necessidade de comentários.

Comentários devem explicar principalmente:

* decisões não óbvias;
* regras especiais;
* limitações;
* motivos de uma implementação específica.

Evitar comentários que apenas repetem o código.

Ruim:

```csharp
// Incrementa um
contador++;
```

Melhor:

```csharp
// Mantemos o contador para evitar nova consulta ao banco durante a mesma operação.
contador++;
```

---

# 13. Código morto

Não manter código comentado ou arquivos antigos apenas por segurança.

Quando algo deixar de ser utilizado:

1. confirmar que não possui dependências;
2. remover;
3. registrar a alteração no Git.

O histórico do Git é o mecanismo de recuperação.

---

# 14. Duplicidade de arquivos

Não criar versões alternativas de um mesmo arquivo.

Evitar nomes como:

```text
ClienteServiceNovo.cs
ClienteService2.cs
ClienteServiceFinal.cs
ClienteServiceFinal2.cs
```

Quando uma implementação for substituída, o arquivo antigo deve ser removido ou adequadamente reorganizado.

---

# 15. Organização de pastas

Cada pasta deve possuir uma finalidade clara.

Não criar uma pasta apenas para "guardar" um arquivo.

Antes de criar uma nova pasta, verificar:

1. qual é a responsabilidade do arquivo;
2. qual camada possui essa responsabilidade;
3. se já existe uma pasta apropriada;
4. se a nova pasta realmente agrega organização.

A estrutura oficial será definida em:

```text
docs/arquitetura/estrutura-projeto.md
```

---

# 16. Dependências

Novas bibliotecas ou pacotes NuGet não devem ser adicionados sem necessidade.

Antes de adicionar uma dependência:

* verificar se o ASP.NET Core já possui a funcionalidade;
* avaliar se o código pode ser resolvido de forma simples;
* verificar manutenção e compatibilidade;
* avaliar impacto no projeto.

> Dependência nova deve resolver um problema real.

---

# 17. Segurança

Nunca armazenar no código-fonte:

* senhas;
* tokens;
* chaves de API;
* connection strings contendo credenciais;
* certificados privados;
* outras informações secretas.

Também devemos evitar registrar segredos nos logs.

---

# 18. Git

O Git será utilizado como histórico oficial do desenvolvimento.

As alterações devem ser feitas de forma incremental.

Preferir commits que representem uma alteração lógica:

```text
Add estrutura inicial de clientes
Add validação de produto
Fix cálculo de estoque
Add consulta de CEP
```

Evitar commits genéricos como:

```text
mudanças
teste
alterações
coisas
final
```

A branch principal do projeto atualmente é:

```text
master
```

---

# 19. Regra para novos arquivos

Antes de criar qualquer arquivo novo:

1. identificar sua responsabilidade;
2. verificar a arquitetura;
3. verificar se já existe funcionalidade equivalente;
4. verificar se o arquivo está sendo criado na pasta correta;
5. somente então criar o arquivo.

Isso evita duplicação e crescimento desorganizado do projeto.

---

# 20. Regra para novos chats

O desenvolvimento do S-TOK poderá utilizar múltiplos chats.

Um novo chat não deve criar uma arquitetura independente.

A referência deve ser:

```text
Documentação
     ↓
Estrutura atual do projeto
     ↓
Código existente
     ↓
Nova implementação
```

Ao iniciar um novo chat, a estrutura atual do projeto poderá ser apresentada para sincronizar o contexto.

Se houver conflito entre uma sugestão feita anteriormente em outro chat e a documentação atual do projeto, a documentação atual deverá prevalecer até que uma alteração arquitetural seja deliberadamente aprovada.

---

# 21. Alterações arquiteturais

Mudanças estruturais importantes devem ser discutidas antes da implementação.

Exemplos:

* criação de nova camada;
* mudança de responsabilidade entre camadas;
* alteração significativa da persistência;
* introdução de nova tecnologia;
* mudança no padrão de comunicação;
* criação de nova estratégia de autenticação;
* mudança significativa na organização de pastas.

Alterações aprovadas devem ser registradas em:

```text
docs/arquitetura/decisoes-tecnicas.md
```

---

# 22. Regra principal

> **Código simples, responsabilidades claras, pouca duplicação e arquitetura suficiente para o problema — nunca mais complexa do que o necessário.**

O S-TOK deve evoluir de maneira controlada.

Primeiro entendemos o problema.

Depois definimos a responsabilidade.

Depois definimos onde ela pertence.

Só então escrevemos o código.

---

**Documento:** Padrão de Codificação
**Versão:** 1.0
**Projeto:** S-TOK
