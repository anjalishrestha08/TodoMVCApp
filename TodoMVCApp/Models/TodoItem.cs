using System.ComponentModel.DataAnnotations;
namespace TodoMVCApp.Models

{
    public class TodoItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")] 
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public bool InProcess { get; set; }
        public bool IsDone { get; set; }
        public string? UserId { get; set; }
    }
}
