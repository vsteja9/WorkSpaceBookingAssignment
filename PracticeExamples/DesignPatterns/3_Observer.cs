/*
 * OBSERVER PATTERN
 * ================
 * Defines a one-to-many dependency between objects.
 * When one object changes state, all its dependents are notified automatically.
 * 
 * WHEN TO USE:
 * - Event handling systems
 * - Model-View updates
 * - Notification systems
 * 
 * YOUR TASK:
 * Create a weather station that notifies multiple displays when temperature changes.
 */

namespace PracticeExamples.DesignPatterns;

// TODO: Create an IObserver interface
// Method: Update(float temperature)



// TODO: Create an ISubject interface
// Method: Attach(IObserver observer)
// Method: Detach(IObserver observer)
// Method: Notify()



// TODO: Create a WeatherStation class that implements ISubject
// Field: private List<IObserver> observers
// Field: private float temperature
// Property: Temperature (when set, call Notify())
// Implement Attach, Detach, and Notify methods



// TODO: Create a PhoneDisplay class that implements IObserver
// Field: private string name
// Constructor: Accept name as parameter
// Implement Update() to display: "[name] shows: [temperature]°C"



// TODO: Create a TVDisplay class that implements IObserver
// Field: private string name
// Constructor: Accept name as parameter
// Implement Update() to display: "[name] displays: Temperature is [temperature]°C"



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestObserver()
    {
        var weatherStation = new WeatherStation();

        var phoneDisplay = new PhoneDisplay("My Phone");
        var tvDisplay = new TVDisplay("Living Room TV");

        // Attach observers
        weatherStation.Attach(phoneDisplay);
        weatherStation.Attach(tvDisplay);

        // Change temperature - all observers get notified
        Console.WriteLine("Setting temperature to 25°C:");
        weatherStation.Temperature = 25.0f;

        Console.WriteLine("\nSetting temperature to 30°C:");
        weatherStation.Temperature = 30.0f;

        // Detach one observer
        weatherStation.Detach(phoneDisplay);

        Console.WriteLine("\nSetting temperature to 28°C (phone detached):");
        weatherStation.Temperature = 28.0f;
    }
}
*/
