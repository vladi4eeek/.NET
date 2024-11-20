using Newtonsoft.Json;
using Vlad.TaskPlanner.DataAccess.Abstractions;
using Vlad.TaskPlanner.Domain.Models;

namespace Vlad.TaskPlanner.DataAccess;

public class FileWorkItemsRepository : IWorkItemsRepository
{
    public WorkItem Get(Guid id)
    {
        if (workItemsDictionary.TryGetValue(id, out var workItem))
        {
            return workItem;
        }
        throw new KeyNotFoundException($"WorkItem with ID {id} not found.");
    }

    public WorkItem[] GetAll()
    {
        return workItemsDictionary.Values.ToArray();
    }

    public bool Update(WorkItem workItem)
    {
        if (workItemsDictionary.ContainsKey(workItem.Id))
        {
            workItemsDictionary[workItem.Id] = workItem;
            return true;
        }
        return false;
    }

    public bool Remove(Guid id)
    {
        return workItemsDictionary.Remove(id);
    }

    private const string FileName = "work-items.json"; // Назва файлу
    private readonly Dictionary<Guid, WorkItem> workItemsDictionary; // Словник для зберігання об'єктів WorkItem

    public FileWorkItemsRepository()
    {
        workItemsDictionary = new Dictionary<Guid, WorkItem>();

        try
        {
            if (File.Exists(FileName))
            {
                string fileContent = File.ReadAllText(FileName);

                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    WorkItem[] workItemsArray = JsonConvert.DeserializeObject<WorkItem[]>(fileContent) ?? Array.Empty<WorkItem>();

                    foreach (var item in workItemsArray)
                    {
                        workItemsDictionary[item.Id] = item;
                    }
                }
                else
                {
                    Console.WriteLine($"File {FileName} is empty. Created new file.");
                }
            }
            else
            {
                Console.WriteLine($"File {FileName} not found. Created new file.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }
    }
    
    public Guid Add(WorkItem workItem)
    {
        // Create a copy of the workItem
        WorkItem workItemCopy = workItem.Clone();

        // Generate a new Guid and assign it to the copy
        Guid newId = Guid.NewGuid();
        workItemCopy.Id = newId;

        // Add the copy to the dictionary
        workItemsDictionary[newId] = workItemCopy;

        // Return the generated ID
        return newId;
    }
    
    public void SaveChanges()
    {
        // Convert the dictionary values to an array
        WorkItem[] workItemsArray = workItemsDictionary.Values.ToArray();

        // Serialize the array to JSON
        string json = JsonConvert.SerializeObject(workItemsArray, Formatting.Indented);

        // Write the JSON to the file, overwriting the existing content
        File.WriteAllText(FileName, json);
    }
    
    // Метод для отримання словника WorkItem
    public Dictionary<Guid, WorkItem> GetWorkItemsDictionary()
    {
        return workItemsDictionary;
    }
}
