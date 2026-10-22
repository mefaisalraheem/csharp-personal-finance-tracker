namespace PersonalFinanceTracker.Models;

public sealed class Budget
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Category { get; set; } = "General";
    public decimal MonthlyLimit { get; set; }

    public Budget() { }
    public Budget(string category, decimal monthlyLimit)
    {
        Category = category;
        MonthlyLimit = monthlyLimit;
    }
}