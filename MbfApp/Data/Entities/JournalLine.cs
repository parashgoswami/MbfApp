namespace MbfApp.Data.Entities;

public class JournalLine
{
    public int Id { get; set; }
    public int JournalId { get; set; }
    public int AccountId { get; set; }
    public decimal DbAmt { get; set; }
    public decimal CrAmt { get; set; }
    public string? Note { get; set; }
    public Journal Journal { get; set; } = default!;
}
