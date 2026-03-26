/*
 * INTERFACE SEGREGATION PRINCIPLE (ISP)
 * ======================================
 * No client should be forced to depend on methods it does not use.
 * Many specific interfaces are better than one general-purpose interface.
 * 
 * YOUR TASK:
 * Create a proper interface design for different types of workers.
 * Not all workers eat, and not all workers work!
 */

namespace PracticeExamples.SOLIDPrinciples;

// TODO: Create an IWorkable interface
// Method: Work()
interface IWorkable
{
    void Work();
}



// TODO: Create an IEatable interface
// Method: Eat()



// TODO: Create an ISleepable interface
// Method: Sleep()



// TODO: Create a HumanWorker class
// Implements: IWorkable, IEatable, ISleepable
// Implement all three methods with appropriate messages



// TODO: Create a RobotWorker class
// Implements: IWorkable only (robots don't eat or sleep!)
// Implement Work() method



// TODO: Create a Manager class
// Manages workers and delegates tasks
// Method: ManageWorker(IWorkable worker) - calls worker.Work()
// Method: BreakTime(IEatable worker) - calls worker.Eat()



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestInterfaceSegregation()
    {
        var human = new HumanWorker("John");
        var robot = new RobotWorker("R2D2");
        
        var manager = new Manager();
        
        // Both can work
        manager.ManageWorker(human);
        manager.ManageWorker(robot);
        
        // Only human can eat
        manager.BreakTime(human);
        // manager.BreakTime(robot); // This won't compile - robots don't eat!
        
        // Humans need sleep
        if (human is ISleepable sleepable)
        {
            sleepable.Sleep();
        }
    }
}
*/
