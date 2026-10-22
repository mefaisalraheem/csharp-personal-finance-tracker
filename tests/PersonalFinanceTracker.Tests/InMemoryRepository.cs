using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Tests;

public sealed class InMemoryRepository : IFinanceRepository
{
    private readonly List<Transaction> _tx = new();
    private readonly List<Budget> _budgets = new();
    private readonly List<RecurringPayment> _rp = new();

    public IReadOnlyList<Transaction> GetTransactions() => _tx;
    public IReadOnlyList<Budget> GetBudgets() => _budgets;
    public IReadOnlyList<RecurringPayment> GetRecurringPayments() => _rp;
    public void AddTransaction(Transaction t) => _tx.Add(t);
    public void AddBudget(Budget b) => _budgets.Add(b);
    public void AddRecurringPayment(RecurringPayment p) => _rp.Add(p);
    public void UpdateRecurringPayment(RecurringPayment p) { /* no-op for in-memory (same instance) */ }
    public void Save() { }
    public void Load() { }
}