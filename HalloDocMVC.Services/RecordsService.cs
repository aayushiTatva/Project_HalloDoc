using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class RecordsService : IRecordsService
    {
        #region Configuration
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<Requestclient> _requestClientRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Requestnote> _requestNotesRepository;
        private readonly IGenericRepository<Region> _regionRepository;
        private readonly IGenericRepository<Emaillog> _emailLogRepository;
        private readonly IGenericRepository<Smslog> _smslogRepository;
        private readonly IGenericRepository<Blockrequest> _blockRequestRepository;
        private readonly IGenericRepository<Admin> _adminRepository;
        private readonly IGenericRepository<Requeststatuslog> _requestStatusLogRepository;
        public RecordsService(IGenericRepository<Request> iRequestRepository, IGenericRepository<Requestclient> iRequestClientRepository,
            IGenericRepository<Physician> iPhysicianRepository, IGenericRepository<Requestnote> iRequestNotesRepository,
            IGenericRepository<Region> iRegionRepository, IGenericRepository<Emaillog> iEmailLogRepository, IGenericRepository<Smslog> iSmslogRepository,
            IGenericRepository<Blockrequest> iBlockRequestRepository, IGenericRepository<Admin> iAdminRepository, IGenericRepository<Requeststatuslog> iRequestStatusLogRepository)
        {
            _requestRepository = iRequestRepository;
            _requestClientRepository = iRequestClientRepository;
            _physicianRepository = iPhysicianRepository;
            _requestNotesRepository = iRequestNotesRepository;
            _regionRepository = iRegionRepository;
            _emailLogRepository = iEmailLogRepository;
            _smslogRepository = iSmslogRepository;
            _blockRequestRepository = iBlockRequestRepository;
            _adminRepository = iAdminRepository;
            _requestStatusLogRepository = iRequestStatusLogRepository;
        }
        #endregion

        #region GetRecords
        public async Task<RecordsModel> GetRecords(RecordsModel model)
        {
            List<SearchRecordsModel> sr = (from r in _requestRepository.GetAll()
                                           join rc in _requestClientRepository.GetAll()
            on r.Requestid equals rc.Requestid into reqClientGroup
                                           from rcg in reqClientGroup.DefaultIfEmpty()
                                           join phy in _physicianRepository.GetAll()
                                           on r.Physicianid equals phy.Physicianid into reqPhyGroup
                                           from rpg in reqPhyGroup.DefaultIfEmpty()
                                           join rn in _requestNotesRepository.GetAll()
                                           on r.Requestid equals rn.Requestid into reqNotesGroup
                                           from rng in reqNotesGroup.DefaultIfEmpty()
                                           where r.Isdeleted == new BitArray(1) &&
                                           (model.Status == null || model.Status == -1 || model.Status == 0 || r.Status == model.Status) &&
                                           (model.FirstName == null || r.Firstname.ToLower().Contains(model.FirstName.ToLower())) &&
                                           (model.RequestType == null || model.RequestType == 0 || r.Requesttypeid == model.RequestType) &&
                                           (model.StartDate == null || r.Createddate.Date == model.StartDate) &&
                                           (model.PhysicianName == null || rpg.Firstname.ToLower().Contains(model.PhysicianName.ToLower())) &&
                                           (model.Email == null || rcg.Email.ToLower().Contains(model.Email.ToLower())) &&
                                           (model.PhoneNumber == null || r.Phonenumber.Contains(model.PhoneNumber))

                                           select new SearchRecordsModel
                                           {
                                               RequestId = r.Requestid,
                                               RequestTypeId = r.Requesttypeid,
                                               FirstName = rcg.Firstname,
                                               Lastname = rcg.Lastname,
                                               Requestor = r.Requesttypeid,
                                               StartDate = r.Createddate,
                                               Email = rcg.Email,
                                               PhoneNumber = rcg.Phonenumber,
                                               Address = rcg.Street + " " + rcg.Address + " " + rcg.City,
                                               ZipCode = rcg.Zipcode,
                                               Status = r.Status,
                                               PhysicianName = rpg.Firstname + " " + rpg.Lastname,
                                               PhysicianNote = rng.Physiciannotes,/*
                                                CancelledByProviderNotes = rng != null ? rng.Physiciannotes : "",*/
                                               AdminNotes = rng.Adminnotes,
                                               PatientNotes = rcg.Notes

                                           }).ToList();
            int totalCount = sr.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);
            List<SearchRecordsModel> list = sr.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                SearchRecords = list,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion
        #region DeleteRequest
        public bool DeleteRequest(int? RequestId)
        {
            try
            {
                var data = _requestRepository.GetAll().FirstOrDefault(v => v.Requestid == RequestId);
                data.Isdeleted = new BitArray(1);
                data.Isdeleted[0] = true;
                data.Modifieddate = DateTime.Now;
                _requestRepository.Update(data);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion DeleteRequest

        #region GetPatientHistory
        public async Task<RecordsModel> GetPatientHistory(RecordsModel model)
        {
            List<SearchRecordsModel> sr = (from r in _requestRepository.GetAll()
                                           join rc in _requestClientRepository.GetAll()
                                           on r.Requestid equals rc.Requestid into reqClientGroup
                                           from rcg in reqClientGroup.DefaultIfEmpty()
                                           where r.Isdeleted == new BitArray(1) && (model.FirstName == null || rcg.Firstname.ToLower().Contains(model.FirstName.ToLower())) &&
                                           (model.Email == null || rcg.Email.ToLower().Contains(model.Email.ToLower())) &&
                                           (model.LastName == null || rcg.Lastname.ToLower().Contains(model.LastName.ToLower())) &&
                                           (model.PhoneNumber == null || rcg.Phonenumber.Contains(model.PhoneNumber))
                                           select new SearchRecordsModel
                                           {
                                               RequestId = r.Requestid,
                                               FirstName = rcg.Firstname,
                                               Lastname = rcg.Lastname,
                                               Email = rcg.Email,
                                               PhoneNumber = rcg.Phonenumber,
                                               Address = rcg.Street + " " + rcg.Address + " " + rcg.City,
                                               UserId = (int)r.Userid
                                           }).ToList();
            int totalCount = sr.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);
            List<SearchRecordsModel> list = sr.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                SearchRecords = list,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetPatientCases
        public async Task<RecordsModel> GetPatientCases(int UserId, RecordsModel records)
        {
            List<SearchRecordsModel> sr = (from req in _requestRepository.GetAll()
                                           join reqClient in _requestClientRepository.GetAll()
                                           on req.Requestid equals reqClient.Requestid into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _physicianRepository.GetAll()
                                           on req.Physicianid equals phys.Physicianid into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join reg in _regionRepository.GetAll()
                                           on rc.Regionid equals reg.Regionid into RegGroup
                                           from rg in RegGroup.DefaultIfEmpty()
                                           where req.Userid == (UserId == null ? records.UserId : UserId)
                                           select new SearchRecordsModel
                                           {
                                               RequestId = req.Requestid,
                                               RequestTypeId = req.Requesttypeid,
                                               FirstName = rc.Firstname,
                                               StartDate = req.Createddate,
                                               Lastname = rc.Lastname,
                                               PhysicianName = p.Firstname + " " + p.Lastname,

                                           }).ToList();
            int totalCount = sr.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)records.PageSize);
            List<SearchRecordsModel> list = sr.Skip((records.CurrentPage - 1) * records.PageSize).Take(records.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                SearchRecords = list,
                CurrentPage = records.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetEmailLogs
        public async Task<RecordsModel> GetEmailLogs(RecordsModel model)
        {
            List<EmailLogModel> em = await (from req in _emailLogRepository.GetAll()
                                            where req.Emailid != null && (model.AccountType == null || model.AccountType == 0 || req.Roleid == model.AccountType) &&
                                           (model.ReceiverName == null || _requestClientRepository.GetAll().FirstOrDefault(e => e.Email == req.Emailid).Firstname.ToLower().Contains(model.FirstName.ToLower())) &&
                                           (model.StartDate == null || req.Createdate.Date == model.StartDate) &&
                                           (model.SentDate == null || req.Sentdate.Value.Date == model.SentDate) &&
                                           (model.Email == null || req.Emailid.ToLower().Contains(model.Email.ToLower()))
                                            select new EmailLogModel
                                            {
                                                RequestId = req.Requestid,
                                                Recipient = _requestClientRepository.GetAll().FirstOrDefault(e => e.Email == req.Emailid).Firstname ?? null,
                                                EmailId = req.Emailid,
                                                CreatedDate = req.Createdate,
                                                SentDate = (DateTime)req.Sentdate,
                                                RoleId = req.Roleid,
                                                Action = req.Action
                                            }).ToListAsync();

            int totalCount = em.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);
            List<EmailLogModel> list = em.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                EmailLog = list,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetSMSLogs
        public async Task<RecordsModel> GetSMSLogs(RecordsModel model)
        {
            List<SMSLogsModel> sm = await (from req in _smslogRepository.GetAll()
                                           where (model.AccountType == null || model.AccountType == -1 || req.Roleid == model.AccountType) &&
                                           (model.ReceiverName == null || _requestClientRepository.GetAll().FirstOrDefault(e => e.Phonenumber == req.Mobilenumber).Firstname.ToLower().Contains(model.ReceiverName.ToLower())) &&
                                           (model.StartDate == null || req.Createdate.Date == model.StartDate &&
                                           (model.SentDate == null || req.Sentdate.Value.Date == model.SentDate) &&
                                           (model.PhoneNumber == null || req.Mobilenumber.Contains(model.PhoneNumber)))
                                           select new SMSLogsModel
                                           {
                                               RequestId = req.Requestid,
                                               Recipient = _requestClientRepository.GetAll().FirstOrDefault(e => e.Phonenumber == req.Mobilenumber).Firstname ?? null,
                                               PhoneNumber = req.Mobilenumber,
                                               CreatedDate = req.Createdate,
                                               SentDate = (DateTime)req.Sentdate,
                                               Action = req.Action,
                                               RoleId = req.Roleid
                                           }).ToListAsync();
            int totalCount = sm.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);
            List<SMSLogsModel> list = sm.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                SMSLog = list,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetBlockedHistory
        public async Task<RecordsModel> GetBlockedHistory(RecordsModel model)
        {
            List<BlockRequestModel> data = (from req in _blockRequestRepository.GetAll()
                                            where (model.StartDate == null || req.Createddate.Value.Date == model.StartDate) &&
                                                  (model.FirstName.IsNullOrEmpty() || _requestClientRepository.GetAll().FirstOrDefault(e => e.Requestid == Convert.ToInt32(req.Requestid)).Firstname.ToLower().Contains(model.FirstName.ToLower())) &&
                                                  (model.Email.IsNullOrEmpty() || req.Email.ToLower().Contains(model.Email.ToLower())) &&
                                                  (model.PhoneNumber.IsNullOrEmpty() || req.Phonenumber.ToLower().Contains(model.PhoneNumber.ToLower()))
                                            select new BlockRequestModel
                                            {
                                                PatientName = _requestClientRepository.GetAll().FirstOrDefault(e => e.Requestid == Convert.ToInt32(req.Requestid)).Firstname,
                                                Email = req.Email,
                                                CreatedDate = (DateTime)req.Createddate,
                                                IsActive = req.Isactive,
                                                RequestId = Convert.ToInt32(req.Requestid),
                                                PhoneNumber = req.Phonenumber,
                                                Reason = req.Reason
                                            }).ToList();
            int totalCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);
            List<BlockRequestModel> list = data.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            RecordsModel roles1 = new()
            {
                BlockedRequest = list,
                CurrentPage = model.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region Unblock
        public bool Unblock(int RequestId, string id)
        {
            try
            {
                Blockrequest block = _blockRequestRepository.GetAll().FirstOrDefault(e => e.Requestid == RequestId);
                block.Isactive = new BitArray(1);
                block.Isactive[0] = true;
                _blockRequestRepository.Update(block);

                DBEntity.DataModels.Request request = _requestRepository.GetAll().FirstOrDefault(e => e.Requestid == RequestId);
                request.Status = 1;
                request.Modifieddate = DateTime.Now;
                _requestRepository.Update(request);

                var admindata = _adminRepository.GetAll().FirstOrDefault(e => e.Aspnetuserid == id);
                Requeststatuslog rs = new()
                {
                    Status = 1,
                    Requestid = RequestId,
                    Adminid = admindata.Adminid,
                    Createddate = DateTime.Now
                };
                _requestStatusLogRepository.Add(rs);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion Unblock
    }
}
