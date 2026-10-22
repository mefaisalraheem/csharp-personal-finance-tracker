using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Services;

/// <summary>
/// Core domain service coordinating transactions, budgets, and recurring payments.
/// </summary>
public sealed class FinanceTracker
{
    private readonly IFinanceRepository _repository;
    private readonly List<Transaction> _transactions;
    private readonly List<Budget> _budgets;
    private readonly List<RecurringPayment> _recurring;

    public FinanceTracker(IFinanceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _repository.Load();
        _transactions = _repository.GetTransactions().ToList();
        _budgets = _repository.GetBudgets().ToList();
        _recurring = _repository.GetRecurringPayments().ToList();
    }

    // ---------------- Transactions ----------------

    public Transaction AddTransaction(
        TransactionType type,
        decimal amount,
        string category,
        string description,
        DateTime date)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.", nameof(category));

        var tx = new Transaction
        {
            Type = type,
            Amount = amount,
            Category = category.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Date = date.Date
        };

        _transactions.Add(tx);
        _repository.AddTransaction(tx);
        return tx;
    }

    public IReadOnlyList<Transaction> GetTransactions() =>
        _transactions.OrderByDescending(t => t.Date).ToList();

    public IReadOnlyList<Transaction> GetTransactionsForMonth(int year, int month) =>
        _transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .OrderBy(t => t.Date)
            .ToList();

    // ---------------- Budgets ----------------

    public Budget SetBudget(string category, decimal monthlyLimit)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.", nameof(category));
        if (monthlyLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyLimit));

        var existing = _budgets.FirstOrDefault(b =>
            string.Equals(b.Category, category, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            existing.MonthlyLimit = monthlyLimit;
            return existing;
        }

        var budget = new Budget(category.Trim(), monthlyLimit);
        _budgets.Add(budget);
        _repository.AddBudget(budget);
        return budget;
    }

    public IReadOnlyList<Budget> GetBudgets() =>
        _budgets.OrderBy(b => b.Category).ToList();

    // ---------------- Recurring Payments ----------------

    public RecurringPayment AddRecurringPayment(
        TransactionType type,
        decimal amount,
        string category,
        string description,
        int dayOfMonth,
        DateTime startDate)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (dayOfMonth < 1 || dayOfMonth > 28)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Day must be between 1 and 28.");

        var rp = new RecurringPayment
        {
            Type = type,
            Amount = amount,
            Category = category.Trim(),
            Description = description?.Trim() ?? string.Empty,
            DayOfMonth = dayOfMonth,
            StartDate = startDate.Date
        };

        _recurring.Add(rp);
        _repository.AddRecurringPayment(rp);
        return rp;
    }

    public IReadOnlyList<RecurringPayment> GetRecurringPayments() => _recurring.ToList();

    /// <summary>
    /// Materializes recurring payments up to <paramref name="asOf"/> that haven't
    /// already been applied. Returns the transactions created.
    /// </summary>
    public IReadOnlyList<Transaction> ProcessRecurringPayments(DateTime asOf)
    {
        var created = new List<Transaction>();
        asOf = asOf.Date;

        foreach (var rp in _recurring)
        {
            var cursor = rp.LastApplied ?? rp.StartDate.AddDays(-1);
            var candidate = new DateTime(cursor.Year, cursor.Month, 1).AddMonths(1);
            candidate = new DateTime(candidate.Year, candidate.Month,
                Math.Min(rp.DayOfMonth, DateTime.DaysInMonth(candidate.Year, candidate.Month)));

            while (candidate <= asOf)
            {
                if (candidate >= rp.StartDate)
                {
                    var tx = rp.ToTransaction(candidate);
                    _transactions.Add(tx);
                    _repository.AddTransaction(tx);
                    created.Add(tx);
                }
                candidate = candidate.AddMonths(1);
            }

            if (candidate > asOf)
                rp.LastApplied = asOf;
            _repository.UpdateRecurringPayment(rp);
        }

        return created;
    }

    // ---------------- Queries / LINQ ----------------

    public decimal TotalIncome() =>
        _transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);

    public decimal TotalExpenses() =>
        _transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

    public IReadOnlyDictionary<string, decimal> ExpensesByCategory() =>
        _transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

    public void Save() => _repository.Save();
}