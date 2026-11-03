using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum TipoContato
{
    [Display(Name = "Telefone")] Telefone = 1,
    [Display(Name = "WhatsApp")] WhatsApp = 2,
    [Display(Name = "e-mail")] Email = 3,
    [Display(Name = "Outro")] Outro = 9
}
