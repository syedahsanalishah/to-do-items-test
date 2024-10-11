using to_do_items_test.Models;

namespace to_do_items_test.Services
{
    public interface ITodoService
    {
        /// <summary>
        /// Asynchronously creates a new to-do item.
        /// </summary>
        /// <param name="item">The to-do item to be added.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task<TodoItem> CreateAsync(TodoItem item);

        /// <summary>
        /// Asynchronously retrieves all the to-do items from the list, with optional filtering and sorting.
        /// </summary>
        /// <param name="dueDate">Optional filter for items with a specific due date.</param>
        /// <param name="isCompleted">Optional filter for completed items.</param>
        /// <param name="sortBy">Optional sorting parameter (e.g., "duedate" or "title").</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task<IEnumerable<TodoItem>> GetAllAsync(DateTime? dueDate = null, bool? isCompleted = null, string? sortBy = null);

        /// <summary>
        /// Asynchronously retrieves a specific to-do item by its unique identifier (ID).
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task<TodoItem> GetByIdAsync(Guid id);

        /// <summary>
        /// Asynchronously updates an existing to-do item.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be updated.</param>
        /// <param name="item">The updated to-do item.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task<TodoItem> UpdateAsync(Guid id, TodoItem item);

        /// <summary>
        /// Marks a specific to-do item as completed.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be marked as completed.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task MarkAsCompletedAsync(Guid id);

        /// <summary>
        /// Asynchronously deletes a to-do item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be deleted.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task DeleteAsync(Guid id);
    }
}
