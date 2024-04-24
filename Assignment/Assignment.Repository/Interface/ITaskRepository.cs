using Assignment.DBEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Interface
{
    public interface ITaskRepository
    {
        public TaskModel GetRecords(TaskModel model);
        public Task<bool> GetRecordById(TaskModel model);
        public Task<bool> DeleteTask(int Id);
        public Task<List<CategoryModel>> CategoryComboBox();
        public Task<bool> AddTask(TaskModel model);
    }
}
