using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class ProvidersController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IAdminDashboard _IAdminDashboard;
        private readonly IContactYourProvider _IContactYourProvider;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly EmailConfiguration _emailConfiguration;
        public ProvidersController(HalloDocContext context,IAdminDashboard iAdminDashboard,IContactYourProvider iContactYourProvider, IComboBox iComboBox, INotyfService iNotyfService,EmailConfiguration emailConfiguration)
        {
            _context = context;
            _IAdminDashboard = iAdminDashboard;
            _IContactYourProvider = iContactYourProvider;
            _IComboBox = iComboBox;
            _INotyfService = iNotyfService;
            _emailConfiguration = emailConfiguration;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? region,PaginationProvider paginationProvider)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            PaginationProvider data = _IContactYourProvider.GetContacts(paginationProvider);
            if (region == null)
            {
                data = _IContactYourProvider.GetContacts(paginationProvider);
            }
            else
            {
                data = _IContactYourProvider.PhysicianByRegion(region, paginationProvider);
            }
            return View("~/Views/AdminPanel/Admin/Provider/Index.cshtml", data);
        }
        #endregion
        #region ChangeNotification
        public async Task<IActionResult> ChangeNotification(string changedValues)
        {
            Dictionary<int, bool> changeValueDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _IContactYourProvider.ChangeNotification(changeValueDict);
            return RedirectToAction("Index");
        }
        #endregion

        #region SendMessage
        public async Task<IActionResult> SendMessage(string? email, string? contact, int? way, string? message)
        {
            bool result = false, sms = false;
            if (way == 1)
            {
                sms = SendSMS(contact, message).Result;
            }
            else if (way == 2)
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
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.UserRoleComboBox = await _IComboBox.PhysicianRoleComboBox();
            if (await _IContactYourProvider.AddPhysician(Physicians, CV.ID()))
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

        #region PhysicianProfile
        public async Task<IActionResult> PhysicianProfile(int? Id)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.UserRoleComboBox = await _IComboBox.PhysicianRoleComboBox();
            if (Id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                ProviderModel pvm = await _IContactYourProvider.GetPhysicianById((int) Id);
                return View("~/Views/AdminPanel/Admin/Provider/EditPhysician.cshtml",pvm);
            }
            return View("~/Views/AdminPanel/Admin/Provider/EditPhysician.cshtml");
        }
        #endregion

        #region EditAccountInfo
        public async Task<IActionResult> EditAccountInfo(ProviderModel data)
        {
            string actionName = RouteData.Values["action"].ToString();
            string actionNameaq = ControllerContext.ActionDescriptor.ActionName; // Get the current action name
            if (await _IContactYourProvider.EditAccountInfo(data))
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
        #region EditPhysicianInfo
        public async Task<IActionResult> EditPhysicianInfo(ProviderModel data)
        {
            if (await _IContactYourProvider.EditPhysicianInfo(data))
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
            if (await _IContactYourProvider.EditMailBillingInfo(data, CV.ID()))
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
            if (await _IContactYourProvider.EditProviderProfile(data, CV.ID()))
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
            if (await _IContactYourProvider.EditProviderOnbording(data, CV.ID()))
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
            string authToken = "e246e88eac98dd0c034be9b8b1f1a738";
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
    }
} 
             