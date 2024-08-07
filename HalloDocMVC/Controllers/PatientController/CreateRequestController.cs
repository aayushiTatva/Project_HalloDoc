﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DataModels;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.PatientController
{
    public class CreateRequestController : Controller
    {
        #region Configuration
        private readonly ICreateRequestService _ICreateRequestService;
        private readonly INotyfService _INotyfService;
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly HalloDocContext _context;
        public CreateRequestController(ICreateRequestService iCreateRequestService, INotyfService iNotyfService, IGenericRepository<Aspnetuser> aspNetUserRepository, HalloDocContext context)
        {
            _ICreateRequestService = iCreateRequestService;
            _INotyfService = iNotyfService;
            _aspNetUserRepository = aspNetUserRepository;
            _context = context;
        }
        #endregion Configuration

        #region Index
        public IActionResult Index()
        {
            return View("~/Views/PatientPanel/CreateRequest/SubmitRequestPage.cshtml");
        }
        #endregion

        #region CheckEmail

        [HttpPost]
        public async Task<IActionResult> CheckEmail(string email)
        {
            string message;
            var aspnetuser = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == email);
            if (aspnetuser == null)
            {
                message = "False";
            }
            else
            {
                message = "Success";
            }
            return Json(new
            {
                isAspnetuser = aspnetuser == null
            });
        }
        #endregion

        #region PatientRequest
        public IActionResult CreatePatientRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/PatientRequestPage.cshtml");
        }
        public async Task<IActionResult> PatientRequest(ViewDataPatientRequestModel model)
        {
            if (await _ICreateRequestService.CreatePatientRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion

        #region FamilyRequest
        public IActionResult CreateFamilyRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/FamilyRequestPage.cshtml");
        }
        public async Task<IActionResult> FamilyRequest(ViewDataFamilyRequestModel model)
        {
            if (await _ICreateRequestService.CreateFamilyRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion

        #region ConciregeRequest
        public IActionResult CreateConciergeRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/ConciergeRequestPage.cshtml");
        }
        public async Task<IActionResult> ConciergeRequest(ViewDataConciergeRequestModel model)
        {
            if (await _ICreateRequestService.CreateConciergeRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion

        #region BusinessRequest
        public IActionResult CreateBusinessRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/BusinessRequestPage.cshtml");
        }
        public async Task<IActionResult> BusinessRequest(ViewDataBusinessRequestModel model)
        {
            if (await _ICreateRequestService.CreateBusinessRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }

        #endregion
    }
}
