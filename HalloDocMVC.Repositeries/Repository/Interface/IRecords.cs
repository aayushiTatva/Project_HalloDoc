using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IRecords
    {
        public Task<RecordsModel> GetRecords(RecordsModel model);
        public Task<RecordsModel> GetPatientHistory(RecordsModel model);
        public Task<RecordsModel> GetPatientCases(int UserId, RecordsModel records);
        public Task<RecordsModel> GetEmailLogs(RecordsModel model);
        public Task<RecordsModel> GetSMSLogs(RecordsModel model);
        public Task<RecordsModel> GetBlockedHistory(RecordsModel model);
    }
}
