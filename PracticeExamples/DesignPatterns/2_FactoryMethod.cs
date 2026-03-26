/*
 * FACTORY METHOD PATTERN
 * ======================
 * Defines an interface for creating objects, but lets subclasses decide
 * which class to instantiate.
 * 
 * WHEN TO USE:
 * - When you don't know the exact type of object needed until runtime
 * - When you want to delegate the instantiation logic
 * 
 * YOUR TASK:
 * Create a vehicle factory that creates different types of vehicles.
 */

namespace PracticeExamples.DesignPatterns;

// TODO: Create an abstract Vehicle class
// Properties: Brand (string), Type (string)
// Method: GetDetails() that returns a string with vehicle info



// TODO: Create a Car class that inherits from Vehicle
// Set Type = "Car" in constructor
// Accept Brand as parameter



// TODO: Create a Motorcycle class that inherits from Vehicle
// Set Type = "Motorcycle" in constructor
// Accept Brand as parameter



// TODO: Create a Truck class that inherits from Vehicle
// Set Type = "Truck" in constructor
// Accept Brand as parameter



// TODO: Create a VehicleFactory class
// Static method: CreateVehicle(string type, string brand)
// Use a switch statement to return the appropriate vehicle
// Types: "car", "motorcycle", "truck"
// Return null or throw exception for unknown types



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestFactoryMethod()
    {
        var car = VehicleFactory.CreateVehicle("car", "Toyota");
        Console.WriteLine(car.GetDetails());

        var motorcycle = VehicleFactory.CreateVehicle("motorcycle", "Harley-Davidson");
        Console.WriteLine(motorcycle.GetDetails());

        var truck = VehicleFactory.CreateVehicle("truck", "Volvo");
        Console.WriteLine(truck.GetDetails());
    }
}
*/
