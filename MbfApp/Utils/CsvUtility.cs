using System;

namespace MbfApp.Utils;

public static class CsvUtility
{
    public static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        bool mustQuote = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');

        if (value.Contains('"'))
        {
            value = value.Replace("\"", "\"\"");
        }

        return mustQuote ? $"\"{value}\"" : value;
    }
}