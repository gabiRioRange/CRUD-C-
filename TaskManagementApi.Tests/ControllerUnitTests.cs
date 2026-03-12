using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementApi.Controllers;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Tests;

public class TasksControllerUnitTests
{
    private Mock<ITaskService> CreateMockService()
    {
        return new Mock<ITaskService>();
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Empty((IEnumerable<TaskItem>)okResult.Value!);
        mockService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithTasks_WhenTasksExist()
    {
        // Arrange
        var mockService = CreateMockService();
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Titulo = "Task 1", Concluida = false },
            new TaskItem { Id = 2, Titulo = "Task 2", Concluida = true }
        };

        mockService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsType<List<TaskItem>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count);
        Assert.Equal("Task 1", returnedTasks[0].Titulo);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        var mockService = CreateMockService();
        var task = new TaskItem { Id = 1, Titulo = "Existing Task", Concluida = false };

        mockService
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(task, okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.GetById(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenValidTaskProvided()
    {
        // Arrange
        var mockService = CreateMockService();
        var newTask = new TaskItem { Id = 0, Titulo = "New Task", Descricao = "Description" };
        var createdTask = new TaskItem { Id = 1, Titulo = "New Task", Descricao = "Description", Concluida = false };

        mockService
            .Setup(s => s.CreateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Create(newTask, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TasksController.GetById), createdResult.ActionName);
        Assert.Equal(1, ((TaskItem)createdResult.Value!).Id);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenTaskExists()
    {
        // Arrange
        var mockService = CreateMockService();
        var updateTask = new TaskItem { Titulo = "Updated Task", Descricao = "Updated Description" };

        mockService
            .Setup(s => s.UpdateAsync(1, It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateTask, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.UpdateAsync(1, It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var mockService = CreateMockService();
        var updateTask = new TaskItem { Titulo = "Updated Task" };

        mockService
            .Setup(s => s.UpdateAsync(999, It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Update(999, updateTask, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTaskExists()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Delete(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Complete Tests

    [Fact]
    public async Task Complete_ShouldReturnNoContent_WhenTaskExists()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.MarkAsCompletedAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Complete(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Complete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var mockService = CreateMockService();
        mockService
            .Setup(s => s.MarkAsCompletedAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var controller = new TasksController(mockService.Object);

        // Act
        var result = await controller.Complete(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
