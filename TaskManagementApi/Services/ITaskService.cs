using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, TaskItem taskItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MarkAsCompletedAsync(int id, CancellationToken cancellationToken = default);
}
