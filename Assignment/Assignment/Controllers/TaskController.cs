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
            return View("~/Views/Task/TaskRecords.cshtml", taskModel);
        }

        public async Task<IActionResult> DeleteTask(int Id)
        {
            bool taskModel = await _ITaskRepository.DeleteTask(Id);
            return View("~/Views/Task/TaskRecords.cshtml", taskModel);
        }

        public async Task<IActionResult> AddTask(TaskModel model)
        {
            ViewBag.CategoryComboBox = await _ITaskRepository.CategoryComboBox();
            bool taskModel = await _ITaskRepository.AddTask(model);
            return View("~/Views/Task/TaskRecords.cshtml", taskModel);
        }
    }
}
