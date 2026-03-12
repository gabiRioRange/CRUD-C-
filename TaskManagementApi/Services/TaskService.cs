using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public class TaskService(AppDbContext context) : ITaskService
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Tasks
            .AsNoTracking() // AsNoTracking reduz custo em leitura pura.
            .OrderBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        context.Tasks.Add(taskItem);
        await context.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task<bool> UpdateAsync(int id, TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        var existingTask = await context.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
        if (existingTask is null)
        {
            return false;
        }

        existingTask.Titulo = taskItem.Titulo;
        existingTask.Descricao = taskItem.Descricao;
        existingTask.Concluida = taskItem.Concluida;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingTask = await context.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
        if (existingTask is null)
        {
            return false;
        }

        context.Tasks.Remove(existingTask);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> MarkAsCompletedAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingTask = await context.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
        if (existingTask is null)
        {
            return false;
        }

        if (!existingTask.Concluida)
        {
            existingTask.Concluida = true;
            await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
