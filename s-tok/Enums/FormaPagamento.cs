using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum FormaPagamento
{
    [Display(Name = "Dinheiro")] Dinheiro = 1,
    [Display(Name = "Pix")] Pix = 2,
    [Display(Name = "Cartão de débito")] CartaoDebito = 3,
    [Display(Name = "Cartão de crédito")] CartaoCredito = 4,
    [Display(Name = "Boleto")] Boleto = 5,
    [Display(Name = "Fiado")] Fiado = 6
}
