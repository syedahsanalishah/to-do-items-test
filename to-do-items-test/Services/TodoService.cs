using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using to_do_items_test.Models;

namespace to_do_items_test.Services
{
    /// <summary>
    /// This class implements the ITodoService interface and provides 
    /// CRUD operations for managing a list of to-do items in memory.
    /// </summary>
    public class TodoService : ITodoService
    {
        // In-memory list to store to-do items
        private readonly List<TodoItem> _todoItems = new();

        /// <summary>
        /// Asynchronously creates a new to-do item, assigns it a unique identifier, and adds it to the list.
        /// </summary>
        /// <param name="item">The to-do item to be added. This should include at least a title.</param>
        /// <returns>A Task that represents the asynchronous operation. The result contains the created to-do item with a unique ID assigned.</returns>
        public Task<TodoItem> CreateAsync(TodoItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("Title is required.", nameof(item.Title));

            // Assign a new unique identifier (GUID) to the item
            item.Id = Guid.NewGuid();

            // Add the new item to the in-memory list
            _todoItems.Add(item);

            // Return the newly created to-do item asynchronously
            return Task.FromResult(item);
        }

        /// <summary>
        /// Asynchronously retrieves all the to-do items from the list, with optional filtering and sorting.
        /// </summary>
        /// <param name="dueDate">Optional filter for items with a specific due date.</param>
        /// <param name="isCompleted">Optional filter for completed items.</param>
        /// <param name="sortBy">Optional sorting parameter (e.g., "duedate" or "title").</param>
        /// <returns>A Task that represents the asynchronous operation. The result contains an IEnumerable of all filtered and sorted to-do items currently stored in memory.</returns>
        public Task<IEnumerable<TodoItem>> GetAllAsync(DateTime? dueDate = null, bool? isCompleted = null, string? sortBy = null)
        {
            // Start with all to-do items
            var items = _todoItems.AsQueryable();

            // Apply filtering
            if (dueDate.HasValue)
            {
                items = items.Where(i => i.DueDate.HasValue && i.DueDate.Value.Date == dueDate.Value.Date);
            }

            if (isCompleted.HasValue)
            {
                items = items.Where(i => i.Completed == isCompleted.Value);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                items = sortBy.ToLower() switch
                {
                    "duedate" => items.OrderBy(i => i.DueDate),
                    "title" => items.OrderBy(i => i.Title),
                    _ => items // Default to no sorting
                };
            }

            // Return the filtered and sorted list of to-do items asynchronously
            return Task.FromResult<IEnumerable<TodoItem>>(items.ToList());
        }

        /// <summary>
        /// Asynchronously retrieves a specific to-do item by its unique identifier (ID).
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item.</param>
        /// <returns>A Task that represents the asynchronous operation. The result contains the to-do item with the given ID, or null if not found.</returns>
        public Task<TodoItem> GetByIdAsync(Guid id)
        {
            // Find the item with the given ID
            var item = _todoItems.FirstOrDefault(t => t.Id == id);

            // Throw an exception if the item is not found
            if (item == null)
            {
                throw new KeyNotFoundException($"Todo item with ID {id} not found.");
            }

            // Return the result asynchronously
            return Task.FromResult(item);
        }

        /// <summary>
        /// Asynchronously updates an existing to-do item with new values for its properties.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be updated.</param>
        /// <param name="item">The updated to-do item with new values for the properties.</param>
        /// <returns>A Task that represents the asynchronous operation. The result contains the updated to-do item, or null if no item with the given ID was found.</returns>
        public Task<TodoItem> UpdateAsync(Guid id, TodoItem item)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("Title is required.", nameof(item.Title));

            // Retrieve the existing item by its ID
            var existingItem = _todoItems.FirstOrDefault(t => t.Id == id);

            // Throw an exception if the item is not found
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Todo item with ID {id} not found.");
            }

            // Update the fields of the existing item with the new values
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            existingItem.DueDate = item.DueDate;
            existingItem.Completed = item.Completed;

            // Return the updated to-do item asynchronously
            return Task.FromResult(existingItem);
        }

        /// <summary>
        /// Marks a specific to-do item as completed.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be marked as completed.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public Task MarkAsCompletedAsync(Guid id)
        {
            // Retrieve the existing item by its ID
            var existingItem = _todoItems.FirstOrDefault(t => t.Id == id);

            // Throw an exception if the item is not found
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Todo item with ID {id} not found.");
            }

            // Mark the item as completed
            existingItem.Completed = true;

            // Return a completed task
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously deletes a to-do item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to be deleted.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public Task DeleteAsync(Guid id)
        {
            // Retrieve the item to be deleted by its ID
            var item = _todoItems.FirstOrDefault(t => t.Id == id);

            // If the item is found, remove it from the list
            if (item != null)
            {
                _todoItems.Remove(item);
            }

            // Return a completed task
            return Task.CompletedTask;
        }
    }
}
