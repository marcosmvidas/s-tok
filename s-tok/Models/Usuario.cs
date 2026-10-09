using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s_tok.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome completo.")]
        [StringLength(150)]
        [Display(Name = "Nome completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        public string IdentityUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(IdentityUserId))]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser IdentityUser
        {
            get;
            set;
        } = null!;
    }
}