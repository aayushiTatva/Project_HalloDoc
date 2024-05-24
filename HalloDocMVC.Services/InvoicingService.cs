using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class InvoicingService : IInvoicingService
    {
            #region Constructor
            private readonly IHttpContextAccessor httpContextAccessor;
            private readonly EmailConfiguration _emailConfig;
            private readonly IGenericRepository<Emaillog> _emailLogRepository;
            private readonly IGenericRepository<TimesheetDetail> _timeSheetDetailRepository;
            private readonly IGenericRepository<Timesheet> _timeSheetRepository;
            private readonly IGenericRepository<TimesheetDetailReimbursement> _timeSheetDetailReimbursementRepository;
            private readonly IGenericRepository<Shift> _shiftRepository;
            private readonly IGenericRepository<Shiftdetail> _shiftDetailRepository;
            private readonly IGenericRepository<Physician> _physicianRepository;
            private readonly IGenericRepository<PayrateByProvider> _payrateByProviderRepository;
            public InvoicingService(EmailConfiguration emailConfig, IHttpContextAccessor httpContextAccessor,IGenericRepository<Emaillog> iEmailLogRepository,IGenericRepository<TimesheetDetail> iTimeSheetDetailRepository,
                IGenericRepository<Timesheet> iTimesheetRepository, IGenericRepository<TimesheetDetailReimbursement> iTimeSheetDetailReimbursementRepository,
                IGenericRepository<Shift> iShiftRepository, IGenericRepository<Shiftdetail> iShiftDetailRepository, IGenericRepository<Physician> iPhysicianRepository,
                IGenericRepository<PayrateByProvider> iPayrateByProviderRepository)
            {
                _emailConfig = emailConfig;
                this.httpContextAccessor = httpContextAccessor;
                _emailLogRepository = iEmailLogRepository;
                _timeSheetDetailRepository = iTimeSheetDetailRepository;
                _timeSheetRepository = iTimesheetRepository;
                _timeSheetDetailReimbursementRepository = iTimeSheetDetailReimbursementRepository;
                _shiftRepository = iShiftRepository;
                _shiftDetailRepository = iShiftDetailRepository;
                _physicianRepository = iPhysicianRepository;
                _payrateByProviderRepository = iPayrateByProviderRepository;
            }
            #endregion

            #region TimesheetApproval
            public bool isApprovedTimesheet(int PhysicianId, DateOnly StartDate)
            {
                var data = _timeSheetRepository.GetAll().Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
                if (data.IsApproved == false)
                {

                    return false;
                }
                return true;
            }
            #endregion

            #region TimesheetFinalize
            public bool isFinalizeTimesheet(int PhysicianId, DateOnly StartDate)
            {
                var data = _timeSheetRepository.GetAll().Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
                if (data == null)
                {
                    return false;
                }
                else if (data.IsFinalize == false)
                {

                    return false;
                }
                return true;
            }
            #endregion

            #region SetToFinalize
            public bool SetToFinalize(int timesheetid, string AdminId)
            {
                try
                {
                    var data = _timeSheetRepository.GetAll().Where(e => e.TimesheetId == timesheetid).FirstOrDefault();
                    if (data != null)
                    {
                        data.IsFinalize = true;
                        data.IsApproved = false;
                        _timeSheetRepository.Update(data);
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
            #endregion

            #region Set_To_Sheet_Approve
            public async Task<bool> SetToApprove(TimeSheetModel tsm, string AdminId)
            {
                try
                {
                    var data = _timeSheetRepository.GetAll().Where(e => e.TimesheetId == tsm.Timesheeid).FirstOrDefault();
                var physician = _physicianRepository.GetAll().Where(e => e.Physicianid == tsm.PhysicianId).FirstOrDefault();
                    if (data != null)
                    {
                        data.IsApproved = true;
                        data.BonusAmount = tsm.Bonus;
                        data.AdminNotes = tsm.AdminNotes;
                        _timeSheetRepository.Update(data);
                    }
                    var d = httpContextAccessor.HttpContext.Request.Host;
                    //var res = _context.Requestclients.FirstOrDefault(e => e.Requestid == v.RequestID);
                    string emailContent = @"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                 <meta charset=""UTF-8"">
                                 <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                 <title>Provider</title>
                                </head>
                                <body>
                                 <div style=""background-color: #f5f5f5; padding: 20px;"">
                                 <h2>Welcome to Our Healthcare Platform!</h2>
                                <p>Dear Provider ,</p>
                                <ol>
                                    <li>Your TimeSheet Startwith" + data.StartDate + @""" and End With " + data.EndDate + @""" Is Approve</li>
                                </ol>
                                <p>If you have any questions or need further assistance, please don't hesitate to contact us.</p>
                                <p>Thank you,</p>
                                <p>The Healthcare Team</p>
                                </div>
                                </body>
                                </html>
                                ";
                    _emailConfig.SendMail(physician.Email, "New Order arrived", emailContent);
                    Emaillog log = new()
                    {
                        Emailtemplate = emailContent,
                        Subjectname = "New Order arrived",
                        Emailid = physician.Email,
                        Createdate = DateTime.Now,
                        Sentdate = DateTime.Now,
                        Physicianid = data.PhysicianId,
                        Action = 12,
                        Isemailsent = new BitArray(new[] { true }),
                        Senttries = 1,
                        Roleid = 3,
                    };
                    _emailLogRepository.Add(log);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            #endregion

            #region Timesheet_Add
            public List<TimesheetDetail> PostTimesheetDetails(int PhysicianId, DateOnly StartDate, int AfterDays, string AdminId)
            {
                var Timesheet = new Timesheet();
                var data = _timeSheetRepository.GetAll().Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
                if (data == null && AfterDays != 0)
                {
                    DateOnly EndDate = StartDate.AddDays(AfterDays - 1);
                    Timesheet.StartDate = StartDate;
                    Timesheet.PhysicianId = PhysicianId;
                    Timesheet.IsFinalize = false;
                    Timesheet.EndDate = EndDate;
                    Timesheet.CreatedDate = DateTime.Now;
                    Timesheet.CreatedBy = AdminId;
                    _timeSheetRepository.Add(Timesheet);

                    for (DateOnly i = StartDate; i <= EndDate; i = i.AddDays(1))
                    {
                        var Timesheetdetail = new TimesheetDetail
                        {
                            TimesheetId = Timesheet.TimesheetId,
                            TimesheetDate = i,
                            NumberOfHouseCall = 0,
                            NumberOfPhoneCall = 0,
                            TotalHours = 0
                        };
                        _timeSheetDetailRepository.Add(Timesheetdetail);
                    }

                    return _timeSheetDetailRepository.GetAll().Where(e => e.TimesheetId == Timesheet.TimesheetId).OrderBy(r => r.TimesheetDate).ToList();
                }
                else if (data == null && AfterDays == 0)
                {
                    return null;

                }
                else
                {
                    return _timeSheetDetailRepository.GetAll().Where(e => e.TimesheetId == data.TimesheetId).OrderBy(r => r.TimesheetDate).ToList();
                }

            }
            #endregion
           
            #region Timesheet_Edit
            public bool PutTimesheetDetails(List<TimesheetdetailModel> tds, string AdminId)
            {
                try
                {                  
                    foreach (var item in tds)
                    {
                        var td = _timeSheetDetailRepository.GetAll().Where(r => r.TimesheetDetailId == item.Timesheetdetailid).FirstOrDefault();
                        td.TotalHours = item.Totalhours;
                        td.NumberOfHouseCall = item.Numberofhousecall;
                        td.NumberOfPhoneCall = item.Numberofphonecall;
                        td.IsWeekend = item.Isweekend;
                        td.ModifiedBy = AdminId;
                        td.ModifiedDate = DateTime.Now;
                        _timeSheetDetailRepository.Update(td);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }


            }

            #endregion

            #region Timesheet_Get
            public TimeSheetModel GetTimesheetDetails(List<TimesheetDetail> td, List<TimesheetdetailreimbursementModel> tr, int PhysicianId)
            {
                try
                {
                    var TimeSheet = new TimeSheetModel();

                    TimeSheet.TimesheetdetailsList = td.Select(e => new TimesheetdetailModel
                    {
                        Isweekend = e.IsWeekend == null || e.IsWeekend == false ? false : true,
                        Modifiedby = e.ModifiedBy,
                        Modifieddate = e.ModifiedDate,
                        Numberofhousecall = e.NumberOfHouseCall,
                        Numberofphonecall = e.NumberOfPhoneCall,
                        OnCallhours = FindOnCallProvider(PhysicianId, e.TimesheetDate),
                        Timesheetdate = e.TimesheetDate,
                        Timesheetdetailid = e.TimesheetDetailId,
                        Totalhours = e.TotalHours,
                        Timesheetid = e.TimesheetId
                    }).OrderBy(r => r.Timesheetdate).ToList();
                    if (tr != null)
                    {
                        TimeSheet.TimesheetdetailreimbursementList = tr.Select(e => new TimesheetdetailreimbursementModel
                        {
                            Amount = e.Amount,
                            Timesheetdetailreimbursementid = e.Timesheetdetailreimbursementid,
                            Isdeleted = e.Isdeleted,
                            Itemname = e.Itemname,
                            Bill = e.Bill,
                            Createddate = e.Createddate,
                            Timesheetdate = e.Timesheetdate,
                            Timesheetid = _timeSheetDetailRepository.GetAll().Where(r => r.TimesheetDetailId == e.Timesheetdetailid).FirstOrDefault().TimesheetId,
                            Modifiedby = e.Modifiedby,
                            Timesheetdetailid = e.Timesheetdetailid,
                        }).OrderBy(r => r.Timesheetdetailid).ToList();
                    }
                    else
                    {
                        TimeSheet.TimesheetdetailreimbursementList = new List<TimesheetdetailreimbursementModel> { };
                    }
                TimeSheet.PayrateWithProviderList = _payrateByProviderRepository.GetAll().Where(r => r.PhysicianId == PhysicianId).ToList();
                if (td.Count > 0)
                {
                    TimeSheet.Timesheeid = TimeSheet.TimesheetdetailsList[0].Timesheetid;
                }
                TimeSheet.PhysicianId = PhysicianId;
                    return TimeSheet;
                }
                catch (Exception e)
                {
                    return null;
                }


            }
            #endregion

            #region FindOnCallProvider
            public int FindOnCallProvider(int PhysicianId, DateOnly Timesheetdate)
            {
                int i = 0;
                var s = _shiftRepository.GetAll().Where(r => r.Physicianid == PhysicianId).ToList();
                foreach (var item in s)
                {
                    i += _shiftDetailRepository.GetAll().Where(r => r.Shiftid == item.Shiftid && DateOnly.FromDateTime(r.Shiftdate) == Timesheetdate).Count();
                }
                return i;
            }
            #endregion

            #region Timesheet_Bill_Get
            public async Task<List<TimesheetdetailreimbursementModel>> GetTimesheetBills(List<TimesheetDetail> TimeSheetDetails)
            {
                try
                {
                    List<TimesheetdetailreimbursementModel> TimeSheetBills = await (
                        from timesheetdoc in _timeSheetDetailReimbursementRepository.GetAll()
                        join timesheetdetail in _timeSheetDetailRepository.GetAll() // Join with the input list
                        on timesheetdoc.TimesheetDetailId equals timesheetdetail.TimesheetDetailId
                        where TimeSheetDetails.Contains(timesheetdoc.TimesheetDetail) && !(timesheetdoc.IsDeleted ?? false)// Assuming IsDeleted is a property in Timesheetdetailreimbursements table
                        select new TimesheetdetailreimbursementModel
                        {
                            Timesheetdetailreimbursementid = timesheetdoc.TimesheetDetailReimbursementId,
                            Timesheetdetailid = timesheetdoc.TimesheetDetailId,
                            Itemname = timesheetdoc.ItemName,
                            Amount = timesheetdoc.Amount,
                            Timesheetdate = timesheetdetail.TimesheetDate,
                            Bill = timesheetdoc.Bill,

                        }).ToListAsync();

                    return TimeSheetBills;
                }
                catch (Exception e)
                {
                    return null;
                }
            }


            #endregion
            #region TimeSheet_Bill_AddEdit
            public bool TimeSheetBillAddEdit(TimesheetdetailreimbursementModel trb, string AdminId)
            {
                TimesheetDetail data = _timeSheetDetailRepository.GetAll().Where(e => e.TimesheetDetailId == trb.Timesheetdetailid).FirstOrDefault();
                if (data != null && trb.Timesheetdetailreimbursementid == null)
                {
                    TimesheetDetailReimbursement timesheetdetailreimbursement = new TimesheetDetailReimbursement();
                    timesheetdetailreimbursement.TimesheetDetailId = trb.Timesheetdetailid;
                    timesheetdetailreimbursement.Amount = (int)trb.Amount;
                    timesheetdetailreimbursement.Bill = FileSave.UploadTimesheetDoc(trb.Billfile, data.TimesheetId);
                    timesheetdetailreimbursement.ItemName = trb.Itemname;
                    timesheetdetailreimbursement.CreatedDate = DateTime.Now;
                    timesheetdetailreimbursement.CreatedBy = AdminId;
                    timesheetdetailreimbursement.IsDeleted = false;
                    _timeSheetDetailReimbursementRepository.Add(timesheetdetailreimbursement);


                    return true;
                }
                else if (data != null && trb.Timesheetdetailreimbursementid != null)
                {
                    TimesheetDetailReimbursement timesheetdetailreimbursement = _timeSheetDetailReimbursementRepository.GetAll().Where(r => r.TimesheetDetailReimbursementId == trb.Timesheetdetailreimbursementid).FirstOrDefault(); ;
                    timesheetdetailreimbursement.Amount = (int)trb.Amount;

                    timesheetdetailreimbursement.ItemName = trb.Itemname;
                    timesheetdetailreimbursement.ModifiedDate = DateTime.Now;
                    timesheetdetailreimbursement.ModifiedBy = AdminId;
                    _timeSheetDetailReimbursementRepository.Update(timesheetdetailreimbursement);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            #endregion

            #region TimeSheetBill_Delete
            public bool TimeSheetBillRemove(TimesheetdetailreimbursementModel trb, string AdminId)
            {
                TimesheetDetailReimbursement data = _timeSheetDetailReimbursementRepository.GetAll().Where(e => e.TimesheetDetailReimbursementId == trb.Timesheetdetailreimbursementid).FirstOrDefault();
                if (data != null)
                {

                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = AdminId;
                    data.IsDeleted = true;
                    _timeSheetDetailReimbursementRepository.Update(data);
                    return true;
                }
                return false;

            }
        #endregion
            
            #region GetAllPhysicians
            public List<ProviderModel> GetAllPhysicians()
            {
                var result = new List<ProviderModel>();
                result = (from py in _physicianRepository.GetAll()
                          select new ProviderModel
                          {
                              UserName = py.Firstname + " " + py.Lastname,
                              PhysicianId = py.Physicianid,
                          }).ToList();
                return result;
            }
        #endregion
       
            #region GetPendingTimesheet
        public List<Timesheet> GetPendingTimesheet(int PhysicianId, DateOnly StartDate)
        {
            var result = new List<Timesheet>();
            result = (from timesheet in _timeSheetRepository.GetAll()
                        where (timesheet.IsApproved == false && timesheet.PhysicianId == PhysicianId && timesheet.StartDate == StartDate)
                        select timesheet).ToList();

            return result;
        }
        #endregion
        
    }
}
