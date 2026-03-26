/*
 * LISKOV SUBSTITUTION PRINCIPLE (LSP)
 * ====================================
 * Objects of a superclass should be replaceable with objects of its subclasses
 * without breaking the application.
 * 
 * In simple terms: Derived classes must be substitutable for their base classes.
 * 
 * YOUR TASK:
 * Create a proper hierarchy for different types of birds.
 * Not all birds can fly, so design it correctly!
 */

namespace PracticeExamples.SOLIDPrinciples;

// TODO: Create an abstract Bird class
// Properties: Name (string)
// Method: MakeSound() - abstract
public abstract class Bird
{
    public string Name { get; set; }
    public abstract string MakeSound();
}



// TODO: Create an interface IFlyable
// Method: Fly()
public interface IFlyable
{
    string Fly();
}



// TODO: Create a Sparrow class that inherits Bird and implements IFlyable
// Override MakeSound() to return "Chirp chirp!"
// Implement Fly() to return "Sparrow is flying!"
public class Sparrow : Bird, IFlyable
{
    public string Fly()
    {
        return "Sparrow is flying";
    }

    public override string MakeSound()
    {
        return "Chirp chirp!";
    }

}



// TODO: Create a Penguin class that inherits Bird (but does NOT implement IFlyable)
// Penguins can't fly!
// Override MakeSound() to return "Squawk!"
// Add a method Swim() that returns "Penguin is swimming!"
public class Penguin : Bird
{
    public override string MakeSound()
    {
        return "Squawk!";

    }
    public string Swim()
    {
        return "Penguin is swimming!";
    }

}



// TODO: Create an Eagle class that inherits Bird and implements IFlyable
// Override MakeSound() to return "Screech!"
// Implement Fly() to return "Eagle is soaring high!"
public class Eagle : Bird, IFlyable
{
    public override string MakeSound()
    {
        return "Screech!";
    }
    public string Fly()
    {
        return "Eagle is soaring high!";

    }

}


// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestLiskovSubstitution()
    {
        List<Bird> birds = new List<Bird>
        {
            new Sparrow { Name = "Jack" },
            new Penguin { Name = "Pingu" },
            new Eagle { Name = "Freedom" }
        };

        foreach (var bird in birds)
        {
            Console.WriteLine($"{bird.Name} says: {bird.MakeSound()}");
            
            if (bird is IFlyable flyable)
            {
                Console.WriteLine(flyable.Fly());
            }
            
            if (bird is Penguin penguin)
            {
                Console.WriteLine(penguin.Swim());
            }
        }
    }
}
*/
