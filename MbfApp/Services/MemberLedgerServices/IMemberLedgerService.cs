namespace MbfApp.Services;

public interface IMemberLedgerService
{
    Task ImportFromCsvAsync(Stream csvStream);
}

