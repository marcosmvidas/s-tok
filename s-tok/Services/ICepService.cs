
using s_tok.Models;

namespace s_tok.Services;

public interface ICepService
{
    Task<Endereco?> ConsultarAsync(
        string cep,
        CancellationToken cancellationToken = default);
}
