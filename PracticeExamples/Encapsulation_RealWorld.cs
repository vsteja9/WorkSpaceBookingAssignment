using System;
using System.Text.RegularExpressions;

namespace PracticeExamples.Encapsulation
{
    /// <summary>
    /// ENCAPSULATION - Real World Examples
    /// Encapsulation is about bundling data and methods together while controlling access to them
    /// </summary>
    /// 
    // ==========================================
    // EXAMPLE 1: Bank Account - Complete Encapsulation
    // ==========================================
    public class BankAccount
    {
        // PRIVATE - Only accessible within this class
        private decimal balance;
        private string accountNumber;
        private DateTime lastTransactionDate;

        // PROTECTED - Accessible in this class and derived classes
        protected string bankCode;

        // INTERNAL - Accessible within the same assembly/project
        internal int branchId;

        // PUBLIC - Accessible everywhere
        public string AccountHolderName { get; private set; }


        // Constructor
        public BankAccount(string accountNumber, string accountHolderName, string bankCode)
        {
            this.accountNumber = accountNumber;
            this.AccountHolderName = accountHolderName;
            this.bankCode = bankCode;
            this.balance = 0;
        }

        // Property with DATA VALIDATION in setter
        public decimal Balance
        {
            get { return balance; }
            private set
            {
                // Can't set negative balance directly
                if (value < 0)
                {
                    throw new InvalidOperationException("Balance cannot be negative");
                }
                balance = value;
            }
        }

        // PUBLIC method - Controlled access point for depositing
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive");
            }

            // HIDING IMPLEMENTATION - User doesn't know how balance is stored or calculated
            balance += amount;
            lastTransactionDate = DateTime.Now;
            LogTransaction("Deposit", amount);
        }

        // PUBLIC method - Controlled access point for withdrawal
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive");
            }

            if (amount > balance)
            {
                return false; // Insufficient funds
            }

            // HIDING IMPLEMENTATION - Internal validation and processing
            balance -= amount;
            lastTransactionDate = DateTime.Now;
            LogTransaction("Withdrawal", amount);
            return true;
        }

        // PRIVATE method - Implementation detail hidden from outside
        private void LogTransaction(string type, decimal amount)
        {
            // Internal logging mechanism - users don't need to know this exists
            Console.WriteLine($"[{DateTime.Now}] {type}: ${amount} - New Balance: ${balance}");
        }

        // PUBLIC read-only property - External users can read but not modify
        public string GetAccountInfo()
        {
            // HIDING IMPLEMENTATION - Account number format is internal detail
            return $"Account: {MaskAccountNumber(accountNumber)}, Holder: {AccountHolderName}";
        }

        // PRIVATE helper method - Implementation detail
        private string MaskAccountNumber(string accNum)
        {
            if (accNum.Length <= 4) return accNum;
            return "****" + accNum.Substring(accNum.Length - 4);
        }
    }

    // ==========================================
    // EXAMPLE 2: Premium Account - Demonstrates PROTECTED access
    // ==========================================
    public class PremiumBankAccount : BankAccount
    {
        private decimal creditLimit;

        public PremiumBankAccount(string accountNumber, string accountHolderName, string bankCode)
            : base(accountNumber, accountHolderName, bankCode)
        {
            creditLimit = 5000; // Premium accounts get credit
        }

        // Can access PROTECTED member from base class
        public string GetBankCode()
        {
            return bankCode; // This works because bankCode is protected
            // return accountNumber; // This would NOT work - accountNumber is private
        }

        // Can access INTERNAL member from base class (same assembly)
        public int GetBranchId()
        {
            return branchId; // This works because branchId is internal
        }
    }

    // ==========================================
    // EXAMPLE 3: Employee - Data Validation Example
    // ==========================================
    public class Employee
    {
        private string employeeId;
        private decimal salary;
        private int age;
        private string email;

        // Property with validation - Age must be between 18 and 70
        public int Age
        {
            get { return age; }
            set
            {
                if (value < 18 || value > 70)
                {
                    throw new ArgumentException("Age must be between 18 and 70");
                }
                age = value;
            }
        }

        // Property with validation - Salary must be positive
        public decimal Salary
        {
            get { return salary; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Salary cannot be negative");
                }
                if (value > 1000000)
                {
                    throw new ArgumentException("Salary exceeds maximum limit");
                }
                salary = value;
            }
        }

        // Property with email validation
        public string Email
        {
            get { return email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Email cannot be empty");
                }
                if (!IsValidEmail(value))
                {
                    throw new ArgumentException("Invalid email format");
                }
                email = value;
            }
        }

        // Read-only property - can only be set through constructor
        public string EmployeeId
        {
            get { return employeeId; }
        }

        // Auto-implemented property with private setter
        public string FullName { get; set; }
        public string Department { get; set; }

        // Constructor
        public Employee(string employeeId, string fullName, string department)
        {
            this.employeeId = employeeId; // Set once, never changes
            this.FullName = fullName;
            this.Department = department;
        }

        // PRIVATE validation method - Implementation hidden
        private bool IsValidEmail(string email)
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        // Public method that uses private data
        public void GiveRaise(decimal percentage)
        {
            if (percentage < 0 || percentage > 50)
            {
                throw new ArgumentException("Raise percentage must be between 0 and 50");
            }

            // HIDING IMPLEMENTATION - calculation details are internal
            decimal raiseAmount = salary * (percentage / 100);
            salary += raiseAmount;

            Console.WriteLine($"{FullName} received a {percentage}% raise of ${raiseAmount}");
        }

        // Computed property - no backing field
        public decimal AnnualSalary
        {
            get { return salary * 12; }
        }
    }

    // ==========================================
    // EXAMPLE 4: Shopping Cart - Hiding Complex Implementation
    // ==========================================
    public class ShoppingCart
    {
        // PRIVATE - Internal data structure hidden from users
        private List<CartItem> items;
        private Dictionary<string, decimal> discounts;
        private const decimal TAX_RATE = 0.08m; // Private constant

        public ShoppingCart()
        {
            items = new List<CartItem>();
            discounts = new Dictionary<string, decimal>();
        }

        // Nested PRIVATE class - Implementation detail
        private class CartItem
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        // PUBLIC interface - Simple, clean methods
        public void AddItem(string productId, string productName, decimal price, int quantity)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive");
            }

            // HIDING IMPLEMENTATION - User doesn't see how items are stored
            var existingItem = FindItem(productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity
                });
            }
        }

        public bool RemoveItem(string productId)
        {
            var item = FindItem(productId);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }

        // PRIVATE helper method - Implementation detail
        private CartItem FindItem(string productId)
        {
            return items.FirstOrDefault(i => i.ProductId == productId);
        }

        // PUBLIC property - Hides complex calculation
        public decimal SubTotal
        {
            get
            {
                // Complex logic hidden from outside
                return items.Sum(item => item.Price * item.Quantity);
            }
        }

        public decimal Tax
        {
            get
            {
                // Tax calculation logic hidden
                return CalculateTax();
            }
        }

        public decimal Total
        {
            get
            {
                // Total calculation hides all complexity
                return SubTotal - TotalDiscount + Tax;
            }
        }

        // PRIVATE - Complex discount calculation hidden
        private decimal TotalDiscount
        {
            get
            {
                return discounts.Values.Sum();
            }
        }

        // PRIVATE method - Implementation detail
        private decimal CalculateTax()
        {
            return SubTotal * TAX_RATE;
        }

        // PUBLIC method with simple interface
        public void ApplyDiscount(string code, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Discount amount cannot be negative");
            }

            // HIDING IMPLEMENTATION - Discount storage mechanism is private
            if (!discounts.ContainsKey(code))
            {
                discounts.Add(code, amount);
            }
        }

        // PUBLIC property - Users see count, not internal list
        public int ItemCount
        {
            get { return items.Count; }
        }
    }

    // ==========================================
    // EXAMPLE 5: Configuration Manager - INTERNAL access modifier
    // ==========================================
    internal class ConfigurationManager
    {
        // INTERNAL class - only accessible within the same assembly
        private static ConfigurationManager instance;
        private Dictionary<string, string> settings;

        // PRIVATE constructor - prevents external instantiation
        private ConfigurationManager()
        {
            settings = new Dictionary<string, string>();
            LoadDefaultSettings();
        }

        // INTERNAL static property - controlled access within assembly
        internal static ConfigurationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigurationManager();
                }
                return instance;
            }
        }

        // INTERNAL method
        internal string GetSetting(string key)
        {
            return settings.ContainsKey(key) ? settings[key] : null;
        }

        // PRIVATE method - implementation hidden
        private void LoadDefaultSettings()
        {
            settings.Add("AppName", "WorkSpaceBooking");
            settings.Add("Version", "1.0");
        }
    }

    // ==========================================
    // DEMONSTRATION CLASS
    // ==========================================
    public class EncapsulationDemo
    {
        public static void RunExamples()
        {
            Console.WriteLine("=== ENCAPSULATION REAL WORLD EXAMPLES ===\n");

            // Example 1: Bank Account
            Console.WriteLine("--- Bank Account Example ---");
            var account = new BankAccount("1234567890", "John Doe", "BANK001");
            account.Deposit(1000);
            account.Withdraw(200);
            Console.WriteLine(account.GetAccountInfo());
            Console.WriteLine($"Balance: ${account.Balance}\n");

            // Example 2: Employee with Validation
            Console.WriteLine("--- Employee Example ---");
            var employee = new Employee("EMP001", "Jane Smith", "IT");
            employee.Age = 30; // Valid
            employee.Salary = 75000; // Valid
            employee.Email = "jane.smith@company.com"; // Valid

            try
            {
                employee.Age = 15; // Invalid - will throw exception
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation Error: {ex.Message}");
            }

            employee.GiveRaise(10);
            Console.WriteLine($"Annual Salary: ${employee.AnnualSalary}\n");

            // Example 3: Shopping Cart
            Console.WriteLine("--- Shopping Cart Example ---");
            var cart = new ShoppingCart();
            cart.AddItem("P001", "Laptop", 999.99m, 1);
            cart.AddItem("P002", "Mouse", 29.99m, 2);
            cart.ApplyDiscount("SAVE10", 50);

            Console.WriteLine($"Items in cart: {cart.ItemCount}");
            Console.WriteLine($"SubTotal: ${cart.SubTotal:F2}");
            Console.WriteLine($"Tax: ${cart.Tax:F2}");
            Console.WriteLine($"Total: ${cart.Total:F2}\n");

            // Example 4: Internal Configuration (only works in same assembly)
            Console.WriteLine("--- Configuration Manager Example ---");
            var config = ConfigurationManager.Instance;
            Console.WriteLine($"App Name: {config.GetSetting("AppName")}");
            Console.WriteLine($"Version: {config.GetSetting("Version")}");
        }
    }
}

/*
 * KEY TAKEAWAYS - ENCAPSULATION:
 * 
 * 1. ACCESS MODIFIERS:
 *    - private: Only within the same class (most restrictive)
 *    - protected: Within class and derived classes
 *    - internal: Within the same assembly/project
 *    - public: Accessible everywhere (least restrictive)
 * 
 * 2. PROPERTIES (getters/setters):
 *    - Provide controlled access to private fields
 *    - Can have different access levels (public get, private set)
 *    - Can be auto-implemented or have backing fields
 * 
 * 3. DATA VALIDATION:
 *    - Validation logic in setters ensures data integrity
 *    - Prevents invalid states (negative age, invalid email, etc.)
 *    - Throws meaningful exceptions for invalid data
 * 
 * 4. HIDING IMPLEMENTATION:
 *    - Users interact with simple, clean public methods
 *    - Complex logic is hidden in private methods
 *    - Internal data structures are not exposed
 *    - Can change implementation without affecting users
 * 
 * BENEFITS:
 *    - Data Protection: Private fields can't be modified directly
 *    - Flexibility: Can change implementation without breaking external code
 *    - Maintainability: Logic is centralized in one place
 *    - Security: Sensitive data remains hidden
 *    - Control: All access goes through validated methods
 */
