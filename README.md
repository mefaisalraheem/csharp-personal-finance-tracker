🚀 How to Run
Prerequisites
.NET 8 SDK (or later)

Build & Run
bash
git clone <https://github.com/mefaisalraheem/csharp-personal-finance-tracker.git>
cd csharp-personal-finance-tracker
dotnet build
dotnet run --project src/PersonalFinanceTracker

Run Tests

bash
dotnet test


The app stores data in finance-data.json in the working directory.

🎯 Features

Feature	Description
Transactions	Add income/expense with category, amount, date, description
Budgets	Set monthly budgets per category; automatic overspending detection
Recurring Payments	Define monthly recurring income/expenses; auto-materialize on run
Monthly Summary	Income, expenses, net savings, savings rate, budget status
LINQ	All reporting uses LINQ queries over in-memory data
Decimal	All money uses decimal (never double)
Persistence	JSON file via System.Text.Json
Tests	xUnit unit tests for core logic

🧾 Example Output

=== Personal Finance Tracker ===
1) Add income
2) Add expense
3) Set budget
4) Add recurring payment
5) Process recurring payments
6) Show transactions
7) Show monthly summary
8) List budgets
9) Save & exit

> 2
Category: Groceries
Amount: 45.50
Description: Weekly shop
Date (yyyy-MM-dd, blank=today): 2025-01-15
Expense added.

=== Monthly Summary (2025-01) ===
Income:      $3,200.00
Expenses:    $  745.50
Net Savings: $2,454.50
Savings Rate: 76.70%

Budget Status:

  Groceries  $45.50 / $400.00  [OK]
  Dining     $120.00 / $100.00 [OVER by $20.00]