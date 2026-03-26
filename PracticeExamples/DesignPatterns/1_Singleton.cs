/*
 * SINGLETON PATTERN
 * =================
 * Ensures a class has only ONE instance and provides a global point of access to it.
 * 
 * WHEN TO USE:
 * - Logger classes
 * - Configuration managers
 * - Database connections
 * - Cache managers
 * 
 * YOUR TASK:
 * Create a thread-safe Singleton Logger class.
 */

namespace PracticeExamples.DesignPatterns;

// TODO: Create a Logger class (Singleton)
// 1. Private static instance field
// 2. Private constructor (prevents external instantiation)
// 3. Public static method GetInstance() that returns the single instance
// 4. Add a Log(string message) method that prints: "[LOG] message"
// 
// HINT: For thread safety, use 'lock' or make it simple with static initialization



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestSingleton()
    {
        // Get the logger instance
        var logger1 = Logger.GetInstance();
        var logger2 = Logger.GetInstance();

        // Both should be the same instance
        Console.WriteLine($"Are they the same instance? {ReferenceEquals(logger1, logger2)}");

        // Use the logger
        logger1.Log("Application started");
        logger2.Log("Processing data");
        logger1.Log("Application finished");
    }
}
*/
