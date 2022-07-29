using System.Reflection;
using System.Text.RegularExpressions;
using HaycLib.Lexing;

namespace HaycLib.Extensions;

public static class TokenTypeExtensions
{
    static TokenTypeExtensions()
    {
        Patterns = new Dictionary<TokenType, IEnumerable<Regex>>();

        // Static binding flag is required, because enums have a value__ member
        // that will break our code :)
        foreach (FieldInfo val in typeof(TokenType).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            TokenType tokenType = (TokenType) val.GetValue(null)!;

            if (tokenType == TokenType.Invalid)
            {
                // Special case wohoo
                continue;
            }

            IEnumerable<TokenTypeAttribute> attributesEnumerable
                = val.GetCustomAttributes<TokenTypeAttribute>();
            TokenTypeAttribute[] attributes
                = attributesEnumerable as TokenTypeAttribute[] ?? attributesEnumerable.ToArray();
            if (!attributes.Any())
            {
                throw new InvalidOperationException($"TokenType {val.Name} does not define a regex.");
            }

            Patterns[tokenType] = attributes.Select(x => x.Regex);
        }
    }

    /// <summary>
    /// Maps token types to their regex patterns.
    /// </summary>
    private static readonly Dictionary<TokenType, IEnumerable<Regex>> Patterns;

    /// <summary>
    /// Retrieves the regex object for the given token type.
    /// </summary>
    public static IEnumerable<Regex> GetRegexes(this TokenType tokenType)
    {
        return Patterns[tokenType];
    }

    /// <summary>
    /// Retrieves the precedence for the given token type.
    /// </summary>
    public static int GetPrecedence(this TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.Plus or TokenType.Minus => 0,
            TokenType.Asterisk or TokenType.Slash or TokenType.Percent => 1,
            _ => -1
        };
    }

    /// <summary>
    /// Whether an operator is right-assoc.
    /// </summary>
    public static bool IsRightAssociative(this TokenType tokenType)
    {
        return tokenType == TokenType.Power;
    }
}