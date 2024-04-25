using Assignment.DBEntity.DataContext;
using Assignment.DBEntity.ViewModels;
using Assignment.Models;
using Assignment.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskManagementSystemContext _context;
        private readonly ITaskRepository _ITaskRepository;

        public HomeController( ITaskRepository iTaskRepository, TaskManagementSystemContext context)
        {
            _ITaskRepository = iTaskRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(TaskModel model)
        {
            ViewBag.CategoryComboBox = await _ITaskRepository.CategoryComboBox();
            TaskModel taskModel = _ITaskRepository.GetRecords(model);
            return View("~/Views/Task/TaskRecords.cshtml", taskModel);
        }

    }
}