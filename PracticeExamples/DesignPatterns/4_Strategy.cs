/*
 * STRATEGY PATTERN
 * ================
 * Defines a family of algorithms, encapsulates each one,
 * and makes them interchangeable at runtime.
 * 
 * WHEN TO USE:
 * - Multiple algorithms for a specific task
 * - Need to switch algorithms at runtime
 * - Want to avoid conditional statements
 * 
 * YOUR TASK:
 * Create a payment system that can use different payment strategies.
 */

namespace PracticeExamples.DesignPatterns;

// TODO: Create an IPaymentStrategy interface
// Method: Pay(decimal amount)



// TODO: Create a CreditCardPayment class that implements IPaymentStrategy
// Field: private string cardNumber
// Constructor: Accept cardNumber as parameter
// Implement Pay() to display: "Paid $[amount] using Credit Card ending in [last 4 digits]"



// TODO: Create a PayPalPayment class that implements IPaymentStrategy
// Field: private string email
// Constructor: Accept email as parameter
// Implement Pay() to display: "Paid $[amount] using PayPal account [email]"



// TODO: Create a CryptoPayment class that implements IPaymentStrategy
// Field: private string walletAddress
// Constructor: Accept walletAddress as parameter
// Implement Pay() to display: "Paid $[amount] using Crypto wallet [wallet]"



// TODO: Create a ShoppingCart class
// Field: private IPaymentStrategy paymentStrategy
// Field: private decimal totalAmount
// Method: SetPaymentStrategy(IPaymentStrategy strategy)
// Method: SetTotalAmount(decimal amount)
// Method: Checkout() - calls paymentStrategy.Pay(totalAmount)



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestStrategy()
    {
        var cart = new ShoppingCart();
        cart.SetTotalAmount(150.00m);

        // Pay with credit card
        Console.WriteLine("Paying with Credit Card:");
        cart.SetPaymentStrategy(new CreditCardPayment("1234-5678-9012-3456"));
        cart.Checkout();

        // Pay with PayPal
        Console.WriteLine("\nPaying with PayPal:");
        cart.SetPaymentStrategy(new PayPalPayment("user@example.com"));
        cart.Checkout();

        // Pay with Crypto
        Console.WriteLine("\nPaying with Crypto:");
        cart.SetPaymentStrategy(new CryptoPayment("0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb"));
        cart.Checkout();
    }
}
*/
