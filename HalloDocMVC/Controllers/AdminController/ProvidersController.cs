using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin,Provider")]
    public class ProvidersController : Controller
    {
        #region Configuration
        private readonly IAdminDashboardService _IAdminDashboardService;
        private readonly IProviderService _IProviderService;
        private readonly IComboBoxService _IComboBoxService;
        private readonly INotyfService _INotyfService;
        private readonly EmailConfiguration _emailConfiguration;
        public ProvidersController(IAdminDashboardService iAdminDashboardService, IProviderService iProviderService, IComboBoxService iComboBoxService, INotyfService iNotyfService, EmailConfiguration emailConfiguration)
        {
            _IAdminDashboardService = iAdminDashboardService;
            _IProviderService = iProviderService;
            _IComboBoxService = iComboBoxService;
            _INotyfService = iNotyfService;
            _emailConfiguration = emailConfiguration;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? region, PaginationProvider paginationProvider)
        {
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            PaginationProvider data = _IProviderService.GetContacts(paginationProvider);
            if (region == null)
            {
                data = _IProviderService.GetContacts(paginationProvider);
            }
            else
            {
                data = _IProviderService.PhysicianByRegion(region, paginationProvider);
            }
            return View("~/Views/AdminPanel/Admin/Provider/Index.cshtml", data);
        }
        #endregion

        #region ChangeNotification
        public async Task<IActionResult> ChangeNotification(string changedValues)
        {
            Dictionary<int, bool> changeValueDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _IProviderService.ChangeNotification(changeValueDict);
            return RedirectToAction("Index");
        }
        #endregion

        #region SendMessage
        public async Task<IActionResult> SendMessage(string? email, string? contact, int? communicate, string? message)
        {
            bool result = false, sms = false;
            if (communicate == 1)
            {
                sms = SendSMS(contact, message).Result;
            }
            else if (communicate == 2)
            {
                result = await _emailConfiguration.SendMail(email, "Check Message", "Hello " + message);
            }
            else
            {
                result = await _emailConfiguration.SendMail(email, "Check Message", "Hello " + message);
                sms = SendSMS(contact, message).Result;
            }
            if (result)
            {
                _INotyfService.Success("Email sent Successfully.");
            }
            if (sms)
            {
                _INotyfService.Success("Message sent Successfully.");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region AddPhysician
        [HttpPost]
        public async Task<IActionResult> AddPhysician(ProviderModel Physicians)
        {
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            ViewBag.UserRoleComboBox = await _IComboBoxService.PhysicianRoleComboBox();
            if (await _IProviderService.AddPhysician(Physicians, CV.ID()))
            {
                _INotyfService.Success("Physician added Successfully..!");
            }
            else
            {
                _INotyfService.Error("Physician not added Successfully..!");
            }
            return RedirectToAction("Index");
        }
        #endregion

        /*[Route("Provider/Profile")]
        [Route("Admin/Profile")]*/
        #region PhysicianProfile
        public async Task<IActionResult> PhysicianProfile(int? Id)
        {
            ViewBag.RegionComboBox = await _IComboBoxService.ComboBoxRegions();
            ViewBag.UserRoleComboBox = await _IComboBoxService.PhysicianRoleComboBox();
            if (Id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                ProviderModel pvm = await _IProviderService.GetPhysicianById((int)Id);
                return View("~/Views/AdminPanel/Admin/Provider/EditPhysician.cshtml", pvm);
            }
            return View("~/Views/AdminPanel/Admin/Provider/EditPhysician.cshtml");
        }
        #endregion

        #region EditAccountInfo
        public async Task<IActionResult> EditAccountInfo(ProviderModel data)
        {
            string actionName = RouteData.Values["action"].ToString();
            string actionNameaq = ControllerContext.ActionDescriptor.ActionName; // Get the current action name
            if (await _IProviderService.EditAccountInfo(data))
            {
                _INotyfService.Success("Account Information Changed Successfully..!");
            }
            else
            {
                _INotyfService.Error("Account Information not Changed Successfully..!");
            }
            return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
        }
        #endregion

        #region ResetPassword
        public async Task<IActionResult> ResetPassword(string password, int PhysicianId)
        {
            if (await _IProviderService.ResetPassword(password, PhysicianId))
            {
                _INotyfService.Success("Password Updated Successfully");
            }
            else
            {
                _INotyfService.Error("Password Not Updated");
            }
            return RedirectToAction("PhysicianProfile", new { id = PhysicianId });
        }
        #endregion

        #region EditPhysicianInfo
        public async Task<IActionResult> EditPhysicianInfo(ProviderModel data)
        {
            if (await _IProviderService.EditPhysicianInfo(data))
            {
                _INotyfService.Success("Administrator Information Changed Successfully..!");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("Administrator Information not Changed Successfully..!");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
        }
        #endregion

        #region EditMailBillingInfo
        public async Task<IActionResult> EditMailingInfo(ProviderModel data)
        {
            if (await _IProviderService.EditMailBillingInfo(data, CV.ID()))
            {
                _INotyfService.Success("mail and billing Information Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("mail and billing Information not Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
        }
        #endregion

        #region EditProviderProfile
        public async Task<IActionResult> EditProviderProfile(ProviderModel data)
        {
            if (await _IProviderService.EditProviderProfile(data, CV.ID()))
            {
                _INotyfService.Success("Profile Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("Profile not Changed");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
        }
        #endregion

        #region EditProviderOnbording
        public async Task<IActionResult> EditProviderOnboarding(ProviderModel data)
        {
            if (await _IProviderService.EditProviderOnbording(data, CV.ID()))
            {
                _INotyfService.Success("Provider Onboarding Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("Provider Onboarding not Changed");
                return RedirectToAction("PhysicianProfile", new { id = data.PhysicianId });
            }
        }
        #endregion

        #region SMS
        public async Task<bool> SendSMS(string receiverPhoneNumber, string message)
        {
            string accountSid = "AC69467c819de80787766fec56b90cc459";
            string authToken = "47d40c1a322599e9e01db9d9a59232ae";
            string twilioPhoneNumber = "+12514188182";

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var smsMessage = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(receiverPhoneNumber)
                );

                Console.WriteLine("SMS sent successfully. SID: " + smsMessage.Sid);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the SMS: " + ex.Message);
            }
            return false;
        }
        #endregion SMS

        #region RequestToAdmin
        public IActionResult RequestToAdmin(string Note)
        {
            bool Contact = _IProviderService.RequestToAdmin(Convert.ToInt32(CV.UserID()), Note);
            if (Contact)
            {
                _INotyfService.Success("Mail Sent Succesfully.");
            }
            else
            {
                _INotyfService.Error("Mail did not Send");
            }
            return RedirectToAction("PhysicianProfile", "Providers", new { id = Convert.ToInt32(CV.UserID()) });
        }
        #endregion RequestToAdmin
    }
} 
             