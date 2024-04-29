using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface ISchedulingService
    {
        public void AddShift(SchedulingModel model, List<string?>? chk, string adminId);
        void ViewShift(int shiftdetailid);
        void ViewShiftreturn(SchedulingModel modal);
        bool EditShiftSave(SchedulingModel modal, string id);
        bool ViewShiftDelete(SchedulingModel modal, string id);
        Task<List<ProviderModel>> ProviderOnCall(int? region);
        Task<List<SchedulingModel>> GetAllNotApprovedShift(int? regionId);
        Task<bool> DeleteShift(string s, string AdminID);
        Task<bool> UpdateStatusShift(string s, string AdminID);
    }
}
