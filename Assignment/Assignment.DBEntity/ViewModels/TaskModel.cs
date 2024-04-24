using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.DBEntity.ViewModels
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Assignee { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string City { get; set; }
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public int CurrentPage { get; set; } = 1;
        public List<TaskModel> TaskList { get; set; }
    }
}
