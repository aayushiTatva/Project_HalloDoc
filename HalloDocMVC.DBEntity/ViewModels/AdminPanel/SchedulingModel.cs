using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SchedulingModel
    {
        public int RegionId { get; set; }
        public int PhysicianId { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int RepeatCount { get; set; }
        public string PhysicianName { get; set; }
        public string ModalDate { get; set; }
        public int ShiftDetailId { get; set; }
        public string RegionName { get; set; }
        public short Status { get; set; }
    }
    public class DayWiseScheduling
    {
        public DateTime Date { get; set; }
        public List<Physician> Physicians { get; set; }
        public List<Shiftdetail> ShiftDetail { get; set; }
    }
    public class MonthWiseScheduling
    {
        public DateTime Date { get; set; }
        public List<Shiftdetail> ShiftDetail { get; set; }
    }
    public class WeekWiseScheduling
    {
        public DateTime Date { get; set; }
        public List<Physician> Physicians { get; set; }
        public List<Shiftdetail> ShiftDetail { get; set; }
    }

}
