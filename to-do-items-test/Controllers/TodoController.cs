using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using to_do_items_test.Models;
using to_do_items_test.Services;

namespace to_do_items_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensures that this controller is protected and requires authentication
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// Creates a new to-do item asynchronously.
        /// </summary>
        /// <param name="item">The to-do item to be created. Requires a title.</param>
        /// <returns>The created to-do item with its unique ID.</returns>
        /// <response code="201">Created - The item was successfully created.</response>
        /// <response code="400">Bad Request - The item could not be created due to missing or invalid data.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem item)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request with validation errors
            }

            // Asynchronously create the to-do item
            var createdItem = await _todoService.CreateAsync(item);

            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem); // 201 Created with location header
        }

        /// <summary>
        /// Retrieves all to-do items asynchronously.
        /// </summary>
        /// <returns>An array of all to-do items.</returns>
        /// <response code="200">OK - The items were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoItem[]>> GetAll()
        {
            var items = await _todoService.GetAllAsync(); // Asynchronous service call
            return Ok(items.ToArray()); // 200 OK with list of to-do items
        }

        /// <summary>
        /// Retrieves a specific to-do item by its unique identifier (ID) asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item.</param>
        /// <returns>The to-do item with the specified ID.</returns>
        /// <response code="200">OK - The item was successfully retrieved.</response>
        /// <response code="404">Not Found - The item with the given ID was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItem>> GetById(Guid id)
        {
            var item = await _todoService.GetByIdAsync(id); // Asynchronous service call
            if (item == null) return NotFound(); // 404 Not Found if the item does not exist
            return Ok(item); // 200 OK with the requested item
        }

        /// <summary>
        /// Updates an existing to-do item asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to update.</param>
        /// <param name="item">The updated to-do item.</param>
        /// <returns>The updated to-do item.</returns>
        /// <response code="200">OK - The item was successfully updated.</response>
        /// <response code="404">Not Found - The item with the given ID was not found.</response>
        /// <response code="400">Bad Request - The item could not be updated due to validation errors.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Update(Guid id, [FromBody] TodoItem item)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request with validation errors
            }

            // Asynchronously update the to-do item
            var updatedItem = await _todoService.UpdateAsync(id, item);
            if (updatedItem == null) return NotFound(); // 404 Not Found if the item does not exist
            return Ok(updatedItem); // 200 OK with the updated item
        }

        /// <summary>
        /// Deletes a specific to-do item by its unique identifier (ID) asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to delete.</param>
        /// <response code="204">No Content - The item was successfully deleted.</response>
        /// <response code="404">Not Found - The item with the given ID was not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _todoService.GetByIdAsync(id); // Asynchronous service call
            if (item == null) return NotFound(); // 404 Not Found if the item does not exist

            await _todoService.DeleteAsync(id); // Asynchronous service call
            return NoContent(); // 204 No Content after successful deletion
        }
        // <summary>
        /// Marks a specific to-do item as completed asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the to-do item to mark as completed.</param>
        /// <response code="204">No Content - The item was successfully marked as completed.</response>
        /// <response code="404">Not Found - The item with the given ID was not found.</response>
        [HttpPatch("{id}/complete")] // Using PATCH for partial updates
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            try
            {
                await _todoService.MarkAsCompletedAsync(id); // Call the service to mark as completed
                return NoContent(); // 204 No Content on successful completion
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404 Not Found if the item does not exist
            }
        }
    }
}
