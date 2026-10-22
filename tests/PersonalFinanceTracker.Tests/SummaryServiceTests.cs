using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using Xunit;

namespace PersonalFinanceTracker.Tests;

public class SummaryServiceTests
{
    [Fact]
    public void Build_ComputesNetAndSavingsRate()
    {
        var txs = new[]
        {
            new Transaction { Type = TransactionType.Income,  Amount = 3000m, Category = "Salary",  Date = new DateTime(2025,1,1) },
            new Transaction { Type = TransactionType.Expense, Amount = 500m,  Category = "Food",    Date = new DateTime(2025,1,15) },
            new Transaction { Type = TransactionType.Expense, Amount = 500m,  Category = "Rent",    Date = new DateTime(2025,1,5) },
            // Different month - must be excluded
            new Transaction { Type = TransactionType.Expense, Amount = 999m,  Category = "Food",    Date = new DateTime(2025,2,1) }
        };
        var budgets = new[] { new Budget("Food", 400m), new Budget("Rent", 600m) };

        var summary = new SummaryService().Build(2025, 1, txs, budgets);

        Assert.Equal(3000m, summary.TotalIncome);
        Assert.Equal(1000m, summary.TotalExpenses);
        Assert.Equal(2000m, summary.NetSavings);
        Assert.Equal(2000m / 3000m, summary.SavingsRate);

        var food = summary.Budgets.Single(b => b.Category == "Food");
        Assert.True(food.IsOverBudget);
        Assert.Equal(100m, food.Overage);

        var rent = summary.Budgets.Single(b => b.Category == "Rent");
        Assert.False(rent.IsOverBudget);
    }

    [Fact]
    public void Build_ZeroIncome_SavingsRateIsZero()
    {
        var summary = new SummaryService().Build(2025, 1, Array.Empty<Transaction>(), Array.Empty<Budget>());
        Assert.Equal(0m, summary.SavingsRate);
    }
}