namespace MbfApp.Dtos.FinYear;

public record FinYearResponse
{
    public int Id { get; set; }
    public string FinYrLabel { get; set; } = string.Empty;    
    public float IntDeposit { get; set; }
    public float IntLoan { get; set; }
}
