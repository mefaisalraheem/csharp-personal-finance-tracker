using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Services;

public interface IFinanceRepository
{
    IReadOnlyList<Transaction> GetTransactions();
    IReadOnlyList<Budget> GetBudgets();
    IReadOnlyList<RecurringPayment> GetRecurringPayments();

    void AddTransaction(Transaction transaction);
    void AddBudget(Budget budget);
    void AddRecurringPayment(RecurringPayment payment);
    void UpdateRecurringPayment(RecurringPayment payment);

    void Save();
    void Load();
}