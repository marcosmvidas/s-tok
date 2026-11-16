
namespace s_tok.Services;

public static class PasswordPolicy
{
    public const int MinimoSenhaInterna = 8;
    public const int MinimoSenhaForte = 12;

    public static bool IsStrong(string? senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return false;

        return senha.Length >= MinimoSenhaForte
            && senha.Any(char.IsUpper)
            && senha.Any(char.IsLower)
            && senha.Any(char.IsDigit)
            && senha.Any(c => !char.IsLetterOrDigit(c));
    }

    public const string MensagemSenhaForte =
        "A senha deve conter pelo menos 12 caracteres, " +
        "incluindo letras maiúsculas, minúsculas, números e símbolos.";
}
