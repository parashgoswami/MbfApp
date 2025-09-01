namespace MbfApp.Data.Entities;
public class VoucherSequence
{
    public int Id { get; set; }
    public string FinancialYear { get; set; } = default!;
    public int LastNumber { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
