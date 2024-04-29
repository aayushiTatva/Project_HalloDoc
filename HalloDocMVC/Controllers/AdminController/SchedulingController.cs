using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Controllers.AdminController
{
    public class SchedulingController : Controller
    {
        private readonly IComboBoxService _IComboBoxService;
        private readonly INotyfService _INotyfService;
        private readonly ISchedulingService _ISchedulingService;
        private readonly IActionService _IActionService;
        private readonly IGenericRepository<Physicianregion> _physicianRegionRepository;
        private readonly IGenericRepository<Shiftdetail> _shiftDetailRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Shiftdetailregion> _shiftDetailRegionRepository;
        public SchedulingController(IComboBoxService iComboBoxService, INotyfService iNotyfService, ISchedulingService iSchedulingService, IActionService iActionService,
            IGenericRepository<Physicianregion> physicianRegionRepository, IGenericRepository<Shiftdetail> shiftDetailRepository, IGenericRepository<Physician> physicianRepository
            , IGenericRepository<Shiftdetailregion> shiftDetailRegionRepository)
        {
            _IComboBoxService = iComboBoxService;
            _ISchedulingService = iSchedulingService;
            _INotyfService = iNotyfService;
            _IActionService = iActionService;
            _physicianRegionRepository = physicianRegionRepository;
            _shiftDetailRepository = shiftDetailRepository;
            _physicianRepository = physicianRepository;
            _shiftDetailRegionRepository = shiftDetailRegionRepository;
        }
        #region Index
        public async Task<IActionResult> Index()
        {
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
            SchedulingModel modal = new();
            return View("../AdminPanel/Admin/Scheduling/Index", modal);
        }
        #endregion Index

        #region GetPhysicianByRegion
        public IActionResult GetPhysicianByRegion(int RegionId)
        {
            var PhysiciansByRegion = _IComboBoxService.ProviderByRegion(RegionId);
            return Json(PhysiciansByRegion);
        }
        #endregion GetPhysicianByRegion

        #region LoadSchedulingPartial
        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _physicianRegionRepository.GetAll().Include(u => u.Physician).Where(u => u.Regionid == regionid).Select(u => u.Physician).ToList();
            /*if (regionid == 0)
            {
                physician = _physicianRepository.ToList();
            }*/

            switch (PartialName)
            {
                case "_DayWise":
                    DayWiseScheduling day = new()
                    {
                        Date = currentDate,
                        Physicians = physician,
                        ShiftDetail = _shiftDetailRegionRepository.GetAll().Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).Where(u => u.Regionid == regionid && u.Isdeleted == new BitArray(new[] { false })).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        day.ShiftDetail = _shiftDetailRepository.GetAll().Include(u => u.Shift).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new()
                    {
                        Date = currentDate,
                        Physicians = physician,
                        ShiftDetail = _shiftDetailRegionRepository.GetAll().Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).Where(u => u.Regionid == regionid).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        week.ShiftDetail = _shiftDetailRepository.GetAll().Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new()
                    {
                        Date = currentDate,
                        ShiftDetail = _shiftDetailRegionRepository.GetAll().Include(u => u.Shiftdetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).Where(u => u.Regionid == regionid).Select(u => u.Shiftdetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        month.ShiftDetail = _shiftDetailRepository.GetAll().Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.Isdeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../AdminPanel/Admin/Scheduling/_MonthWise", month);

                default:
                    return PartialView("../AdminPanel/Admin/Scheduling/_DayWise");
            }
        }
        #endregion LoadSchedulingPartial

        #region LoadSchedulingPartialProvider
        public IActionResult LoadSchedulingPartialProvider(string date)
        {
            var currentDate = DateTime.Parse(date);
            MonthWiseScheduling month = new()
            {
                Date = currentDate,
                ShiftDetail = _shiftDetailRepository.GetAll().Include(u => u.Shift).Where(u => u.Isdeleted == new BitArray(new[] { false }) && u.Shift.Physicianid == Int32.Parse(CV.UserID())).ToList()
            };
            return PartialView("../AdminPanel/Admin/Scheduling/_MonthWise", month);
        }
        #endregion

        #region AddShift
        public IActionResult AddShift(SchedulingModel model)
        {
            string adminId = CV.ID();
            var chk = Request.Form["repeatdays"].ToList();
            _ISchedulingService.AddShift(model, chk, adminId);
            return RedirectToAction("Index");

        }
        #endregion AddShift

        #region ViewShift
        public SchedulingModel ViewShift(int shiftdetailid)
        {
            SchedulingModel modal = new();
            var shiftdetail = _shiftDetailRepository.GetAll().FirstOrDefault(u => u.Shiftdetailid == shiftdetailid);

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
            var shiftdetail = _shiftDetailRepository.GetAll().FirstOrDefault(u => u.Shiftdetailid == modal.ShiftDetailId);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _shiftDetailRepository.Update(shiftdetail);

            return RedirectToAction("Index");
        }
        #endregion ViewShiftreturn

        #region EditShift
        public void EditShiftSave(SchedulingModel modal)
        {
            _ISchedulingService.EditShiftSave(modal, CV.ID());
        }
        #endregion

        #region DeleteShift
        public IActionResult ViewShiftDelete(SchedulingModel modal)
        {
            _ISchedulingService.ViewShiftDelete(modal, CV.ID());
            return RedirectToAction("Index");
        }
        #endregion

        #region ProvidersOnCall
        public async Task<IActionResult> ProvidersOnCall(int? regionId)
        {
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            List<ProviderModel> v = await _ISchedulingService.ProviderOnCall(regionId);
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
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            List<SchedulingModel> v = await _ISchedulingService.GetAllNotApprovedShift(regionId);

            return View("../AdminPanel/Admin/Scheduling/ShiftsForReview", v);
        }
        #endregion RequestedShift

        #region _ApprovedShifts

        public async Task<IActionResult> ApprovedShifts(string shiftids)
        {
            if (await _ISchedulingService.UpdateStatusShift(shiftids, CV.ID()))
            {
                _INotyfService.Success("Shift Approved Successfully.");
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion _ApprovedShifts

        #region _DeleteShiftsReview

        public async Task<IActionResult> DeleteShiftsReview(string shiftids)
        {
            if (await _ISchedulingService.DeleteShift(shiftids, CV.ID()))
            {
                _INotyfService.Success("Shift Deleted Successfully.");
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion _DeleteShiftsReview
    }
}
