namespace MbfApp.Services.VoucherNoService;
public static class FinancialYearHelper
{
    public static string GetFinancialYear(DateTime date)
    {
        int year = date.Month >= 4 ? date.Year : date.Year - 1;
        return $"{year}-{(year + 1).ToString().Substring(2)}";
    }
}
