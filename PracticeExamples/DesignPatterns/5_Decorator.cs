/*
 * DECORATOR PATTERN
 * =================
 * Attaches additional responsibilities to an object dynamically.
 * Provides a flexible alternative to subclassing for extending functionality.
 * 
 * WHEN TO USE:
 * - Add features to objects without changing their structure
 * - Combine features dynamically
 * - Alternative to creating many subclasses
 * 
 * YOUR TASK:
 * Create a coffee ordering system where you can add toppings dynamically.
 */

namespace PracticeExamples.DesignPatterns;

// TODO: Create an abstract Coffee class
// Method: abstract string GetDescription()
// Method: abstract decimal GetCost()



// TODO: Create a SimpleCoffee class that inherits from Coffee
// Override GetDescription() to return "Simple Coffee"
// Override GetCost() to return 5.00m



// TODO: Create an abstract CoffeeDecorator class that inherits from Coffee
// Field: protected Coffee coffee
// Constructor: Accept Coffee as parameter and assign to field



// TODO: Create a MilkDecorator class that inherits from CoffeeDecorator
// Constructor: Call base constructor with coffee parameter
// Override GetDescription() to return: coffee.GetDescription() + ", Milk"
// Override GetCost() to return: coffee.GetCost() + 1.50m



// TODO: Create a SugarDecorator class that inherits from CoffeeDecorator
// Constructor: Call base constructor with coffee parameter
// Override GetDescription() to return: coffee.GetDescription() + ", Sugar"
// Override GetCost() to return: coffee.GetCost() + 0.50m



// TODO: Create a WhipCreamDecorator class that inherits from CoffeeDecorator
// Constructor: Call base constructor with coffee parameter
// Override GetDescription() to return: coffee.GetDescription() + ", Whip Cream"
// Override GetCost() to return: coffee.GetCost() + 2.00m



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestDecorator()
    {
        // Simple coffee
        Coffee coffee1 = new SimpleCoffee();
        Console.WriteLine($"{coffee1.GetDescription()} - ${coffee1.GetCost()}");

        // Coffee with milk
        Coffee coffee2 = new SimpleCoffee();
        coffee2 = new MilkDecorator(coffee2);
        Console.WriteLine($"{coffee2.GetDescription()} - ${coffee2.GetCost()}");

        // Coffee with milk and sugar
        Coffee coffee3 = new SimpleCoffee();
        coffee3 = new MilkDecorator(coffee3);
        coffee3 = new SugarDecorator(coffee3);
        Console.WriteLine($"{coffee3.GetDescription()} - ${coffee3.GetCost()}");

        // Coffee with everything!
        Coffee coffee4 = new SimpleCoffee();
        coffee4 = new MilkDecorator(coffee4);
        coffee4 = new SugarDecorator(coffee4);
        coffee4 = new WhipCreamDecorator(coffee4);
        Console.WriteLine($"{coffee4.GetDescription()} - ${coffee4.GetCost()}");
    }
}
*/
