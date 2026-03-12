using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Tests;

public class TaskServiceTests
{
    private static AppDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAndGetById_ShouldReturnCreatedTask()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var created = await service.CreateAsync(new TaskItem
        {
            Titulo = "Balancear HP do Boss",
            Descricao = "Ajustar dano e vida",
            Concluida = false
        });

        var loaded = await service.GetByIdAsync(created.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Balancear HP do Boss", loaded!.Titulo);
        Assert.False(loaded.Concluida);
    }

    [Fact]
    public async Task MarkAsCompleted_ShouldSetConcluidaTrue()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var created = await service.CreateAsync(new TaskItem
        {
            Titulo = "Implementar loot da fase",
            Descricao = "Definir drop rate",
            Concluida = false
        });

        var result = await service.MarkAsCompletedAsync(created.Id);
        var loaded = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.NotNull(loaded);
        Assert.True(loaded!.Concluida);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrueAndPersistChanges_WhenTaskExists()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var created = await service.CreateAsync(new TaskItem
        {
            Titulo = "Ajustar IA dos inimigos",
            Descricao = "Melhorar pathfinding",
            Concluida = false
        });

        var result = await service.UpdateAsync(created.Id, new TaskItem
        {
            Titulo = "Ajustar IA dos inimigos (v2)",
            Descricao = "Pathfinding com menor custo",
            Concluida = true
        });

        var updated = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.NotNull(updated);
        Assert.Equal("Ajustar IA dos inimigos (v2)", updated!.Titulo);
        Assert.True(updated.Concluida);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var result = await service.UpdateAsync(9999, new TaskItem
        {
            Titulo = "Inexistente",
            Descricao = "Sem registro",
            Concluida = false
        });

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrueAndRemoveTask_WhenTaskExists()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var created = await service.CreateAsync(new TaskItem
        {
            Titulo = "Remover quest antiga",
            Descricao = "Quest duplicada",
            Concluida = false
        });

        var result = await service.DeleteAsync(created.Id);
        var loaded = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new TaskService(context);

        var result = await service.DeleteAsync(9999);

        Assert.False(result);
    }
}
