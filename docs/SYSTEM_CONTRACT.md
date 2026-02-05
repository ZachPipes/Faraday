# **System Contract**

## Core Objectives
1. **Store and Manipulate** Financial Records
2. **Track** Stock Actions
3. **Fast and Accurate** Lookups
4. **Zero Data Loss**

## MVP Requirements
1. Accounts
    - Account Graphs
2. Transactions
    - Able to handle Simmons, Commerce, and Fidelity account records
3. Document Storage
    - Stores PDFs of statements
    - Tagged by account, date, etc
    - Search/retrieve when needed
4. Calendar
    - Recurring transactions: Auto-create monthly bills
    - Send reminders 3 days before due date
5. Inbox
    - In app notification page
    - Types: Bill reminders, import errors
    - Mark as read
    - Desktop notification later

## Core Entities
### Essentials
1. **Account**
    - What: Where money is located
    - Examples: "Fidelity Checking", "Wallet/Cash", "Credit Card"
    - Key Attributes: Name, Type, Currency, Opening/Current Balance
2. **Transactions**
    - What: How/When money moves
    - Examples: "Deposit", "Bill", "Groceries"
    - Key Attributes: Description, Amount, Date, Category, Type (Income/Expense)
3. **Categories**
    - What: Method to organize transactions
    - Examples: "Groceries", "Utilities", "Investment"
4. **Stock Position**
    - What: Holdings in investment accounts
    - Examples "100 shares AAPL", "0.5 BTC"
    - Key Attributes: Symbol, Quantity, Purchase Price, Market Value, Cost Basis
    - Notes: Be able to combine all of the same symbols into one singular amount

### Definitions
1. **Account**
    - ID (UUID)
    - Name (string)
    - Type (enum: Checking, Savings, CreditCard, Investment)
    - Institution (enum: Fidelity, Simmons, Commerce, Cash)
    - Currency (enum: USD, CAN, GBP, EUR)
    - OpeningBalance (decimal)
    - IsActive (bool)
    - CreatedAt (Datetime)
    - ModifiedAt (Datetime)

2. **Transaction**
    - ID (UUID)
    - Date (Datetime)
    - Amount (decimal) - positive for income, negative for expense
    - Description (string)
    - Type (enum: Income, Expense, Transfer)
    - AccountID (UUID, Account_FK)
    - CategoryID (UUID, Categories_FK, nullable)
    - TransferAccountID (UUID, nullable)
    - IsVoid (bool, default false)
    - CreatedAt (Datetime)
    - ModifiedAt (Datetime)

3. **Category**
    - ID (UUID)
    - Name (string)

4. **StockPosition**
    - ID (UUID)
    - AccountID (UUID, Account_FK where Type = Investment)
    - Symbol (string)
    - Quantity (decimal)
    - PurchasePrice (decimal)
    - LastUpdated (Datetime)
    - CreatedAt (Datetime)
    - ModifiedAt (Datetime)

5. **Documents**
    - ID (UUID)
    - AccountID (UUID, Account_FK)
    - DocumentType (enum: Statement, TaxForm, Receipt, Other)
    - FilePath (string)
    - UploadDate (Datetime)
    - PeriodStart, PeriodEnd (Datetime, nullable)
    - CreatedAt (Datetime)
    - ModifiedAt (Datetime)

6. **ReccurringTransactions**
    - ID (UUID)
    - Frequency (enum: Daily, Weekly, Biweekly, Monthly, Yearly)
    - NextDueDate (Datetime)
    - ReminderDays (int, default 3)
    - IsActive (bool)
    - CreatedAt (Datetime)
    - ModifiedAt (Datetime)

7. **Notifications**
    - ID (UUID)
    - Type (enum: BillReminder, ImportError, LowBalance, SystemAlert)
    - Message (string)
    - RelatedEntityType (enum: Transaction, RecurringTransaction, Document)
    - RelatedEntityID (UUID, nullable)
    - ReadOn (Datetime, nullable)
    - CreatedAt (Datetime)

## Financial Integrity
1. **Immutable Transactions** - Use IsVoid flag for corrections, never delete
2. **Calculated Balances** - Account.CurrentBalance = SUM(transactions), never stored
3. **Decimal Precision** - All money: `decimal` in C# / `NUMERIC(18,2)` in PostgreSQL
4. **Transfer Pairing** - Transfers create two transactions that must balance

### Stock Tracking
5. **Position Aggregation** - Multiple purchases of same symbol → single position with avg price
6. **Cost Basis Formula** - `NewAvgPrice = (OldCostBasis + NewPurchaseAmount) / (OldQty + NewQty)`
7. **Account Type Validation** - Stock positions only allowed in Investment accounts

### Data Safety
8. **Audit Trail** - All entities have CreatedAt, ModifiedAt
9. **Soft Deletes** - Accounts and categories marked IsActive=false, not deleted
10. **Document Safety** - Deleting document removes DB record, but keeps file (manual cleanup)

---


## Technical Constraints

- **Database:** PostgreSQL (local instance)
- **ORM:** Entity Framework Core 8 with Npgsql
- **UI:** WPF with MVVM pattern
- **IDE:** JetBrains Rider
- **Testing:** xUnit, 70%+ domain coverage
- **Logging:** Serilog to local files

---