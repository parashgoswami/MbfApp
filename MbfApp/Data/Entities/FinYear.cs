namespace MbfApp.Data.Entities;

public class FinYear
{
    public int Id { get; set; }
    public string FinYrLabel { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float IntDeposit { get; set; }
    public float IntLoan { get; set; }
    public bool IsClosed { get; set; } = false;
    public bool IsCurrent { get; set; } = true;
}
