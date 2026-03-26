/*
 * OPEN/CLOSED PRINCIPLE (OCP)
 * ============================
 * Software entities should be OPEN for extension but CLOSED for modification.
 * You should be able to add new functionality without changing existing code.
 * 
 * KEY TECHNIQUE: Use abstraction (interfaces/abstract classes)
 * 
 * YOUR TASK:
 * Create a discount calculation system that can be extended with new discount types
 * without modifying existing code.
 */

namespace PracticeExamples.SOLIDPrinciples;

// TODO: Create an abstract class or interface IDiscount
// It should have a method: decimal Calculate(decimal amount)
public interface IDiscount
{
    public decimal Calculate(decimal amount);
}
//or 
public abstract class IDiscountAbstractClass
{
    public abstract decimal Calculate(decimal amount);

}


// TODO: Create a RegularDiscount class (10% off)
// Implement the Calculate method
public class RegularDiscount : IDiscount
{
    public decimal Calculate(decimal amount)
    {

        return 0.9m * amount;
    }
}



// TODO: Create a PremiumDiscount class (20% off)
// Implement the Calculate method
public class PremiumDiscount : IDiscount
{
    public decimal Calculate(decimal amount)
    {
        if (amount != null)
        {
            throw new NullReferenceException();
            // give me 5 to 10 errors which I need to throw which case.
        }
        return .8m * amount;
    }
}



// TODO: Create a SeasonalDiscount class (15% off)
// Implement the Calculate method
public class SeasonalDiscount : IDiscount
{
    public decimal Calculate(decimal amount)
    {
        if (amount != null) throw new NullReferenceException();
        return 0.85m * amount;

    }
}



// TODO: Create a DiscountCalculator class
// It should have a method: decimal ApplyDiscount(decimal amount, IDiscount discount)
// This class is CLOSED for modification but OPEN for extension
public class DiscountCalculator
{
    public decimal ApplyDiscount(decimal amount, IDiscount discount)
    {
        return discount.Calculate(amount);
    }
}

public class Program
{
    public static void TestOpenClosed()
    {
        var calculator = new DiscountCalculator();
        decimal originalPrice = 1000m;

        var regularDiscount = new RegularDiscount();
        Console.WriteLine($"Regular: ${calculator.ApplyDiscount(originalPrice, regularDiscount)}");

        var premiumDiscount = new PremiumDiscount();
        Console.WriteLine($"Premium: ${calculator.ApplyDiscount(originalPrice, premiumDiscount)}");

        var seasonalDiscount = new SeasonalDiscount();
        Console.WriteLine($"Seasonal: ${calculator.ApplyDiscount(originalPrice, seasonalDiscount)}");
    }
}


// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestOpenClosed()
    {
        var calculator = new DiscountCalculator();
        decimal originalPrice = 1000m;

        var regularDiscount = new RegularDiscount();
        Console.WriteLine($"Regular: ${calculator.ApplyDiscount(originalPrice, regularDiscount)}");

        var premiumDiscount = new PremiumDiscount();
        Console.WriteLine($"Premium: ${calculator.ApplyDiscount(originalPrice, premiumDiscount)}");

        var seasonalDiscount = new SeasonalDiscount();
        Console.WriteLine($"Seasonal: ${calculator.ApplyDiscount(originalPrice, seasonalDiscount)}");
    }
}
*/
