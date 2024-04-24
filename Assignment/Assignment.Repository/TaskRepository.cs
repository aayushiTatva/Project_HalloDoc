using Assignment.DBEntity.DataContext;
using Assignment.DBEntity.DataModels;
using Assignment.DBEntity.ViewModels;
using Assignment.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository
{
    public class TaskRepository : ITaskRepository
    {

        private readonly TaskManagementSystemContext _context;
        public TaskRepository(TaskManagementSystemContext context)
        {
            _context = context;
        }

        public TaskModel GetRecords(TaskModel model)
        {
            List<TaskModel> tm = (from hp in _context.Tasks
                                  join cg in _context.Categories 
                                  on hp.CategoryId equals cg.Id into TaskCategoryGroup
                                  from tcg in TaskCategoryGroup.DefaultIfEmpty()
                                  where hp.Isdeleted == new BitArray(1) 
                                  select new TaskModel
                                  {
                                      TaskId = hp.Id,
                                      TaskName = hp.TaskName,
                                      Assignee = hp.Assignee,
                                      CategoryId = (int)hp.CategoryId,
                                      CategoryName = tcg.Name,
                                      City = hp.City,
                                      DueDate = (DateTime)hp.DueDate,
                                      Description = hp.Description
                                  }).ToList();
            int totalitemCount = tm.Count;
            int TotalPages = (int)Math.Ceiling(totalitemCount / (double)model.PageSize);
            List<TaskModel> list = tm.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            TaskModel task = new()
            {
                TaskList = list,
                CurrentPage = model.CurrentPage,
                TotalPages = TotalPages
            };
            return task;
        }

        public async Task<bool> GetRecordById(TaskModel model)
        {
            
            if(model == null)
            {
                return false;
            }
            else
            {
                var data = await _context.Tasks.Where(e => e.Id == model.TaskId).FirstOrDefaultAsync();
                if(data != null)
                {
                    data.TaskName = model.TaskName;
                    data.Assignee = model.Assignee;
                    data.CategoryId = model.CategoryId;
                    data.Category = model.CategoryName;
                    data.City = model.City;
                    data.DueDate = model.DueDate;   
                    data.Description = model.Description;
                }
                _context.Tasks.Update(data);
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<bool> DeleteTask(int Id)
        {
            if(Id != 0)
            {
                var data = _context.Tasks.FirstOrDefault(e => e.Id == Id);
                if(data != null)
                {
                    _context.Tasks.Remove(data);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<CategoryModel>> CategoryComboBox()
        {
            return await _context.Categories.Select(category => new CategoryModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name
            }).ToListAsync();
        }

        public async Task<bool> AddTask(TaskModel model)
        {
            if(model.TaskId == 0)
            {
                DBEntity.DataModels.Task tm = new()
                {
                    TaskName = model.TaskName,
                    Assignee = model.Assignee,
                    CategoryId = model.CategoryId,
                    Category = model.CategoryName,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    City = model.City,
                };
                _context.Tasks.Add(tm);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
