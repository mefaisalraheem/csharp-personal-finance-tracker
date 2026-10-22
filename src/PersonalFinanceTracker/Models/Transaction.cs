namespace PersonalFinanceTracker.Models;

public sealed class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Category { get; init; } = "General";
    public string Description { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.Today;

    public decimal SignedAmount => Type == TransactionType.Income ? Amount : -Amount;

    public override string ToString() =>
        $"[{Date:yyyy-MM-dd}] {Type,-7} {Category,-14} {Amount,10:C}  {Description}";
}