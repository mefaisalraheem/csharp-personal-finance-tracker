using System.Text.Json;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Data;

public sealed class JsonFinanceRepository : IFinanceRepository
{
    private readonly string _path;
    private readonly List<Transaction> _transactions = new();
    private readonly List<Budget> _budgets = new();
    private readonly List<RecurringPayment> _recurring = new();

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonFinanceRepository(string path)
    {
        _path = path;
    }

    private sealed class Store
    {
        public List<Transaction> Transactions { get; set; } = new();
        public List<Budget> Budgets { get; set; } = new();
        public List<RecurringPayment> RecurringPayments { get; set; } = new();
    }

    public void Load()
    {
        _transactions.Clear();
        _budgets.Clear();
        _recurring.Clear();

        if (!File.Exists(_path))
            return;

        var json = File.ReadAllText(_path);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var store = JsonSerializer.Deserialize<Store>(json, Options);
        if (store is null)
            return;

        _transactions.AddRange(store.Transactions);
        _budgets.AddRange(store.Budgets);
        _recurring.AddRange(store.RecurringPayments);
    }

    public void Save()
    {
        var store = new Store
        {
            Transactions = _transactions,
            Budgets = _budgets,
            RecurringPayments = _recurring
        };
        var json = JsonSerializer.Serialize(store, Options);
        File.WriteAllText(_path, json);
    }

    public IReadOnlyList<Transaction> GetTransactions() => _transactions;
    public IReadOnlyList<Budget> GetBudgets() => _budgets;
    public IReadOnlyList<RecurringPayment> GetRecurringPayments() => _recurring;

    public void AddTransaction(Transaction transaction) => _transactions.Add(transaction);
    public void AddBudget(Budget budget) => _budgets.Add(budget);
    public void AddRecurringPayment(RecurringPayment payment) => _recurring.Add(payment);

    public void UpdateRecurringPayment(RecurringPayment payment)
    {
        var idx = _recurring.FindIndex(r => r.Id == payment.Id);
        if (idx >= 0) _recurring[idx] = payment;
        else _recurring.Add(payment);
    }
}