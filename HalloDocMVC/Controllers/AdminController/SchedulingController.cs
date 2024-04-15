using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Controllers.AdminController
{
    public class SchedulingController : Controller
    {
        private readonly HalloDocContext _context;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly IScheduling _IScheduling;
        private readonly IActions _IActions;
        public SchedulingController(IComboBox iComboBox, HalloDocContext iContext, INotyfService iNotyfService, IScheduling iScheduling, IActions iActions)
        {
            _context = iContext;
            _IComboBox = iComboBox;
            _IScheduling = iScheduling;
            _INotyfService = iNotyfService;
            _IActions = iActions;
        }
        #region Index
        public async Task<IActionResult> Index()
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
            SchedulingModel modal = new();
            return View("../AdminPanel/Admin/Scheduling/Index", modal);
        }
        #endregion Index

        #region GetPhysicianByRegion
        public IActionResult GetPhysicianByRegion(int RegionId)
        {
            var PhysiciansByRegion = _IComboBox.ProviderByRegion(RegionId);
            return Json(PhysiciansByRegion);
        }
        #endregion GetPhysicianByRegion

        #region LoadSchedulingPartial
        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.Physicianregions.Include(u => u.Physician).Where(u => u.Regionid == regionid).Select(u => u.Physician).ToList();
            if (regionid == 0)
            {
                physician = _context.Physicians.ToList();
            }

            switch (PartialName)
            {
                case "_DayWise":
                    DayWiseScheduling day = new()
                    {
                        Date = currentDate,
                        Physicians = physician,
                        ShiftDetail = _context.Shiftdetailregions.Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).Where(u => u.Regionid == regionid && u.Isdeleted == new BitArray(new[] { false })).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        day.ShiftDetail = _context.Shiftdetails.Include(u => u.Shift).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new()
                    {
                        Date = currentDate,
                        Physicians = physician,
                        ShiftDetail = _context.Shiftdetailregions.Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).Where(u => u.Regionid == regionid).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        week.ShiftDetail = _context.Shiftdetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new()
                    {
                        Date = currentDate,
                        ShiftDetail = _context.Shiftdetailregions.Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).Where(u => u.Regionid == regionid).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        month.ShiftDetail = _context.Shiftdetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_MonthWise", month);

                default:
                    return PartialView("../AdminPanel/Admin/Scheduling/_DayWise");
            }
        }
        #endregion LoadSchedulingPartial

        #region AddShift
        public IActionResult AddShift(SchedulingModel model)
        {
            string adminId = CV.ID();
            var chk = Request.Form["repeatdays"].ToList();
            _IScheduling.AddShift(model, chk, adminId);
            return RedirectToAction("Index");

        }
        #endregion AddShift

        #region ViewShift
        public SchedulingModel ViewShift(int shiftdetailid)
        {
            SchedulingModel modal = new();
            var shiftdetail = _context.Shiftdetails.FirstOrDefault(u => u.Shiftdetailid == shiftdetailid);

            if (shiftdetail != null)
            {
                _context.Entry(shiftdetail).Reference(s => s.Shift).Query().Include(s => s.Physician).Load();
            }
            modal.RegionId = (int)shiftdetail.Regionid;
            modal.PhysicianName = shiftdetail.Shift.Physician.Firstname + " " + shiftdetail.Shift.Physician.Lastname;
            modal.ModalDate = shiftdetail.Shiftdate.ToString("yyyy-MM-dd");
            modal.StartTime = shiftdetail.Starttime;
            modal.EndTime = shiftdetail.Endtime;
            modal.ShiftDetailId = shiftdetailid;
            return modal;
        }
        #endregion ViewShift

        #region ViewShiftreturn
        public IActionResult ViewShiftreturn(SchedulingModel modal)
        {
            var shiftdetail = _context.Shiftdetails.FirstOrDefault(u => u.Shiftdetailid == modal.ShiftDetailId);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _context.Shiftdetails.Update(shiftdetail);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion ViewShiftreturn

        #region EditShift
        public void EditShiftSave(SchedulingModel modal)
        {
            _IScheduling.EditShiftSave(modal, CV.ID());
        }
        #endregion

        #region DeleteShift
        public IActionResult ViewShiftDelete(SchedulingModel modal)
        {
            _IScheduling.ViewShiftDelete(modal, CV.ID());
            return RedirectToAction("Index");
        }
        #endregion

        #region ProvidersOnCall
        public async Task<IActionResult> ProvidersOnCall(int? regionId)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            List<ProviderModel> v = await _IScheduling.ProviderOnCall(regionId);
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../AdminPanel/Admin/Scheduling/ProvidersOnCall", v);
        }
        #endregion ProvidersOnCall

        #region RequestedShift
        public async Task<IActionResult> RequestedShift(int? regionId)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            List<SchedulingModel> v = await _IScheduling.GetAllNotApprovedShift(regionId);

            return View("../AdminPanel/Admin/Scheduling/ShiftsForReview", v);
        }
        #endregion RequestedShift

        #region _ApprovedShifts

        public async Task<IActionResult> ApprovedShifts(string shiftids)
        {
            if (await _IScheduling.UpdateStatusShift(shiftids, CV.ID()))
            {
                _INotyfService.Success("Shift Approved Successfully.");
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion _ApprovedShifts

        #region _DeleteShiftsReview

        public async Task<IActionResult> DeleteShiftsReview(string shiftids)
        {
            if (await _IScheduling.DeleteShift(shiftids, CV.ID()))
            {
                _INotyfService.Success("Shift Deleted Successfully.");
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion _DeleteShiftsReview
    }
}
