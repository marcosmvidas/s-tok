
using System.Net.Http.Json;
using s_tok.Models;

namespace s_tok.Services;

public class CepService(HttpClient httpClient) : ICepService
{
    public async Task<Endereco?> ConsultarAsync(
        string cep,
        CancellationToken cancellationToken = default)
    {
        cep = new string(cep.Where(char.IsDigit).ToArray());

        if (cep.Length != 8)
            return null;

        return await httpClient.GetFromJsonAsync<Endereco>(
            $"ws/{cep}/json/",
            cancellationToken);
    }
}
