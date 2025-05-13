// EntityLayer/Loan.cs
public class Loan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateTime StartDate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalRepayment { get; set; }
    public bool IsApproved { get; set; }
}