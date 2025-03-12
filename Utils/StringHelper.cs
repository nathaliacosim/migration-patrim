using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace migracao-patrim.Utils;

public static class StringHelper
{
    public static string LimparString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null; // Retorna null se a string for vazia ou apenas espaços

        input = input.Trim(); // Remove espaços extras no início e no fim
        input = RemoverAcentos(input); // Remove acentos
        input = Regex.Replace(input, @"[^a-zA-Z0-9\s@._-]", ""); // Remove caracteres especiais (exceto @, ., _, -)
        input = Regex.Replace(input, @"\s+", " "); // Substitui múltiplos espaços por um único

        return input;
    }

    public static string LimparEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        email = LimparString(email).ToLower(); // Remove espaços extras e transforma em minúsculas
        return email;
    }

    public static string LimparTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return null;

        return new string(telefone.Where(char.IsDigit).ToArray()); // Mantém apenas números
    }

    private static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return texto;

        string normalizado = texto.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}