using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Models;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HalloDocMVC.Controllers.PatientController
{
    public class PatientDashboardController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IPatientDashboard _IPatientDashboard;
        private readonly INotyfService _INotyfService;
        public PatientDashboardController(HalloDocContext context, IPatientDashboard iPatientDashboard, INotyfService iNotyfService)
        {
            _context = context;
            _IPatientDashboard = iPatientDashboard;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration
        #region Dashboard
        public async Task<IActionResult> Index()
        {
            if (CV.UserID() != null)
            {
                var UserIDForRequest = _context.Users.Where(r => r.Userid == Convert.ToInt32(CV.UserID())).FirstOrDefault();

                if (UserIDForRequest != null)
                {
                    List<DBEntity.DataModels.Request> Request = _context.Requests.Where(r => r.Userid == UserIDForRequest.Userid).ToList();
                    List<int> ids = new();
                    foreach (var request in Request)
                    {

                        var doc = _context.Requestwisefiles.Where(r => r.Requestid == request.Requestid).FirstOrDefault();
                        if (doc != null)
                        {
                            ids.Add(request.Requestid);
                        }
                    }
                    ViewBag.docidlist = ids;
                    ViewBag.list = Request;

                    // Get the list of documents for each request
                    var docList = _context.Requestwisefiles.ToList();

                    // Group the documents by request id and count them
                    var docCount = docList.GroupBy(d => d.Requestid)
                                          .ToDictionary(g => g.Key, g => g.Count());

                    // Store the dictionary in the ViewBag
                    ViewBag.docCount = docCount;

                }
                return View("~/Views/PatientPanel/Dashboard/PatientDashboard.cshtml");
            }
            else
            {
                return View("../Login/Index");
            }

        }
        #endregion Dashboard

        #region ViewDocuments
        public IActionResult ViewDocuments(int? id)
        {
            List<DBEntity.DataModels.Request> Request = _context.Requests.Where(r => r.Requestid == id).ToList();
            ViewBag.requestinfo = Request;
            List<Requestwisefile> DocList = _context.Requestwisefiles.Where(r => r.Requestid == id).ToList();
            ViewBag.DocList = DocList;
            return View("~/Views/PatientPanel/Dashboard/ViewDocuments.cshtml");
        }
        #endregion ViewDocuments

        #region UploadDocuments
        public async Task<IActionResult> UploadDoc(int RequestId, IFormFile? UploadFile)
        {
            if (await _IPatientDashboard.UploadDoc(RequestId, UploadFile))
            {
                _INotyfService.Success("File has been Uploaded Successfully.");
            }
            else
            {
                _INotyfService.Error("File has not been uploaded");
            }
            return RedirectToAction("ViewDocuments", "PatientDashboard", new { id = RequestId });
        }
        #endregion UploadDocuments

        #region RequestForMe
        public IActionResult RequestForMe()
        {
            ViewDataPatientRequestModel model = _IPatientDashboard.RequestForMe();
            return View("~/Views/PatientPanel/Dashboard/RequestForMe.cshtml", model);
        }
        #endregion RequestForMe

        #region CreateRequestForMe
        public async Task<IActionResult> PostMe(ViewDataPatientRequestModel model)
        {
            if (await _IPatientDashboard.PostMe(model))
            {
                _INotyfService.Success("Request has been created successfully");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully");
            }
            return RedirectToAction("Index", "PatientDashboard");
        }
        #endregion CreateRequestForMe

        #region RequestForSomeoneElse
        public IActionResult RequestForSomeoneElse()
        {
            return View("~/Views/PatientPanel/Dashboard/RequestForSomeoneElse.cshtml");
        }
        #endregion RequestForSomeoneElse

        #region CreateRequestForSomeoneElse
        public async Task<IActionResult> PostSomeoneElse(ViewDataPatientRequestModel model)
        {
            if (await _IPatientDashboard.PostSomeoneElse(model))
            {
                _INotyfService.Success("Request has been created successfully");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully");
            }
            return RedirectToAction("Index", "PatientDashboard");
        }
        #endregion CreateRequestForSomeoneElse
    }
}
