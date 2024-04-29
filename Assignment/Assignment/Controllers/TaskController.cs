using Assignment.DBEntity.DataContext;
using Assignment.DBEntity.ViewModels;
using Assignment.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskManagementSystemContext _context;
        private readonly ITaskRepository _ITaskRepository;

        public TaskController(ITaskRepository iTaskRepository, TaskManagementSystemContext context)
        {
            _ITaskRepository = iTaskRepository;
            _context = context;
        }
        public async Task<IActionResult> EditTask(TaskModel model)
        {
            ViewBag.CategoryComboBox = await _ITaskRepository.CategoryComboBox();
            bool taskModel = await _ITaskRepository.GetRecordById(model);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteTask(int Id)
        {
            bool taskModel = await _ITaskRepository.DeleteTask(Id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AddTask(TaskModel model)
        {
            ViewBag.CategoryComboBox = await _ITaskRepository.CategoryComboBox();
            bool taskModel = await _ITaskRepository.AddTask(model);
            return RedirectToAction("Index", "Home");
        }

        public TaskModel ViewTask(int taskid)
        {
            TaskModel modal = new();
            var task = _context.Tasks.FirstOrDefault(u => u.Id == taskid);

            modal.TaskName = task.TaskName;
            modal.Assignee = task.Assignee;
            modal.Description = task.Description;
            modal.DueDate = (DateTime)task.DueDate;
            modal.CategoryName = task.Category;
            modal.CategoryId = (int)task.CategoryId;
            modal.City = task.City;
            modal.TaskId = taskid;
            return modal;
        }

    }
}
