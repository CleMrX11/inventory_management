using System.Globalization;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles;

internal static class ArticleSpecificityParser
{
    public static DateOnly? ParseExpirationDate(string? expirationDate)
    {
        if (string.IsNullOrWhiteSpace(expirationDate))
        {
            return null;
        }

        return DateOnly.TryParseExact(expirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : throw new ArgumentException("Expiration date must use yyyy-MM-dd format.", nameof(expirationDate));
    }

    public static TakeawayAvailability? ParseTakeawayAvailability(string? takeawayAvailability)
    {
        return ParseOptionalEnum<TakeawayAvailability>(
            takeawayAvailability,
            "Takeaway availability is invalid.",
            nameof(takeawayAvailability));
    }

    public static PackagingLevel? ParsePackagingLevel(string? packagingLevel)
    {
        return ParseOptionalEnum<PackagingLevel>(
            packagingLevel,
            "Packaging level is invalid.",
            nameof(packagingLevel));
    }

    private static TEnum? ParseOptionalEnum<TEnum>(string? value, string errorMessage, string parameterName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : throw new ArgumentException(errorMessage, parameterName);
    }
}
