﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using ViewAsPdf = Rotativa.AspNetCore.ViewAsPdf;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ActionsController : Controller
    {
        #region Configuration
        private readonly IActionService _IActionService;
        private readonly IComboBoxService _IComboBoxService;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;
        public ActionsController(IActionService iActionService, IComboBoxService iComboBoxService, INotyfService iNotyfService, ILogger<ActionsController> logger)
        {
            _IActionService = iActionService;
            _IComboBoxService = iComboBoxService;
            _INotyfService = iNotyfService;
            _logger = logger;
        }
        #endregion Configuration

        #region ViewCase
        public async Task<IActionResult> ViewCase(int Id)
        {
            ViewBag.ComboBoxRegion = await _IComboBoxService.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBoxService.ComboBoxCaseReasons();
            ViewCaseModel vcm = _IActionService.GetRequestForViewCase(Id);
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
        }
        #endregion ViewCase

        #region EditCase
        public IActionResult EditCase(ViewCaseModel vcm)
        {
            ViewBag.ComboBoxRegion = _IComboBoxService.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = _IComboBoxService.ComboBoxCaseReasons();
            bool result = _IActionService.EditCase(vcm);
            if (result)
            {
                _INotyfService.Success("Case Information Updated");
                return RedirectToAction("ViewCase", "Actions", new { id = vcm.RequestId });
            }
            else
            {
                _INotyfService.Error("Case Information Not Updated");
                return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
            }
        }
        #endregion EditCase

        #region AssignProvider
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _IActionService.AssignProvider(requestid, ProviderId, Notes))
            {
                _INotyfService.Success("Physician Assigned successfully...");
            }
            else
            {
                _INotyfService.Error("Physician Not Assigned...");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region ProviderbyRegion
        public IActionResult ProviderbyRegion(int? Regionid)
        {
            var data = _IComboBoxService.ProviderByRegion(Regionid);
            return Json(data);
        }
        #endregion ProviderbyRegion

        #region CancelCase
        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {
            bool CancelCase = _IActionService.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _INotyfService.Success("Case Cancelled Successfully");

            }
            else
            {
                _INotyfService.Error("Case Not Cancelled");

            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion CancelCase

        #region BlockCase
        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _IActionService.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                _INotyfService.Success("Case Blocked Successfully");
            }
            else
            {
                _INotyfService.Error("Case Not Blocked");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion BlockCase

        #region ClearCase
        public IActionResult ClearCase(int RequestID)
        {
            bool ClearCase = _IActionService.ClearCase(RequestID);
            if (ClearCase)
            {
                _INotyfService.Success("Cleared Case Successfully");
            }
            else
            {
                _INotyfService.Error("Case Not Cleared");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region TransferPhysician
        public async Task<IActionResult> TransferPhysician(int requestid, int ProviderId, string Notes)
        {
            if (await _IActionService.TransferPhysician(requestid, ProviderId, Notes))
            {
                _INotyfService.Success("Physician Transferred Successfully.");
            }
            else
            {
                _INotyfService.Error("Physician Not Transferred.");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion TransferPhysician

        #region View_Notes
        public IActionResult ViewNotes(int id)
        {
            ViewNotesModel vnm = _IActionService.getNotes(id);
            return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml", vnm);
        }
        #endregion

        #region Edit_Notes
        public IActionResult EditViewNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _IActionService.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _INotyfService.Success("Notes Updated successfully");
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    _INotyfService.Error("Notes Not Updated");
                    return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml");
                }
            }
            else
            {
                _INotyfService.Information("Only Physician notes and Admin notes can be updated, Please Select one of them!!");
                TempData["Errormassage"] = "Only Physician notes and Admin notes can be updated, Please Select one of them!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
        }
        #endregion

        #region ViewDocuments
        public async Task<IActionResult> ViewDocuments(int? id, ViewUploadModel model)
        {
            ViewUploadModel vum = await _IActionService.GetDocument(id, model);
            return View("~/Views/AdminPanel/Actions/ViewUploads.cshtml", vum);
        }
        #endregion

        #region UploadDocuments
        public IActionResult UploadDocuments(int Requestid, IFormFile file)
        {
            if (_IActionService.UploadDocuments(Requestid, file))
            {
                _INotyfService.Success("File Uploaded Successfully");
            }
            else
            {
                _INotyfService.Error("File mot uploaded");
            }
            return RedirectToAction("ViewDocuments", "Actions", new { id = Requestid });
        }
        #endregion

        #region DeleteFile
        public async Task<IActionResult> DeleteFile(int? id, int Requestid)
        {
            if (await _IActionService.DeleteDocuments(id.ToString()))
            {
                _INotyfService.Success("File Deleted Successfully");
            }
            else
            {
                _INotyfService.Error("File Not deleted");
            }
            return RedirectToAction("ViewDocuments", "Actions", new { id = Requestid });
        }
        #endregion

        #region DeleteAllFiles
        public async Task<IActionResult> DeleteAllFiles(string deleteids, int Requestid)
        {
            if (await _IActionService.DeleteDocuments(deleteids))
            {
                _INotyfService.Success("All Files have been deleted successfully");
            }
            else
            {
                _INotyfService.Error("All Files have not been deleted successfully");
            }
            return RedirectToAction("ViewDocuments", "Actions", new { id = Requestid });
        }
        #endregion

        #region SendOrder

        public async Task<IActionResult> Orders(int id)
        {
            List<ComboBoxHealthProfessionalType> hpt = await _IComboBoxService.ComboBoxHealthProfessionalType();
            ViewBag.ProfessionType = hpt;
            SendOrderModel data = new SendOrderModel
            {
                RequestID = id
            };
            return View("~/Views/AdminPanel/Actions/SendOrders.cshtml", data);
        }

        public Task<IActionResult> ProfessionByType(int HealthProfessionId)
        {
            var v = _IComboBoxService.ProfessionByType(HealthProfessionId);
            return Task.FromResult<IActionResult>(Json(v));
        }

        public Task<IActionResult> SelectProfessionalById(int VendorId)
        {
            var v = _IActionService.SelectProfessionalById(VendorId);
            return Task.FromResult<IActionResult>(Json(v));
        }
        public IActionResult SendOrders(SendOrderModel som)
        {
            if (ModelState.IsValid)
            {
                bool data = _IActionService.SendOrders(som);
                if (data)
                {
                    _INotyfService.Success("Order Created  successfully...");
                    //_INotyfService.Information("Mail is sended to Vendor successfully...");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _INotyfService.Error("Order Not Created...");
                    return View("~/Views/AdminPanel/Actions/SendOrders.cshtml", som);
                }
            }
            else
            {
                return View("~/Views/AdminPanel/Actions/SendOrders.cshtml", som);
            }
        }
        #endregion SendOrder

        #region SendAgreement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAgreementMail(int RequestId)
        {
            if (_IActionService.SendAgreement(RequestId))
            {
                _INotyfService.Success("Mail sent successfully");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region CloseCase
        public async Task<IActionResult> CloseCase_CC(int RequestID)
        {
            CloseCaseModel ccm = _IActionService.GetRequestForCloseCase(RequestID);
            return View("~/Views/AdminPanel/Actions/CloseCase.cshtml", ccm);
        }
        public IActionResult CloseCaseUnpaid(int Id)
        {
            bool ccu = _IActionService.CloseCase(Id);
            if (ccu)
            {
                _INotyfService.Success("Closed case");
                _INotyfService.Information("You can see closed case in unpaid state");
            }
            else
            {
                _INotyfService.Error("Case not closed");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult EditCloseCase(CloseCaseModel ccu)
        {
            bool result = _IActionService.EditCloseCase(ccu);
            if (result)
            {
                _INotyfService.Success("Case Edited Successfully");
                return RedirectToAction("CloseCase_CC", new { ccu.RequestID });
            }
            else
            {
                _INotyfService.Error("Case Not Edited");
                return RedirectToAction("CloseCase_CC", new { ccu.RequestID });

            }
        }
        #endregion

        #region GetEncounterData
        public IActionResult Encounter(int id)
        {
            EncounterModel efm = _IActionService.GetEncounterData(id);
            return View("~/Views/AdminPanel/Actions/EncounterForm.cshtml", efm);
        }
        #endregion

        #region EditEncounterData
        public IActionResult EditEncounterData(EncounterModel model)
        {
            if (_IActionService.EditEncounterData(model, CV.ID()))
            {
                _INotyfService.Success("Encounter Changes Saved.");
            }
            else
            {
                _INotyfService.Error("Encounter data remains unchanged.");
            }
            return RedirectToAction("Encounter", new { id = model.RequesId });
        }
        #endregion

        #region SendLink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> SendLink(string FirstName, string LastName, string Email, string PhoneNumber)
        {
            if (_IActionService.SendLink(FirstName, LastName, Email, PhoneNumber))
            {
                _INotyfService.Success("Mail Sent Successfully.");
            }
            return Task.FromResult<IActionResult>(RedirectToAction("Index", "Dashboard"));
        }
        #endregion

        #region AcceptRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int RequestId, string Note)
        {
            if (await _IActionService.AcceptPhysician(RequestId, Note, Convert.ToInt32(CV.UserID())))
            {
                _INotyfService.Success("Case Accepted.");
            }
            else
            {
                _INotyfService.Success("Case Not Accepted.");
            }
            return Redirect("~/Provider/Dashboard");
        }
        #endregion AcceptRequest

        #region TransferToAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferToAdmin(int RequestID, string Note)
        {
            if (await _IActionService.TransfertoAdmin(RequestID, Note, Convert.ToInt32(CV.UserID())))
            {
                _INotyfService.Success("Request Successfully transferred to admin.");
            }

            return Redirect("~/Provider/Dashboard");
        }
        #endregion TransferToAdmin

        #region Finalize
        public IActionResult Finalize(EncounterModel model)
        {
            bool data = _IActionService.EditEncounterData(model, CV.ID());
            if (data)
            {
                bool final = _IActionService.CaseFinalized(model);
                if (final)
                {
                    _INotyfService.Success("Case Is Finalized...");
                    if (CV.role() == "Provider")
                    {
                        return RedirectToAction("Index", "DashBoard");
                    }
                    return RedirectToAction("Index", "AdminDashBoard");
                }
                else
                {
                    _INotyfService.Error("Case Is Not Finalized Please Enter Valid Data...");
                    return RedirectToAction("Encounter", new { id = model.RequesId });
                }
            }
            else
            {
                _INotyfService.Error("Case Is Not Finalized...");
                return RedirectToAction("Encounter", new { id = model.RequesId });
            }

        }
        #endregion

        #region Housecall
        public IActionResult Housecall(int RequestId)
        {
            if (_IActionService.Housecall(RequestId))
            {
                _INotyfService.Success("Case Accepted...");
            }
            else
            {
                _INotyfService.Error("Case Not Accepted...");
            }
            return Redirect("~/Provider/Dashboard");
        }
        #endregion

        #region Consult
        public IActionResult Consult(int RequestId)
        {
            if (_IActionService.Consult(RequestId))
            {
                _INotyfService.Success("Case is in conclude state...");
            }
            else
            {
                _INotyfService.Error("Error...");
            }
            return Redirect("~/Provider/Dashboard");
        }
        #endregion

        #region GeneratePDF
        public IActionResult GeneratePDF(int id)
        {
            var FormDetails = _IActionService.GetEncounterData(id);
            return new ViewAsPdf("../AdminPanel/Actions/EncounterPdf", FormDetails);
        }
        #endregion

        #region ConcludeCare
        public async Task<IActionResult> ConcludeCare(int? id, ViewUploadModel viewDocument)
        {
            id ??= viewDocument.RequestId;
            ViewUploadModel v = await _IActionService.GetDocument(id, viewDocument);
            return View("../AdminPanel/Actions/ConcludeCare", v);
        }
        #endregion ConcludeCare

        #region UploadDocProvider
        public IActionResult UploadDocProvider(int Requestid, IFormFile file)
        {
            if (_IActionService.UploadDocuments(Requestid, file))
            {
                _INotyfService.Success("File Uploaded Successfully");
            }
            else
            {
                _INotyfService.Error("File Not Uploaded");
            }
            return RedirectToAction("ConcludeCare", "Actions", new { id = Requestid });
        }
        #endregion UploadDocProvider

        #region ConcludeCarePost
        public IActionResult ConcludeCarePost(int RequestId, string Notes)
        {
            if (_IActionService.ConcludeCare(RequestId, Notes))
            {
                _INotyfService.Success("Case concluded...");
            }
            else
            {
                _INotyfService.Error("Error...");
            }
            return Redirect("~/Provider/Dashboard");
        }
        #endregion ConcludeCarePost
    }
}