using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class InvoicingService : IInvoicingService
    {
        #region Configuration
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<PayrateByProvider> _payrateByProviderRepository;
        private readonly IGenericRepository<PayrateCategory> _payrateCategoryRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Timesheet> _timeSheetRepository;
        private readonly IGenericRepository<TimesheetDetail> _timeSheetDetailRepository;
        private readonly IGenericRepository<TimesheetDetailReimbursement> _timeSheetDetailReimbursementRepository;

        public InvoicingService(IGenericRepository<Request> requestRepository, IGenericRepository<PayrateByProvider> payrateByProviderRepository,
            IGenericRepository<PayrateCategory> payrateCategoryRepository, IGenericRepository<Physician> physicianRepository, IGenericRepository<Timesheet> timeSheetRepository,
            IGenericRepository<TimesheetDetail> timeSheetDetailRepository, IGenericRepository<TimesheetDetailReimbursement> timeSheetDetailReimbursementRepository)
        {
            _requestRepository = requestRepository;
            _payrateByProviderRepository = payrateByProviderRepository;
            _payrateCategoryRepository = payrateCategoryRepository;
            _physicianRepository = physicianRepository;
            _timeSheetRepository = timeSheetRepository;
            _timeSheetDetailRepository = timeSheetDetailRepository;
            _timeSheetDetailReimbursementRepository = timeSheetDetailReimbursementRepository;
        }
        #endregion

        #region GetPayrateByProvider
        public List<PayrateModel> GetPayrateByProvider(int Id, PayrateModel model)
        {
            List<PayrateModel> model1 = (from pbp in _payrateByProviderRepository.GetAll()
                                         join pc in _payrateCategoryRepository.GetAll()
                                         on pbp.PayrateCategoryId equals pc.PayrateCategoryId into PayrateGroup
                                         from pg in PayrateGroup.DefaultIfEmpty()
                                         join phy in _physicianRepository.GetAll()
                                         on pbp.PhysicianId equals phy.Physicianid into PayratePhysicianGroup
                                         from phyGroup in PayratePhysicianGroup.DefaultIfEmpty()
                                         where pbp.PhysicianId == Id
                                         select new PayrateModel
                                         {
                                             Payrate = (int)pbp.Payrate,
                                             PayrateCategoryId = pbp.PayrateCategoryId,
                                             PayrateCategoryName = pg.CategoryName,
                                             PhysicianId = pbp.PhysicianId
                                         }).ToList();

            return model1;
        }
        #endregion

        #region EditPayrate
        public async Task<bool> EditPayrate(PayrateModel pm, int categoryId, int id)
        {
            var data = _payrateByProviderRepository.GetAll().FirstOrDefault(e => e.PhysicianId == id && e.PayrateCategoryId == categoryId);
            if(data != null)
            {
                data.PayrateCategoryId = categoryId;
                data.Payrate = pm.Payrate;
                _payrateByProviderRepository.Update(data);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region GetTimesheet
        public TimeSheetModel GetTimesheet(TimeSheetModel psm, DateTime date)
        {
            int day = (int)psm.Date;
            int month = (int)psm.Month;
            int year = (int)psm.Year;

            DateTime dates = new DateTime(year, month, day);
            List<DateTime> dateList = new List<DateTime>();
            if (day == 1)
            {
                for (int i = 0; i < 14; i++)
                {
                    DateTime currentDate = dates.AddDays(i);
                    dateList.Add(currentDate);
                }
            }
            else
            {
                if (month == 2)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        DateTime currentDate = dates.AddDays(i);
                        dateList.Add(currentDate);
                    }
                }
                else if (month % 2 != 0)
                {
                    if (month <= 7)
                    {
                        for (int i = 0; i < 17; i++)
                        {
                            DateTime currentDate = dates.AddDays(i);
                            dateList.Add(currentDate);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            DateTime currentDate = dates.AddDays(i);
                            dateList.Add(currentDate);
                        }
                    }

                }
                else
                {
                    if (month <= 7)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            DateTime currentDate = dates.AddDays(i);
                            dateList.Add(currentDate);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 17; i++)
                        {
                            DateTime currentDate = dates.AddDays(i);
                            dateList.Add(currentDate);
                        }
                    }
                }
            }


            List<TimeSheetModel> timesheetdata = (from ts in _timeSheetRepository.GetAll()
                                 join tsd in _timeSheetDetailRepository.GetAll() on ts.TimesheetId equals tsd.TimesheetId into timesheetGroup
                                 from tsg in timesheetGroup.DefaultIfEmpty()
                                 where ts.PhysicianId == null
                                 select new TimeSheetModel
                                 {
                                      TotalHours = (int)tsg.TotalHours,
                                      IsWeekend_IsHoliday = (bool)tsg.IsWeekend,
                                      NumberOfHouseCalls = (int)tsg.NumberOfHouseCall,
                                      NumberofPhoneCalls = (int)tsg.NumberOfPhoneCall,
                                      TimeSheetDate = tsg.TimesheetDate,
                                      TimeSheetDetailId = tsg.TimesheetDetailId,
                                      TimeSheetId = 1
                                 }).ToList();
            TimeSheetModel model = new TimeSheetModel
            {
                DateList = dateList,
                TimeSheetData = timesheetdata,
                StartDate = dates
            };

            return model;
        }
        #endregion

        #region EditTimesheet
        public async Task<bool> EditTimesheet(TimeSheetModel model, int TimesheetId, string PhysicianAspnetuserID)
        {
           
            if (TimesheetId == 0)
            {
                Timesheet timesheet = new Timesheet();
                var physician = _physicianRepository.GetAll().FirstOrDefault(e => e.Aspnetuserid == PhysicianAspnetuserID);

                if (physician != null)
                {
                    timesheet.PhysicianId = physician.Physicianid;
                    timesheet.StartDate = (DateTime)model.StartDate;
                    timesheet.CreatedBy = PhysicianAspnetuserID;
                    timesheet.CreatedDate = DateTime.Now;
                    _timeSheetRepository.Add(timesheet);
                    return true;
                }
                else
                {
                    // Handle the case where the Physician with the given AspnetUserId is not found
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
//    using (var db = new YourDbContext())
//    {
//        // Fetch the TimesheetDetail data for the generated dates
//        var timesheetData = await db.TimesheetDetail
//            .Where(td => dateList.Contains(td.TimesheetDate.Date))
//            .ToListAsync();

//        // Map the TimesheetDetail data to the TimesheetDetailModel
//        timesheetDetails = timesheetData.Select(td => new TimesheetDetailModel
//        {
//            TimesheetDetailId = td.TimesheetDetailId,
//            PhysicianId = td.PhysicianId,
//            // Add other properties from the TimesheetDetail table
//        }).ToList();
//    }


