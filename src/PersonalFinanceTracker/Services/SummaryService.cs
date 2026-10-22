using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Services;

public sealed class SummaryService
{
    /// <summary>
    /// Builds a monthly summary including per-category budget status.
    /// </summary>
    public MonthlySummary Build(int year, int month, IEnumerable<Transaction> transactions, IEnumerable<Budget> budgets)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        var monthTx = transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .ToList();

        var income = monthTx
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expenses = monthTx
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var spendByCategory = monthTx
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var statuses = budgets
            .Select(b => new BudgetStatus(
                b.Category,
                spendByCategory.TryGetValue(b.Category, out var spent) ? spent : 0m,
                b.MonthlyLimit))
            .OrderByDescending(s => s.IsOverBudget)
            .ThenBy(s => s.Category)
            .ToList();

        return new MonthlySummary
        {
            Year = year,
            Month = month,
            TotalIncome = income,
            TotalExpenses = expenses,
            Budgets = statuses
        };
    }
}