using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.DBEntity.ViewModels
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        [Required(ErrorMessage = "TaskName is required.")]
        public string TaskName { get; set; }
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Assignee is required.")]
        public string Assignee { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "DueDate is required.")]
        public DateTime DueDate { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public int CurrentPage { get; set; } = 1;
        public List<TaskModel> TaskList { get; set; }
    }
}
