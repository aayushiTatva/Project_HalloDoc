using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IInvoicingService
    {
        public bool isApprovedTimesheet(int PhysicianId, DateOnly StartDate);
        public bool isFinalizeTimesheet(int PhysicianId, DateOnly StartDate);
        public bool SetToFinalize(int timesheetid, string AdminId);
        public Task<bool> SetToApprove(TimeSheetModel tsm, string AdminId);
        public List<TimesheetDetail> PostTimesheetDetails(int PhysicianId, DateOnly StartDate, int AfterDays, string AdminId);
        public bool PutTimesheetDetails(List<TimesheetdetailModel> tds, string AdminId);
        public TimeSheetModel GetTimesheetDetails(List<TimesheetDetail> td, List<TimesheetdetailreimbursementModel> tr, int PhysicianId);
        public int FindOnCallProvider(int PhysicianId, DateOnly Timesheetdate);
        public Task<List<TimesheetdetailreimbursementModel>> GetTimesheetBills(List<TimesheetDetail> TimeSheetDetails);
        public bool TimeSheetBillAddEdit(TimesheetdetailreimbursementModel trb, string AdminId);
        public bool TimeSheetBillRemove(TimesheetdetailreimbursementModel trb, string AdminId);
        public List<ProviderModel> GetAllPhysicians();
        public List<Timesheet> GetPendingTimesheet(int PhysicianId, DateOnly StartDate);
    }
}
