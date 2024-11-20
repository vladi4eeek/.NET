using System.Globalization;
using Vlad.TaskPlanner.DataAccess;
using Vlad.TaskPlanner.Domain.Logic;
using Vlad.TaskPlanner.Domain.Models;
using Vlad.TaskPlanner.Domain.Models.Enums;

internal static class Program
{
    public static void Main(string[] args)
    {
        FileWorkItemsRepository repository = new FileWorkItemsRepository();
        SimpleTaskPlanner planner = new SimpleTaskPlanner(repository);

        while (true)
        {
            Console.WriteLine("Choose an operation: [A]dd, [B]uild a plan, [M]ark as completed, [R]emove, [Q]uit");
            string operation = Console.ReadLine().ToUpper();

            switch (operation)
            {
                case "A":
                    AddWorkItem(repository);
                    break;
                case "B":
                    BuildPlan(planner);
                    break;
                case "M":
                    MarkAsCompleted(repository);
                    break;
                case "R":
                    RemoveWorkItem(repository);
                    break;
                case "Q":
                    repository.SaveChanges();
                    return;
                default:
                    Console.WriteLine("Invalid operation. Please try again.");
                    break;
            }
        }
    }

    private static void AddWorkItem(FileWorkItemsRepository repository)
    {
        string title = PromptForInput("Title: ");
        string description = PromptForInput("Description: ");
        DateTime creationDate = PromptForDate("Creation Date (dd.MM.yyyy): ");
        DateTime dueDate = PromptForDate("Due Date (dd.MM.yyyy): ");
        Priority priority = PromptForEnum<Priority>("Priority (Low, Medium, High): ");
        Complexity complexity = PromptForEnum<Complexity>("Complexity (None, Minutes, Hours, Days, Weeks): ");

        WorkItem workItem = new WorkItem
        {
            Title = title,
            Description = description,
            CreationDate = creationDate,
            DueDate = dueDate,
            Priority = priority,
            Complexity = complexity,
            IsCompleted = false
        };

        repository.Add(workItem);
        repository.SaveChanges();
        Console.WriteLine("Work item added successfully.");
    }

    private static string PromptForInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private static DateTime PromptForDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            Console.WriteLine("Invalid date format. Please try again.");
        }
    }

    private static TEnum PromptForEnum<TEnum>(string prompt) where TEnum : struct
    {
        while (true)
        {
            Console.Write(prompt);
            if (Enum.TryParse(Console.ReadLine(), true, out TEnum result) && Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }
            Console.WriteLine($"Invalid {typeof(TEnum).Name}. Please try again.");
        }
    }

    private static void BuildPlan(SimpleTaskPlanner planner)
    {
        WorkItem[] sortedWorkItems = planner.CreatePlan();

        Console.WriteLine("Sorted WorkItems:");
        foreach (var item in sortedWorkItems)
        {
            Console.WriteLine(item);
        }
    }

    private static void MarkAsCompleted(FileWorkItemsRepository repository)
    {
        Console.Write("Enter the ID of the work item to mark as completed: ");
        Guid id = Guid.Parse(Console.ReadLine());
    
        WorkItem workItem = repository.Get(id);
        if (workItem != null)
        {
            workItem.IsCompleted = true;
            repository.Update(workItem);
            repository.SaveChanges();
            Console.WriteLine("Work item marked as completed.");
        }
        else
        {
            Console.WriteLine("Work item not found.");
        }
    }
    
    private static void RemoveWorkItem(FileWorkItemsRepository repository)
    {
        Console.Write("Enter the ID of the work item to remove: ");
        Guid id = Guid.Parse(Console.ReadLine());
    
        if (repository.Remove(id))
        {
            repository.SaveChanges();
            Console.WriteLine("Work item removed successfully.");
        }
        else
        {
            Console.WriteLine("Work item not found.");
        }
    }
}