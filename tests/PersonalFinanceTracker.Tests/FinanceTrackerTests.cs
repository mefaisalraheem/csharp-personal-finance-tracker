using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using Xunit;

namespace PersonalFinanceTracker.Tests;

public class FinanceTrackerTests
{
    private static FinanceTracker NewTracker() => new(new InMemoryRepository());

    [Fact]
    public void AddTransaction_RejectsNonPositiveAmount()
    {
        var tracker = NewTracker();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            tracker.AddTransaction(TransactionType.Expense, 0m, "Food", "x", DateTime.Today));
    }

    [Fact]
    public void AddTransaction_StoresAndReturns()
    {
        var tracker = NewTracker();
        tracker.AddTransaction(TransactionType.Income, 100m, "Salary", "pay", new DateTime(2025, 1, 5));
        Assert.Single(tracker.GetTransactions());
        Assert.Equal(100m, tracker.TotalIncome());
        Assert.Equal(0m, tracker.TotalExpenses());
    }

    [Fact]
    public void Totals_UseDecimalMath()
    {
        var tracker = NewTracker();
        tracker.AddTransaction(TransactionType.Income, 0.1m, "A", "", DateTime.Today);
        tracker.AddTransaction(TransactionType.Income, 0.2m, "A", "", DateTime.Today);
        // double would give 0.30000000000000004
        Assert.Equal(0.3m, tracker.TotalIncome());
    }

    [Fact]
    public void ExpensesByCategory_GroupsWithLinq()
    {
        var tracker = NewTracker();
        tracker.AddTransaction(TransactionType.Expense, 10m, "Food", "", DateTime.Today);
        tracker.AddTransaction(TransactionType.Expense, 5m, "Food", "", DateTime.Today);
        tracker.AddTransaction(TransactionType.Expense, 7m, "Transport", "", DateTime.Today);

        var byCat = tracker.ExpensesByCategory();
        Assert.Equal(15m, byCat["Food"]);
        Assert.Equal(7m, byCat["Transport"]);
    }

    [Fact]
    public void SetBudget_UpdatesExisting()
    {
        var tracker = NewTracker();
        tracker.SetBudget("Food", 100m);
        tracker.SetBudget("Food", 200m);
        Assert.Single(tracker.GetBudgets());
        Assert.Equal(200m, tracker.GetBudgets()[0].MonthlyLimit);
    }

    [Fact]
    public void RecurringPayments_MaterializeUpToDate()
    {
        var tracker = NewTracker();
        tracker.AddRecurringPayment(TransactionType.Expense, 15m, "Streaming", "Netflix", 10,
            new DateTime(2025, 1, 1));

        var created = tracker.ProcessRecurringPayments(new DateTime(2025, 3, 31));
        Assert.Equal(3, created.Count); // Jan 10, Feb 10, Mar 10

        // Calling again for the same period must not duplicate
        var again = tracker.ProcessRecurringPayments(new DateTime(2025, 3, 31));
        Assert.Empty(again);
    }
}