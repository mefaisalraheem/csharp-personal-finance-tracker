using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

var dataPath = Path.Combine(Environment.CurrentDirectory, "finance-data.json");
var repo = new JsonFinanceRepository(dataPath);
var tracker = new FinanceTracker(repo);
var summaryService = new SummaryService();

Console.WriteLine("=== Personal Finance Tracker ===");

bool running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1) Add income");
    Console.WriteLine("2) Add expense");
    Console.WriteLine("3) Set budget");
    Console.WriteLine("4) Add recurring payment");
    Console.WriteLine("5) Process recurring payments");
    Console.WriteLine("6) Show transactions");
    Console.WriteLine("7) Show monthly summary");
    Console.WriteLine("8) List budgets");
    Console.WriteLine("9) Save & exit");
    Console.Write("> ");

    var choice = Console.ReadLine()?.Trim();
    try
    {
        switch (choice)
        {
            case "1": AddTransaction(tracker, TransactionType.Income); break;
            case "2": AddTransaction(tracker, TransactionType.Expense); break;
            case "3": SetBudget(tracker); break;
            case "4": AddRecurring(tracker); break;
            case "5": ProcessRecurring(tracker); break;
            case "6": ShowTransactions(tracker); break;
            case "7": ShowSummary(tracker, summaryService); break;
            case "8": ListBudgets(tracker); break;
            case "9":
                tracker.Save();
                Console.WriteLine("Data saved. Goodbye!");
                running = false;
                break;
            default:
                Console.WriteLine("Unknown option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void AddTransaction(FinanceTracker tracker, TransactionType type)
{
    Console.Write("Category: ");
    var category = Console.ReadLine() ?? "";

    Console.Write("Amount: ");
    if (!decimal.TryParse(Console.ReadLine(), out var amount))
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    Console.Write("Description: ");
    var desc = Console.ReadLine() ?? "";

    Console.Write("Date (yyyy-MM-dd, blank=today): ");
    var dateInput = Console.ReadLine();
    var date = string.IsNullOrWhiteSpace(dateInput) ? DateTime.Today : DateTime.Parse(dateInput);

    tracker.AddTransaction(type, amount, category, desc, date);
    Console.WriteLine($"{type} added.");
}

static void SetBudget(FinanceTracker tracker)
{
    Console.Write("Category: ");
    var cat = Console.ReadLine() ?? "";

    Console.Write("Monthly limit: ");
    if (!decimal.TryParse(Console.ReadLine(), out var limit))
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    tracker.SetBudget(cat, limit);
    Console.WriteLine("Budget saved.");
}

static void AddRecurring(FinanceTracker tracker)
{
    Console.Write("Type (income/expense): ");
    var typeStr = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
    var type = typeStr == "income" ? TransactionType.Income : TransactionType.Expense;

    Console.Write("Category: ");
    var cat = Console.ReadLine() ?? "";

    Console.Write("Amount: ");
    if (!decimal.TryParse(Console.ReadLine(), out var amount)) { Console.WriteLine("Invalid amount."); return; }

    Console.Write("Description: ");
    var desc = Console.ReadLine() ?? "";

    Console.Write("Day of month (1-28): ");
    if (!int.TryParse(Console.ReadLine(), out var day)) { Console.WriteLine("Invalid day."); return; }

    Console.Write("Start date (yyyy-MM-dd, blank=today): ");
    var sd = Console.ReadLine();
    var start = string.IsNullOrWhiteSpace(sd) ? DateTime.Today : DateTime.Parse(sd);

    tracker.AddRecurringPayment(type, amount, cat, desc, day, start);
    Console.WriteLine("Recurring payment saved.");
}

static void ProcessRecurring(FinanceTracker tracker)
{
    var created = tracker.ProcessRecurringPayments(DateTime.Today);
    Console.WriteLine($"{created.Count} recurring transaction(s) materialized.");
    foreach (var t in created) Console.WriteLine("  " + t);
}

static void ShowTransactions(FinanceTracker tracker)
{
    var txs = tracker.GetTransactions();
    if (txs.Count == 0) { Console.WriteLine("No transactions."); return; }
    foreach (var t in txs) Console.WriteLine(t);
}

static void ShowSummary(FinanceTracker tracker, SummaryService summaryService)
{
    Console.Write("Year (yyyy): ");
    if (!int.TryParse(Console.ReadLine(), out var year)) { Console.WriteLine("Invalid year."); return; }
    Console.Write("Month (1-12): ");
    if (!int.TryParse(Console.ReadLine(), out var month)) { Console.WriteLine("Invalid month."); return; }

    var summary = summaryService.Build(year, month, tracker.GetTransactions(), tracker.GetBudgets());
    Console.WriteLine();
    Console.Write(summary);
}

static void ListBudgets(FinanceTracker tracker)
{
    var budgets = tracker.GetBudgets();
    if (budgets.Count == 0) { Console.WriteLine("No budgets set."); return; }
    foreach (var b in budgets)
        Console.WriteLine($"{b.Category,-14} {b.MonthlyLimit,12:C}");
}