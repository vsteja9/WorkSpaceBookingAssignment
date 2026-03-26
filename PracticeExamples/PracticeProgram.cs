/*
 * PRACTICE TESTING PROGRAM
 * ========================
 * Use this file to test your implementations.
 * Uncomment the test methods as you complete each exercise.
 */


using Microsoft.VisualBasic;

using PracticeExamples.SOLIDPrinciples;
using PracticeExamples.DesignPatterns;

namespace PracticeExamples;

public class PracticeProgram
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("SOLID & Design Patterns Practice");
        Console.WriteLine("=================================\n");

        // SOLID PRINCIPLES TESTS
        // Uncomment each test method after implementing the corresponding file

        Test1_SingleResponsibility();
        // Test2_OpenClosed();
        // Test3_LiskovSubstitution();
        // Test4_InterfaceSegregation();
        // Test5_DependencyInversion();

        // DESIGN PATTERNS TESTS
        // Uncomment each test method after implementing the corresponding file

        // Test6_Singleton();
        // Test7_FactoryMethod();
        // Test8_Observer();
        // Test9_Strategy();
        // Test10_Decorator();

        Console.WriteLine("\n=================================");
        Console.WriteLine("All tests completed!");
        Console.WriteLine("=================================");
    }

    // ==================== SOLID PRINCIPLES ====================

    static void Test1_SingleResponsibility()
    {
        Console.WriteLine("\n--- Test 1: Single Responsibility Principle ---");
        
        // Option: Call the Program class method from 1_SingleResponsibility.cs
        var program = new PracticeExamples.SOLIDPrinciples.Program();
        program.TestSingleResponsibility();
    }

    static void Test2_OpenClosed()
    {
        Console.WriteLine("\n--- Test 2: Open/Closed Principle ---");
        // TODO: Copy the usage example from 2_OpenClosed.cs here
    }

    static void Test3_LiskovSubstitution()
    {
        Console.WriteLine("\n--- Test 3: Liskov Substitution Principle ---");
        // TODO: Copy the usage example from 3_LiskovSubstitution.cs here
    }

    static void Test4_InterfaceSegregation()
    {
        Console.WriteLine("\n--- Test 4: Interface Segregation Principle ---");
        // TODO: Copy the usage example from 4_InterfaceSegregation.cs here
    }

    static void Test5_DependencyInversion()
    {
        Console.WriteLine("\n--- Test 5: Dependency Inversion Principle ---");
        // TODO: Copy the usage example from 5_DependencyInversion.cs here
    }

    // ==================== DESIGN PATTERNS ====================

    static void Test6_Singleton()
    {
        Console.WriteLine("\n--- Test 6: Singleton Pattern ---");
        // TODO: Copy the usage example from 1_Singleton.cs here
    }

    static void Test7_FactoryMethod()
    {
        Console.WriteLine("\n--- Test 7: Factory Method Pattern ---");
        // TODO: Copy the usage example from 2_FactoryMethod.cs here
    }

    static void Test8_Observer()
    {
        Console.WriteLine("\n--- Test 8: Observer Pattern ---");
        // TODO: Copy the usage example from 3_Observer.cs here
    }

    static void Test9_Strategy()
    {
        Console.WriteLine("\n--- Test 9: Strategy Pattern ---");
        // TODO: Copy the usage example from 4_Strategy.cs here
    }

    static void Test10_Decorator()
    {
        Console.WriteLine("\n--- Test 10: Decorator Pattern ---");
        // TODO: Copy the usage example from 5_Decorator.cs here
    }
}
