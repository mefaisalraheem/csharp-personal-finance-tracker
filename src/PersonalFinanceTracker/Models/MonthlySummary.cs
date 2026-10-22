namespace PersonalFinanceTracker.Models;

public sealed class MonthlySummary
{
    public int Year { get; init; }
    public int Month { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal NetSavings => TotalIncome - TotalExpenses;
    public decimal SavingsRate => TotalIncome == 0 ? 0 : NetSavings / TotalIncome;
    public IReadOnlyList<BudgetStatus> Budgets { get; init; } = Array.Empty<BudgetStatus>();

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== Monthly Summary ({Year}-{Month:00}) ===");
        sb.AppendLine($"Income:       {TotalIncome,12:C}");
        sb.AppendLine($"Expenses:     {TotalExpenses,12:C}");
        sb.AppendLine($"Net Savings:  {NetSavings,12:C}");
        sb.AppendLine($"Savings Rate: {SavingsRate,11:P2}");
        sb.AppendLine();
        if (Budgets.Count > 0)
        {
            sb.AppendLine("Budget Status:");
            foreach (var b in Budgets)
                sb.AppendLine($"  {b}");
        }
        return sb.ToString();
    }
}

public sealed record BudgetStatus(
    string Category,
    decimal Spent,
    decimal Limit)
{
    public bool IsOverBudget => Spent > Limit;
    public decimal Overage => IsOverBudget ? Spent - Limit : 0m;

    public override string ToString() =>
        IsOverBudget
            ? $"{Category,-14} {Spent,10:C} / {Limit,10:C}  [OVER by {Overage:C}]"
            : $"{Category,-14} {Spent,10:C} / {Limit,10:C}  [OK]";
}