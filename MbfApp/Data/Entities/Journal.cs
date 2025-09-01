namespace MbfApp.Data.Entities;
public class Journal
{
    public int Id { get; set; }
    public string JvNo { get; set; } = string.Empty;
    public int FinYearId { get; set; }
    public DateTime JournalDate { get; set; }
    public string Narration { get; set; } = string.Empty;
    public List<JournalLine> Lines { get; set; } = new();
    public FinYear FinYear { get; set; } = default!;
}
