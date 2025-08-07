namespace MbfApp.Data.Entities;

public class AccountBalance
{
    public int Id { get; set; }
    public  int FinYearId  { get; set; }
    public int AccountId { get; set; }
    public decimal OpBalDb { get; set; }
    public decimal OpBalCr { get; set; }
    public decimal ClBalDb { get; set; }
    public decimal ClBalCr { get; set; }
    public decimal DbTotal { get; set; }
    public decimal CrTotal { get; set; }
    public Account Account { get; set; } = default!;
    public FinYear FinYear { get; set; } = default!;
}
