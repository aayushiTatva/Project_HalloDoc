using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class CreateRequestService : ICreateRequestService
    {
        #region Configuration
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<Requestclient> _requestClientRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Requestwisefile> _requestWiseFileRepository;
        private readonly IGenericRepository<Business> _businessRepository;
        private readonly IGenericRepository<Requestbusiness> _requestBusinessRepository;
        private readonly IGenericRepository<Concierge> _conciergeRepository;
        private readonly IGenericRepository<Requestconcierge> _requestConciergeRepository;
        private readonly IConfirmationNumberService _confirmationNumberService;
        private readonly EmailConfiguration _emailConfiguration;

        public CreateRequestService(IGenericRepository<Aspnetuser> aspNetUserRepository, IGenericRepository<Request> requestRepository, IGenericRepository<Requestclient> requestClientRepository, IGenericRepository<User> userRepository, IGenericRepository<Requestwisefile> requestWiseFileRepository, IGenericRepository<Business> businessRepository, IGenericRepository<Requestbusiness> requestBusinessRepository, IGenericRepository<Concierge> conciergeRepository, IGenericRepository<Requestconcierge> requestConciergeRepository, IConfirmationNumberService confirmationNumberService, EmailConfiguration emailConfiguration)
        {
            _aspNetUserRepository = aspNetUserRepository;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _userRepository = userRepository;
            _requestWiseFileRepository = requestWiseFileRepository;
            _businessRepository = businessRepository;
            _requestBusinessRepository = requestBusinessRepository;
            _conciergeRepository = conciergeRepository;
            _requestConciergeRepository = requestConciergeRepository;
            _confirmationNumberService = confirmationNumberService;
            _emailConfiguration = emailConfiguration;
        }


        #endregion
        #region PatientRequest
        public async Task<bool> CreatePatientRequest(ViewDataPatientRequestModel viewDataPatientRequest)
        {
            /*if(ModelState.IsValid) 
            {*/
            var Aspnetuser = new Aspnetuser();
            var User = new User();
            var Request = new Request();
            var Requestclient = new Requestclient();
            var isexist = _userRepository.GetAll().FirstOrDefault(x => x.Email == viewDataPatientRequest.Email);
            var hasher = new PasswordHasher<String>();
            if (isexist == null)
            {
                // Aspnetuser
                Guid g = Guid.NewGuid();
                Aspnetuser.Id = g.ToString();
                Aspnetuser.Username = viewDataPatientRequest.FirstName;
                Aspnetuser.Passwordhash = hasher.HashPassword(null, viewDataPatientRequest.PassWord);
                Aspnetuser.CreatedDate = DateTime.Now;
                Aspnetuser.Email = viewDataPatientRequest.Email;
                Aspnetuser.Phonenumber = viewDataPatientRequest.PhoneNumber;
                _aspNetUserRepository.Add(Aspnetuser);

                User.Aspnetuserid = Aspnetuser.Id;
                User.Firstname = viewDataPatientRequest.FirstName;
                User.Lastname = viewDataPatientRequest.LastName;
                User.Email = viewDataPatientRequest.Email;
                User.Mobile = viewDataPatientRequest.PhoneNumber;
                User.Intdate = viewDataPatientRequest.DateOfBirth.Day;
                User.Strmonth = viewDataPatientRequest.DateOfBirth.ToString("MMMM");
                User.Intyear = viewDataPatientRequest.DateOfBirth.Year;
                User.Street = viewDataPatientRequest.Street;
                User.State = viewDataPatientRequest.State;
                User.City = viewDataPatientRequest.City;
                User.Zipcode = viewDataPatientRequest.ZipCode;
                User.Regionid = 1;
                User.Createdby = Aspnetuser.Id;
                User.Isdeleted = new BitArray(1);
                User.Createddate = DateTime.Now;
                _userRepository.Add(User);
            }

            Request.Requesttypeid = 2;
            Request.Status = 1;

            if (isexist == null)
            {
                Request.Userid = User.Userid;
            }
            else
            {
                Request.Userid = isexist.Userid;
            }
            Request.Firstname = viewDataPatientRequest.FirstName;
            Request.Lastname = viewDataPatientRequest.LastName;
            Request.Email = viewDataPatientRequest.Email;
            Request.Phonenumber = viewDataPatientRequest.PhoneNumber;
            Request.Confirmationnumber = _confirmationNumberService.GetConfirmationNumber(viewDataPatientRequest.State, viewDataPatientRequest.FirstName, viewDataPatientRequest.LastName);
            Request.Isurgentemailsent = new BitArray(1);
            Request.Isdeleted = new BitArray(1);
            Request.Createddate = DateTime.Now;
            _requestRepository.Add(Request);

            Requestclient.Requestid = Request.Requestid;
            Requestclient.Firstname = viewDataPatientRequest.FirstName;
            Requestclient.Address = viewDataPatientRequest.Street;
            Requestclient.Lastname = viewDataPatientRequest.LastName;
            Requestclient.Email = viewDataPatientRequest.Email;
            Requestclient.Phonenumber = viewDataPatientRequest.PhoneNumber;
            Requestclient.Notes = viewDataPatientRequest.Symptoms;
            Requestclient.Intdate = viewDataPatientRequest.DateOfBirth.Day;
            Requestclient.Strmonth = viewDataPatientRequest.DateOfBirth.ToString("MMMM");
            Requestclient.Intyear = viewDataPatientRequest.DateOfBirth.Year;
            Requestclient.Street = viewDataPatientRequest.Street;
            Requestclient.State = viewDataPatientRequest.State;
            Requestclient.City = viewDataPatientRequest.City;
            Requestclient.Zipcode = viewDataPatientRequest.ZipCode;
            Requestclient.Regionid = 1;

            _requestClientRepository.Add(Requestclient);

            if (viewDataPatientRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(viewDataPatientRequest.UploadFile, Request.Requestid);

                var requestwisefile = new Requestwisefile()
                {
                    Requestid = Request.Requestid,
                    Filename = upload,
                    Createddate = DateTime.Now,
                    Isdeleted = new BitArray(1)
                };
                _requestWiseFileRepository.Add(requestwisefile);
            }
            /* }*/
            return true; /*View("../Request/SubmitRequestPage");*/ /*which page is to be returned after saving the details in DB*/
        }
        #endregion

        #region FamilyRequest

        public async Task<bool> CreateFamilyRequest(ViewDataFamilyRequestModel viewDataFamilyRequest)
        {
            var aspnetuser = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(m => m.Email == viewDataFamilyRequest.Email);
            if (aspnetuser == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "localhost:5171/Login/CreateNewAccount";
                _emailConfiguration.SendMail(viewDataFamilyRequest.Email, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }
            var Request = new Request
            {
                Requesttypeid = 3, /* these details are added to requestclient table to refer to patient*/
                Status = 1,
                Firstname = viewDataFamilyRequest.FF_FirstName,
                Lastname = viewDataFamilyRequest.FF_LastName,
                Email = viewDataFamilyRequest.FF_Email,
                Relationname = viewDataFamilyRequest.FF_RelationWithPatient,
                Confirmationnumber = _confirmationNumberService.GetConfirmationNumber(viewDataFamilyRequest.State, viewDataFamilyRequest.FirstName, viewDataFamilyRequest.LastName),
                Phonenumber = viewDataFamilyRequest.FF_PhoneNumber,
                Createddate = DateTime.Now,
                Isdeleted = new BitArray(1),
                Isurgentemailsent = new BitArray(1)
            };
            _requestRepository.Add(Request);

            var Requestclient = new Requestclient
            {
                Request = Request, /* these details are added to requestclient table*/
                Requestid = Request.Requestid,
                Notes = viewDataFamilyRequest.Symptoms,
                Firstname = viewDataFamilyRequest.FirstName,
                Lastname = viewDataFamilyRequest.LastName,
                Phonenumber = viewDataFamilyRequest.PhoneNumber,
                Email = viewDataFamilyRequest.Email,
                Location = viewDataFamilyRequest.RoomSuite,
                Strmonth = viewDataFamilyRequest.DateOfBirth.ToString("MMMM"),
                Intdate = viewDataFamilyRequest.DateOfBirth.Day,
                Intyear = viewDataFamilyRequest.DateOfBirth.Year,
                Address = viewDataFamilyRequest.Street + "," + viewDataFamilyRequest.City + "," + viewDataFamilyRequest.State,
                Street = viewDataFamilyRequest.Street,
                State = viewDataFamilyRequest.State,
                City = viewDataFamilyRequest.City,
                Zipcode = viewDataFamilyRequest.ZipCode,
                Regionid = 1

            };
            _requestClientRepository.Add(Requestclient);

            if (viewDataFamilyRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(viewDataFamilyRequest.UploadFile, Request.Requestid);

                var requestwisefile = new Requestwisefile()
                {
                    Requestid = Request.Requestid,
                    Filename = upload,
                    Isdeleted = new BitArray(1),
                    Createddate = DateTime.Now,
                };
                _requestWiseFileRepository.Add(requestwisefile);
            }

            return true;
        }
        #endregion

        #region ConciergeRequest

        public async Task<bool> CreateConciergeRequest(ViewDataConciergeRequestModel viewDataConciergeRequest)
        {
            var aspnetuser = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(m => m.Email == viewDataConciergeRequest.Email);
            if (aspnetuser == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "localhost:5171/AdminPanel/Login/CreateAccount";
                _emailConfiguration.SendMail(viewDataConciergeRequest.Email, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }
            var Concierge = new Concierge();
            var Request = new Request();
            var Requestclient = new Requestclient();
            var Requestconcierge = new Requestconcierge();

            Concierge.Conciergename = viewDataConciergeRequest.CON_FirstName + " " + viewDataConciergeRequest.CON_LastName;
            Concierge.Street = viewDataConciergeRequest.CON_Street;
            Concierge.City = viewDataConciergeRequest.CON_City;
            Concierge.State = viewDataConciergeRequest.CON_State;
            Concierge.Zipcode = viewDataConciergeRequest.CON_Zipcode;
            Concierge.Address = viewDataConciergeRequest.CON_Street + " " + viewDataConciergeRequest.CON_City + " " + viewDataConciergeRequest.CON_State;
            Concierge.Regionid = 1;
            Concierge.Createddate = DateTime.Now;
            _conciergeRepository.Add(Concierge);
            int id1 = Concierge.Conciergeid;

            Request.Requesttypeid = 4;
            Request.Status = 1;
            Request.Firstname = viewDataConciergeRequest.CON_FirstName;
            Request.Lastname = viewDataConciergeRequest.CON_LastName;
            Request.Email = viewDataConciergeRequest.CON_Email;
            Request.Confirmationnumber = _confirmationNumberService.GetConfirmationNumber(viewDataConciergeRequest.CON_State, viewDataConciergeRequest.FirstName, viewDataConciergeRequest.LastName);
            Request.Phonenumber = viewDataConciergeRequest.CON_PhoneNumber;
            Request.Isdeleted = new BitArray(1);
            Request.Isurgentemailsent = new BitArray(1);
            Request.Createddate = DateTime.Now;
            _requestRepository.Add(Request);
            int id2 = Request.Requestid;

            Requestclient.Requestid = Request.Requestid;
            Requestclient.Notes = viewDataConciergeRequest.Symptoms;
            Requestclient.Firstname = viewDataConciergeRequest.FirstName;
            Requestclient.Lastname = viewDataConciergeRequest.LastName;
            Requestclient.Email = viewDataConciergeRequest.Email;
            Requestclient.Phonenumber = viewDataConciergeRequest.PhoneNumber;
            Requestclient.Intdate = viewDataConciergeRequest.DateOfBirth.Day;
            Requestclient.Strmonth = viewDataConciergeRequest.DateOfBirth.ToString("MMMM");
            Requestclient.Intyear = viewDataConciergeRequest.DateOfBirth.Year;
            Requestclient.Location = viewDataConciergeRequest.RoomSuite;
            Requestclient.City = viewDataConciergeRequest.CON_City;
            Requestclient.State = viewDataConciergeRequest.CON_State;
            Requestclient.Zipcode = viewDataConciergeRequest.CON_Zipcode;
            Requestclient.Address = viewDataConciergeRequest.CON_Street + " " + viewDataConciergeRequest.CON_City + " " + viewDataConciergeRequest.CON_State + " " + viewDataConciergeRequest.CON_Zipcode;
            _requestClientRepository.Add(Requestclient);

            Requestconcierge.Requestid = id2;
            Requestconcierge.Conciergeid = id1;

            _requestConciergeRepository.Add(Requestconcierge);

            return true;
        }
        #endregion

        #region BusinessRequest

        public async Task<bool> CreateBusinessRequest(ViewDataBusinessRequestModel viewDataBusinessRequest)
        {
            var aspnetuser = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(m => m.Email == viewDataBusinessRequest.Email);
            if (aspnetuser == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "localhost:5171/AdminPanel/Login/CreateAccount";
                _emailConfiguration.SendMail(viewDataBusinessRequest.Email, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }
            var Request = new Request
            {
                Requesttypeid = 4,
                Status = 1,
                Firstname = viewDataBusinessRequest.BP_FirstName,
                Lastname = viewDataBusinessRequest.BP_LastName,
                Email = viewDataBusinessRequest.BP_Email,
                Phonenumber = viewDataBusinessRequest.BP_PhoneNumber,
                Confirmationnumber = _confirmationNumberService.GetConfirmationNumber(viewDataBusinessRequest.State, viewDataBusinessRequest.FirstName, viewDataBusinessRequest.LastName),
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1),
                Isdeleted = new BitArray(1)

            };
            _requestRepository.Add(Request);

            var Business = new Business
            {
                Name = viewDataBusinessRequest.BP_FirstName + " " + viewDataBusinessRequest.BP_LastName,
                Phonenumber = viewDataBusinessRequest.BP_PhoneNumber,
                Address1 = viewDataBusinessRequest.Street,
                City = viewDataBusinessRequest.City,
                Zipcode = viewDataBusinessRequest.ZipCode,
                Createddate = DateTime.Now
            };
            _businessRepository.Add(Business);

            var Requestclient = new Requestclient
            {
                Request = Request,
                Requestid = Request.Requestid,
                Notes = viewDataBusinessRequest.Symptoms,
                Firstname = viewDataBusinessRequest.FirstName,
                Lastname = viewDataBusinessRequest.LastName,
                Phonenumber = viewDataBusinessRequest.PhoneNumber,
                Email = viewDataBusinessRequest.Email,
                Intdate = viewDataBusinessRequest.DateOfBirth.Day,
                Strmonth = viewDataBusinessRequest.DateOfBirth.ToString("MMMM"),
                Intyear = viewDataBusinessRequest.DateOfBirth.Year,
                State = viewDataBusinessRequest.State,
                City = viewDataBusinessRequest.City,
                Zipcode = viewDataBusinessRequest.ZipCode,
                Address = viewDataBusinessRequest.Street + "," + viewDataBusinessRequest.City + "," + viewDataBusinessRequest.State,
                Location = viewDataBusinessRequest.RoomSuite,
                Regionid = 1

            };
            _requestClientRepository.Add(Requestclient);
            return true;
        }
        #endregion
    }
}
