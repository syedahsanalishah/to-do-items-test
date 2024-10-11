using System;
using System.ComponentModel.DataAnnotations;
using to_do_items_test.Services;

namespace to_do_items_test.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } // Unique ID for the to-do item

        /// <summary>
        /// Title is required and must be at most 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty; // Title of the to-do item

        /// <summary>
        /// Optional description with a maximum length of 500 characters.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; } // Description (optional)

        /// <summary>
        /// Optional due date for the to-do item. Must be today or a future date if provided.
        /// </summary>
        [DataType(DataType.Date)]
        [FutureOrTodayDate(ErrorMessage = "Due date must be today or in the future.")]
        public DateTime? DueDate { get; set; } // Due date (optional)

        /// <summary>
        /// Completion status of the to-do item.
        /// </summary>
        public bool Completed { get; set; } // Completion status
    }
}
