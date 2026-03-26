# 🎯 Practice Guide: SOLID Principles & Design Patterns

Welcome! This guide will help you practice coding SOLID principles and design patterns step by step.

## 📋 Getting Started

### Order of Practice (Recommended)

1. **SOLID Principles First** (Build strong fundamentals)
   - Single Responsibility Principle
   - Open/Closed Principle
   - Liskov Substitution Principle
   - Interface Segregation Principle
   - Dependency Inversion Principle

2. **Design Patterns Second** (Apply patterns to solve problems)
   - Singleton Pattern
   - Factory Method Pattern
   - Observer Pattern
   - Strategy Pattern
   - Decorator Pattern

## 🚀 How to Practice

### For Each File:

1. **Open the file** in your editor
2. **Read the explanation** at the top
3. **Find the TODO comments** - these mark where you need to write code
4. **Write the code** following the hints
5. **Uncomment the usage example** at the bottom
6. **Test your code** using the Program.cs file

### Example Workflow:

```
1. Open: 1_SingleResponsibility.cs
2. Read the concept explanation
3. Implement the Invoice class (properties: Id, CustomerName, Amount, Date)
4. Implement the InvoicePrinter class
5. Implement the InvoiceRepository class
6. Uncomment the test code
7. Run Program.cs to test
```

## 🎓 Learning Tips

### While Coding:

- **Don't rush** - understand WHY each principle/pattern is used
- **Type everything** - don't copy/paste, muscle memory helps!
- **Experiment** - try breaking the rules to see what happens
- **Ask yourself**: "What problem does this solve?"

### After Each Implementation:

- ✅ Does my code follow the principle/pattern?
- ✅ Can I explain it to someone else?
- ✅ What real-world scenario would use this?

## 📊 Progress Tracker

### SOLID Principles

- [ ] Single Responsibility - Invoice system
- [ ] Open/Closed - Discount calculator
- [ ] Liskov Substitution - Bird hierarchy
- [ ] Interface Segregation - Workers
- [ ] Dependency Inversion - Notification system

### Design Patterns

- [ ] Singleton - Logger
- [ ] Factory Method - Vehicle factory
- [ ] Observer - Weather station
- [ ] Strategy - Payment system
- [ ] Decorator - Coffee ordering

## 🧪 Testing Your Code

Use the `PracticeProgram.cs` file to test each implementation:

```csharp
// Uncomment the test method for what you just implemented
TestSingleResponsibility();
TestOpenClosed();
// etc.
```

## 💡 Quick Reference

### SOLID Cheat Sheet

- **S**: One class, one job
- **O**: Extend without modifying
- **L**: Subclasses should work like parent
- **I**: Small, specific interfaces
- **D**: Depend on abstractions, not concrete classes

### Pattern Cheat Sheet

- **Singleton**: Only one instance
- **Factory**: Create objects without specifying exact class
- **Observer**: Notify many objects of changes
- **Strategy**: Swap algorithms at runtime
- **Decorator**: Add features dynamically

## 🎯 Challenge Yourself

After completing all examples:

1. **Combine patterns** - Use multiple patterns together
2. **Real project** - Apply to your actual code
3. **Code review** - Look at existing code and identify patterns
4. **Refactor** - Take bad code and apply SOLID principles

## 📚 Next Steps

Once comfortable with these:

- Learn more patterns (Adapter, Facade, Command, etc.)
- Study SOLID in real frameworks (ASP.NET Core uses DI heavily!)
- Practice identifying anti-patterns

---

**Remember**: The goal is understanding, not memorization. Take your time! 🌟
