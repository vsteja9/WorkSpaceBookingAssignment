/*
 * DEPENDENCY INVERSION PRINCIPLE (DIP)
 * =====================================
 * High-level modules should not depend on low-level modules.
 * Both should depend on abstractions (interfaces).
 * 
 * Abstractions should not depend on details.
 * Details should depend on abstractions.
 * 
 * YOUR TASK:
 * Create a notification system where the NotificationService doesn't
 * depend on concrete implementations (Email, SMS, etc.)
 */

namespace PracticeExamples.SOLIDPrinciples;

// TODO: Create an IMessageSender interface
// Method: SendMessage(string recipient, string message)



// TODO: Create an EmailSender class that implements IMessageSender
// Simulate sending an email in SendMessage method



// TODO: Create an SmsSender class that implements IMessageSender
// Simulate sending an SMS in SendMessage method



// TODO: Create a PushNotificationSender class that implements IMessageSender
// Simulate sending a push notification in SendMessage method



// TODO: Create a NotificationService class
// Constructor: Accept IMessageSender as a parameter (dependency injection)
// Method: Notify(string recipient, string message)
// This method should use the injected IMessageSender



// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestDependencyInversion()
    {
        string recipient = "john@example.com";
        string message = "Hello, this is a test notification!";

        // We can easily switch between different senders
        var emailService = new NotificationService(new EmailSender());
        emailService.Notify(recipient, message);

        var smsService = new NotificationService(new SmsSender());
        smsService.Notify("+1234567890", message);

        var pushService = new NotificationService(new PushNotificationSender());
        pushService.Notify("user123", message);
        
        // The NotificationService doesn't need to change when we add new senders!
    }
}
*/
