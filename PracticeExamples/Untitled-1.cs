// encapsulation is the process of data hiding in the form of class 
// which consists of state and behaviour (data and method) and 
// restricting direct access to the internal properties.
// Encapsulation--> hides what you store and exposes what you want to access
// oops are useful to model the real world entities in the form of object and

public class BankAccount {
    public decimal Balance;
    public decimal Owner;
}

public class EncapsulatedBankAccount {
    private decimal _balance;
    private readonly string _owner;
    private List<string> _transactions = new();

    public EncapsulatedBankAccount(decimal balance, string owner) {
        if (string.IsNullOrEmpty(owner))
            throw new ArgumentException("Empty Input");
        if (string.IsNullOrEmpty(balance))
            throw new ArgumentException("exception");
    }
}
