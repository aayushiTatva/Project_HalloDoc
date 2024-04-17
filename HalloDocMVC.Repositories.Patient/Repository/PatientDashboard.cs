using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections;
using System.Globalization;


namespace HalloDocMVC.Repositories.Patient.Repository
{
    public class PatientDashboard : IPatientDashboard
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public PatientDashboard(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        public PatientDashboardModel GetPatientData(string id, PatientDashboardModel model)
        {
            List<PatientDashboardModel> allData = _context.Requests.Include(x => x.Requestwisefiles)
                                                                  .Where(x => x.Userid == Int32.Parse(id) && x.Isdeleted == new BitArray(1))
                                                                  .Select(x => new PatientDashboardModel
                                                                  {
                                                                      CreatedDate = x.Createddate,
                                                                      Status = x.Status,
                                                                      RequestId = x.Requestid,
                                                                      DocumentCount = x.Requestwisefiles.Where(x => x.Isdeleted == new BitArray(1)).Count()
                                                                  }).ToList();

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)model.PageSize);
            List<PatientDashboardModel> list1 = allData.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            PatientDashboardModel Data = new()
            {
                PatientData = list1,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages,
                PageSize = model.PageSize,
                IsAscending = model.IsAscending,
                TotalItemCount = totalItemCount,
                UserId = Int32.Parse(id),
            };
            return Data;
        }

        #region UploadDoc
        public async Task<bool> UploadDoc(int RequestId, IFormFile? UploadFile)
        {
            if (UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(UploadFile, RequestId);
                var requestwisefile = new Requestwisefile
                {
                    Requestid = RequestId,
                    Filename = upload,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion

        #region RequestForMe (RequestByPatient)
        public ViewDataPatientRequestModel RequestForMe()
        {
            var patientRequest = _context.Users
                                .Where(r => r.Userid == Convert.ToInt32(CV.UserID()))
                               .Select(r => new ViewDataPatientRequestModel
                               {
                                   FirstName = r.Firstname,
                                   LastName = r.Lastname,
                                   Email = r.Email,
                                   PhoneNumber = r.Mobile,
                                   DateOfBirth = new DateTime((int)r.Intyear, DateTime.ParseExact(r.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)r.Intdate)
                               })
                               .FirstOrDefault();
            return patientRequest;
        }
        #endregion

        #region CreateRequestForMe
        public async Task<bool> PostMe(ViewDataPatientRequestModel viewpatientrequestforme)
        {
            var Request = new DBEntity.DataModels.Request();
            var Requestclient = new Requestclient();

            Request.Requesttypeid = 2;
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewpatientrequestforme.Email);
            Request.Userid = isexist.Userid;
            Request.Firstname = isexist.Firstname;
            Request.Lastname = isexist.Lastname;
            Request.Email = isexist.Email;
            Request.Phonenumber = isexist.Mobile;
            Request.Isurgentemailsent = new BitArray(1);
            Request.Createddate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Requestclient.Requestid = Request.Requestid;
            Requestclient.Firstname = viewpatientrequestforme.FirstName;
            Requestclient.Address = viewpatientrequestforme.Street;
            Requestclient.Lastname = viewpatientrequestforme.LastName;
            Requestclient.Email = viewpatientrequestforme.Email;
            Requestclient.Phonenumber = viewpatientrequestforme.PhoneNumber;


            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();


            if (viewpatientrequestforme.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(viewpatientrequestforme.UploadFile, Request.Requestid);

                var requestwisefile = new Requestwisefile
                {
                    Requestid = Request.Requestid,
                    Filename = upload,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion CreateRequestForMe

        #region CreateREquestForSomeoneElse
        public async Task<bool> PostSomeoneElse(ViewDataPatientRequestModel viewpatientrequestforelse)
        {
            var Request = new DBEntity.DataModels.Request();
            var Requestclient = new Requestclient();
            var isexist = _context.Users.FirstOrDefault(x => x.Userid == Convert.ToInt32(CV.UserID()));
            Request.Requesttypeid = 2;
            //Request.Userid = isexist.Userid;
            Request.Firstname = isexist.Firstname;
            Request.Lastname = isexist.Lastname;
            Request.Email = isexist.Email;
            Request.Phonenumber = isexist.Mobile;
            Request.Relationname = viewpatientrequestforelse.Relation;
            Request.Isurgentemailsent = new BitArray(1);
            Request.Createddate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Requestclient.Requestid = Request.Requestid;
            Requestclient.Firstname = viewpatientrequestforelse.FirstName;
            Requestclient.Address = viewpatientrequestforelse.Street;
            Requestclient.Lastname = viewpatientrequestforelse.LastName;
            Requestclient.Email = viewpatientrequestforelse.Email;
            Requestclient.Phonenumber = viewpatientrequestforelse.PhoneNumber;

            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();


            if (viewpatientrequestforelse.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(viewpatientrequestforelse.UploadFile, Request.Requestid);

                var requestwisefile = new Requestwisefile
                {
                    Requestid = Request.Requestid,
                    Filename = upload,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion
    }
}