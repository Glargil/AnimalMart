using System.Text.RegularExpressions;

namespace AnimalMart.Services;

public class PasswordAnalyzerService : IPasswordAnalyzer
{
    private const int MinLength = 8;    // policy: alm. bruger 8-64 tegn
    private const int MaxLength = 64;

    public PasswordResult Analyze(string password, string? username)
    {
        password ??= "";
        username ??= "";
        var feedback = new List<string>();
        bool meetsPolicy = true;

        // ----- Længde: 8-64 (policy) -----
        if (password.Length < MinLength)
        {
            meetsPolicy = false;
            feedback.Add($"Passwordet skal være mindst {MinLength} tegn. (15 anbefales)");
        }
        if (password.Length > MaxLength)
        {
            meetsPolicy = false;
            feedback.Add($"Passwordet må maks være {MaxLength} tegn.");
        }

        // ----- Kompleksitet: mindst ét af hver (policy) -----
        if (!password.Any(char.IsLower))
        {
            meetsPolicy = false;
            feedback.Add("Passwordet skal indeholde mindst ét lille bogstav.");
        }

        if (!password.Any(char.IsUpper))
        {
            meetsPolicy = false;
            feedback.Add("Passwordet skal indeholde mindst ét stort bogstav.");
        }

        if (!password.Any(char.IsDigit))
        {
            meetsPolicy = false;
            feedback.Add("Passwordet skal indeholde mindst ét tal.");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            meetsPolicy = false;
            feedback.Add("Passwordet skal indeholde mindst ét specialtegn (fx !, -, _, %).");
        }

        // ----- Forbudt: brugernavn i passwordet (policy) -----
        if (!string.IsNullOrEmpty(username) &&
            password.ToLower().Contains(username.ToLower()))
        {
            meetsPolicy = false;
            feedback.Add("Passwordet må ikke indeholde dit brugernavn.");
        }

        // ----- Tips (ud over policyen): mønstre -----
        bool hasPattern =
            Regex.IsMatch(password, @"(.)\1{2,}") || HasAscendingSequence(password);

        if (Regex.IsMatch(password, @"(.)\1{2,}"))
            feedback.Add("Tip: undgå gentagne tegn som 'aaaa'.");

        if (HasAscendingSequence(password))
            feedback.Add("Tip: undgå sekvenser som '1234' eller 'abcd'.");

        // ----- Score -----
        int score = 0;
        if (password.Length >= MinLength && password.Length <= MaxLength) score += 20;
        if (password.Any(char.IsLower))  score += 15;
        if (password.Any(char.IsUpper))  score += 15;
        if (password.Any(char.IsDigit)) score += 15;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score += 15;
        if (meetsPolicy && password.Length >= 15) score += 10;  // policy: 15 tegn anbefales
        if (password.Length >= 25) score += 10;                   // ekstra længde belønnes

        // Mønstre kan aldrig gøre passwordet "Stærkt" — men det er stadig policy-godkendt
        if (hasPattern) score = Math.Min(score, 60);

        score = Math.Clamp(score, 0, 100);

        PasswordCategory category = score switch
        {
            < 40 => PasswordCategory.Weak,
            < 70 => PasswordCategory.Medium,
            _    => PasswordCategory.Strong
        };

        return new PasswordResult(score, category, feedback, meetsPolicy);
    }

    private static bool HasAscendingSequence(string password, int length = 3)
    {
        if (password.Length < length) return false;

        for (int i = 0; i <= password.Length - length; i++)
        {
            bool ascending = true;
            for (int j = 1; j < length; j++)
            {
                if (password[i + j] != password[i] + j)
                {
                    ascending = false;
                    break;
                }
            }
            if (ascending) return true;
        }
        return false;
    }
}