using System.Diagnostics.CodeAnalysis;

namespace RecipeBook.Domain.Extensions;

public static class StringExtension
{
    public static bool IsNotEmpty([NotNullWhen(true)]this string? str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }
}