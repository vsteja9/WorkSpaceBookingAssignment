/*
 * SINGLE RESPONSIBILITY PRINCIPLE (SRP)
 * =====================================
 * A class should have only ONE reason to change.
 * Each class should have only ONE responsibility/job.
 * 
 * BAD EXAMPLE: A class that handles both user data AND email sending
 * GOOD EXAMPLE: Separate classes for User management and Email service
 * 
 * YOUR TASK:
 * Create a proper separation of concerns for an Invoice system.
 * The Invoice class should only handle invoice data.
 * Create separate classes for saving and printing invoices.
 */

namespace PracticeExamples.SOLIDPrinciples;

public class Invoice
{
    public Invoice()
    {
        id = new Guid();
    }
    public Guid id { get; private set; }
    public string? CustomerName { get; set; }
    public double? Amount { get; set; }
    public DateTime? Date { get; set; }
}

// TODO: Create an Invoice class with properties:
// - Id (int)
// - CustomerName (string)
// - Amount (decimal)
// - Date (DateTime)




// TODO: Create an InvoicePrinter class that handles printing
// It should have a method: Print(Invoice invoice)
// Hint: Just Console.WriteLine the invoice details

public class InvoicePrinter
{
    public void Print(Invoice invoice)
    {
        Console.WriteLine($"id:{invoice.id}\nname:{invoice.CustomerName}\namount:{invoice.Amount}");
    }
}


// TODO: Create an InvoiceRepository class that handles saving
// It should have a method: Save(Invoice invoice)
// Hint: Just simulate saving with Console.WriteLine

public class InvoiceRepository
{
    public void Save(Invoice invoice)
    {
        Console.WriteLine($"the Invoice is Saved and its ID is:{invoice.id}");
    }
}

public class Program1
{
    public void TestSingleResponsibility()
    {
        Invoice invoice = new Invoice()
        {
            CustomerName = "sai",
            Amount = 100,
            Date = new DateTime()
        };
        var printer = new InvoicePrinter();
        printer.Print(invoice);
        var repository = new InvoiceRepository();
        repository.Save(invoice);

    }
}


// USAGE EXAMPLE - Uncomment and test after implementing:
/*
public class Program
{
    public static void TestSingleResponsibility()
    {
        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "John Doe",
            Amount = 250.50m,
            Date = DateTime.Now
        };

        var printer = new InvoicePrinter();
        printer.Print(invoice);

        var repository = new InvoiceRepository();
        repository.Save(invoice);
    }
}
*/
