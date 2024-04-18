using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Patient.Repository
{
    public class CreateRequest : ICreateRequest
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IConfirmationNumber _confirmationNumber;
        public CreateRequest(HalloDocContext context, IConfirmationNumber iConfirmationNumber)
        {
            _context = context;
            _confirmationNumber = iConfirmationNumber;
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
                var isexist = _context.Users.FirstOrDefault(x => x.Email == viewDataPatientRequest.Email);
                if (isexist == null)
                {
                    // Aspnetuser
                    Guid g = Guid.NewGuid();
                    Aspnetuser.Id = g.ToString();
                    Aspnetuser.Username = viewDataPatientRequest.Email;
                    Aspnetuser.Passwordhash = viewDataPatientRequest.PassWord;
                    Aspnetuser.CreatedDate = DateTime.Now;
                    Aspnetuser.Email = viewDataPatientRequest.Email;
                    _context.Aspnetusers.Add(Aspnetuser);
                    await _context.SaveChangesAsync();

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
                    User.Createdby = Aspnetuser.Id;
                    User.Createddate = DateTime.Now;
                    _context.Users.Add(User);
                    await _context.SaveChangesAsync();
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
            Request.Confirmationnumber = _confirmationNumber.GetConfirmationNumber(viewDataPatientRequest.State, viewDataPatientRequest.FirstName, viewDataPatientRequest.LastName);
            Request.Isurgentemailsent = new BitArray(1);
            Request.Isdeleted = new BitArray(1);
                Request.Createddate = DateTime.Now;
                _context.Requests.Add(Request);
                await _context.SaveChangesAsync();

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

            _context.Requestclients.Add(Requestclient);
                await _context.SaveChangesAsync();

                if (viewDataPatientRequest.UploadFile != null)
                {
                    string FilePath = "wwwroot\\Upload";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileNameWithPath = Path.Combine(path, viewDataPatientRequest.UploadFile.FileName);
                    viewDataPatientRequest.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewDataPatientRequest.UploadFile.FileName;

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        viewDataPatientRequest.UploadFile.CopyTo(stream);
                    }

                    var requestwisefile = new Requestwisefile
                    {
                        Requestid = Request.Requestid,
                        Filename = viewDataPatientRequest.UploadFile.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                }
           /* }*/
            return true; /*View("../Request/SubmitRequestPage");*/ /*which page is to be returned after saving the details in DB*/
        }
        #endregion

        #region FamilyRequest

        public async Task<bool> CreateFamilyRequest(ViewDataFamilyRequestModel viewDataFamilyRequest)
        {
            var Request = new Request
            {
                Requesttypeid = 3, /* these details are added to requestclient table to refer to patient via client*/
                Status = 1,
                Firstname = viewDataFamilyRequest.FF_FirstName,
                Lastname = viewDataFamilyRequest.FF_LastName,
                Email = viewDataFamilyRequest.FF_Email,
                Relationname = viewDataFamilyRequest.FF_RelationWithPatient,
                Phonenumber = viewDataFamilyRequest.FF_PhoneNumber,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1)

            };
            _context.Requests.Add(Request);/*To add details to DB*/
            await _context.SaveChangesAsync();/*To save details to synchronisation*/

            var Requestclient = new Requestclient
            {
                Request = Request, /* these details are added to request table*/
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
                Zipcode = viewDataFamilyRequest.ZipCode

            };
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            if (viewDataFamilyRequest.UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewDataFamilyRequest.UploadFile.FileName);
                viewDataFamilyRequest.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewDataFamilyRequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewDataFamilyRequest.UploadFile.CopyTo(stream);
                }

                var requestwisefile = new Requestwisefile()
                {
                    Requestid = Request.Requestid,
                    Filename = viewDataFamilyRequest.UploadFile.FileName,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }

            return true;
        }
        #endregion

        #region ConciergeRequest

        public async Task<bool> CreateConciergeRequest(ViewDataConciergeRequestModel viewDataConciergeRequest)
        {
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
            _context.Concierges.Add(Concierge);
            await _context.SaveChangesAsync();
            int id1 = Concierge.Conciergeid;

            Request.Requesttypeid = 4;
            Request.Status = 1;
            Request.Firstname = viewDataConciergeRequest.CON_FirstName;
            Request.Lastname = viewDataConciergeRequest.CON_LastName;
            Request.Email = viewDataConciergeRequest.CON_Email;
            Request.Phonenumber = viewDataConciergeRequest.CON_PhoneNumber;
            Request.Isurgentemailsent = new BitArray(1);
            Request.Createddate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();
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
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            Requestconcierge.Requestid = id2;
            Requestconcierge.Conciergeid = id1;

            _context.Requestconcierges.Add(Requestconcierge);
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion

        #region BusinessRequest

        public async Task<bool> CreateBusinessRequest(ViewDataBusinessRequestModel viewDataBusinessRequest)
        {

            var Request = new Request
            {
                Requesttypeid = 4,
                Status = 1,
                Firstname = viewDataBusinessRequest.BP_FirstName,
                Lastname = viewDataBusinessRequest.BP_LastName,
                Email = viewDataBusinessRequest.BP_Email,
                Phonenumber = viewDataBusinessRequest.BP_PhoneNumber,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1)

            };
            _context.Requests.Add(Request);/*To add details to DB*/
            await _context.SaveChangesAsync();/*To save details to synchronisation*/

            var Business = new Business
            {
                Name = viewDataBusinessRequest.BP_FirstName + " " + viewDataBusinessRequest.BP_LastName,
                Phonenumber = viewDataBusinessRequest.BP_PhoneNumber,
                Address1 = viewDataBusinessRequest.Street,
                City = viewDataBusinessRequest.City,
                Zipcode = viewDataBusinessRequest.ZipCode,
                Createddate = DateTime.Now
            };
            _context.Businesses.Add(Business);/*To add details to DB*/
            await _context.SaveChangesAsync();/*To save details to synchronisation*/

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
                Address = viewDataBusinessRequest.Street + "," + viewDataBusinessRequest.City  + "," + viewDataBusinessRequest.State,
                Location = viewDataBusinessRequest.RoomSuite

            };
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
