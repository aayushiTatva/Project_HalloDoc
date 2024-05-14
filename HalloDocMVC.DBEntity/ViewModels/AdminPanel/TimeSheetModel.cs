using HalloDocMVC.DBEntity.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class TimeSheetModel
    {
        public List<TimesheetdetailModel>? TimesheetdetailsList { get; set; }
        public List<TimesheetdetailreimbursementModel>? TimesheetdetailreimbursementList { get; set; }
        public List<PayrateByProvider>? PayrateWithProviderList { get; set; }
        public int Timesheeid { get; set; }
        public string? Bonus { get; set; }
        public string? AdminNotes { get; set; }
        public int PhysicianId { get; set; }
    }
    public class TimesheetdetailModel
    {
        public int Timesheetdetailid { get; set; }
        public int Timesheetid { get; set; }
        public DateOnly Timesheetdate { get; set; }
        public int? OnCallhours { get; set; }
        public decimal? Totalhours { get; set; }
        public bool Isweekend { get; set; }
        public int? Numberofhousecall { get; set; }
        public int? Numberofphonecall { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
    }
    public class TimesheetdetailreimbursementModel
    {
        public int? Timesheetdetailreimbursementid { get; set; } = null!;
        public int Timesheetdetailid { get; set; }
        public int Timesheetid { get; set; }
        public string Itemname { get; set; } = null!;
        public int? Amount { get; set; } = null!;
        public DateOnly Timesheetdate { get; set; }
        public string Bill { get; set; } = null!;
        public IFormFile Billfile { get; set; }
        public bool? Isdeleted { get; set; }
        public string Createdby { get; set; } = null!;
        public DateTime Createddate { get; set; }
        public string? Modifiedby { get; set; }
    }
}
