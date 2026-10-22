namespace PersonalFinanceTracker.Models;

public sealed class RecurringPayment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "General";
    public string Description { get; set; } = string.Empty;
    public int DayOfMonth { get; set; } = 1;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? LastApplied { get; set; }

    public Transaction ToTransaction(DateTime date) => new()
    {
        Type = Type,
        Amount = Amount,
        Category = Category,
        Description = Description,
        Date = date
    };
}