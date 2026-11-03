using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum TipoDocumento
{
    [Display(Name = "CPF")] Cpf = 1,
    [Display(Name = "CNPJ")] Cnpj = 2,
    [Display(Name = "RG")] Rg = 3,
    [Display(Name = "CNH")] Cnh = 4
}
