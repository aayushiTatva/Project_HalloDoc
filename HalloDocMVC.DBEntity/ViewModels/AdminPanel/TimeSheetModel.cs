using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class TimeSheetModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        public string AdminApproveStatus { get; set; }
        public int? Date { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<DateTime> DateList { get; set; }
        public int OnCallHours { get; set; }
        public int TotalHours { get; set; }
        public bool IsWeekend_IsHoliday { get; set; }
        public int NumberOfHouseCalls { get; set; }
        public int NumberofPhoneCalls { get; set; }
        public int TimeSheetDetailId { get; set; }
        public int TimeSheetId { get; set; }
        public DateTime TimeSheetDate { get; set; }
        public List<TimeSheetModel> TimeSheetData { get; set; }
    }
}
