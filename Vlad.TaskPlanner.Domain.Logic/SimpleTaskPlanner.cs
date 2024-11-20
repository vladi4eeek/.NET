using Vlad.TaskPlanner.DataAccess.Abstractions;
using Vlad.TaskPlanner.Domain.Models;

namespace Vlad.TaskPlanner.Domain.Logic;

public class SimpleTaskPlanner
{
    private readonly IWorkItemsRepository _repository;

    public SimpleTaskPlanner(IWorkItemsRepository repository)
    {
        _repository = repository;
    }

    public WorkItem[] CreatePlan()
    {
        // Fetch all work items from the repository
        var workItems = _repository.GetAll();

        // Filter out completed tasks
        var relevantWorkItems = workItems.Where(w => !w.IsCompleted);

        // Sort the work items based on priority and due date
        var sortedWorkItems = relevantWorkItems
            .OrderByDescending(w => w.Priority) // High priority first
            .ThenBy(w => w.DueDate) // Earliest due date first
            .ToArray();

        return sortedWorkItems;
    }
}