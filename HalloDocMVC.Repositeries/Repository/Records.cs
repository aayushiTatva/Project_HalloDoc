using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class Records : IRecords
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public Records(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region GetRecords
        public async Task<RecordsModel> GetRecords(RecordsModel model)
        {
            List<SearchRecordsModel> sr = (from r in _context.Requests
                                                 join rc in _context.Requestclients
                                                 on r.Requestid equals rc.Requestid into reqClientGroup
                                                 from rcg in reqClientGroup.DefaultIfEmpty()
                                                 join phy in _context.Physicians
                                                 on r.Physicianid equals phy.Physicianid into reqPhyGroup
                                                 from rpg in reqPhyGroup.DefaultIfEmpty()
                                                 join rn in _context.Requestnotes
                                                 on r.Requestid equals rn.Requestid into reqNotesGroup
                                                 from rng in reqNotesGroup.DefaultIfEmpty()
                                                 where r.Isdeleted == new BitArray(1) && 
                                                 (model.Status == null || model.Status == -1 || model.Status == 0 || r.Status == model.Status) &&
                                                 (model.FirstName == null || r.Firstname.Contains(model.FirstName)) && 
                                                 (model.RequestType == null || model.RequestType == 0 || r.Requesttypeid == model.RequestType) &&
                                                 (model.StartDate == DateTime.MinValue || r.Createddate.Date == model.StartDate.Date) &&
                                                 (model.PhysicianName == null || rpg.Firstname.Contains(model.PhysicianName)) &&
                                                 (model.Email == null || r.Email.Contains(model.Email)) && 
                                                 (model.PhoneNumber == null || r.Phonenumber.Contains(model.PhoneNumber))

                                                 select new SearchRecordsModel
                                                 {
                                                     RequestId = r.Requestid,
                                                     RequestTypeId = r.Requesttypeid,
                                                     FirstName = r.Firstname,
                                                     Lastname = r.Lastname,
                                                     Requestor = r.Requesttypeid,
                                                     StartDate = r.Createddate,
                                                     Email = r.Email,
                                                     PhoneNumber = r.Phonenumber,
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
                var data = _context.Requests.FirstOrDefault(v => v.Requestid == RequestId);
                data.Isdeleted = new BitArray(1);
                data.Isdeleted[0] = true;
                data.Modifieddate = DateTime.Now;
                _context.Requests.Update(data);
                _context.SaveChanges();
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
            List<SearchRecordsModel> sr = (from r in _context.Requests
                                           join rc in _context.Requestclients
                                           on r.Requestid equals rc.Requestid into reqClientGroup
                                           from rcg in reqClientGroup.DefaultIfEmpty()
                                           where r.Isdeleted == new BitArray(1) && (model.FirstName == null || r.Firstname.Contains(model.FirstName)) && 
                                           (model.Email == null || r.Email.Contains(model.Email)) && 
                                           (model.LastName == null || r.Lastname.Contains(model.LastName)) && 
                                           (model.PhoneNumber == null || r.Phonenumber.Contains(model.PhoneNumber))
                                           select new SearchRecordsModel
                                           {
                                               RequestId = r.Requestid,
                                               FirstName = r.Firstname,
                                               Lastname = r.Lastname,
                                               Email = r.Email,
                                               PhoneNumber = r.Phonenumber,
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
            List<SearchRecordsModel> sr = (from req in _context.Requests
                                           join reqClient in _context.Requestclients
                                           on req.Requestid equals reqClient.Requestid into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _context.Physicians
                                           on req.Physicianid equals phys.Physicianid into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join reg in _context.Regions
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
            List<EmailLogModel> em = await (from req in _context.Emaillogs
                                            where req.Emailid != null && (model.AccountType == null || model.AccountType == -1 || req.Roleid == model.AccountType) &&
                                           (model.ReceiverName == null || _context.Requestclients.FirstOrDefault(e => e.Email == req.Emailid).Firstname.Contains(model.ReceiverName)) &&
                                           (model.StartDate.Date == DateTime.MinValue || req.Createdate.Date == model.StartDate.Date) &&
                                           (model.SentDate.Date == DateTime.MinValue || req.Sentdate.Value.Date == model.SentDate.Date) &&
                                           (model.Email == null || req.Emailid.Contains(model.Email))
                                            select new EmailLogModel
                                              {
                                                  RequestId = req.Requestid,
                                                  Recipient = _context.Requestclients.FirstOrDefault(e => e.Email == req.Emailid).Firstname ?? null,
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
            List<SMSLogsModel> sm = await (from req in _context.Smslogs
                                           where (model.AccountType == null || model.AccountType == -1 || req.Roleid == model.AccountType) &&
                                           (model.ReceiverName == null || _context.Requestclients.FirstOrDefault(e => e.Phonenumber == req.Mobilenumber).Firstname.Contains(model.ReceiverName)) &&
                                           (model.StartDate.Date == DateTime.MinValue || req.Createdate.Date == model.StartDate.Date) &&
                                           (model.SentDate.Date == DateTime.MinValue || req.Sentdate.Value.Date == model.SentDate.Date) &&
                                           (model.PhoneNumber == null || req.Mobilenumber.Contains(model.PhoneNumber))
                                           select new SMSLogsModel
                                           {
                                               RequestId = req.Requestid,
                                               Recipient = _context.Requestclients.FirstOrDefault(e => e.Phonenumber == req.Mobilenumber).Firstname ?? null,
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
            List<BlockRequestModel> data = (from req in _context.Blockrequests
                                            where (model.StartDate == DateTime.MinValue || req.Createddate.Value.Date == model.StartDate.Date) &&
                                                  (model.FirstName.IsNullOrEmpty() || _context.Requests.FirstOrDefault(e => e.Requestid == Convert.ToInt32(req.Requestid)).Firstname.ToLower().Contains(model.FirstName.ToLower())) &&
                                                  (model.Email.IsNullOrEmpty() || req.Email.ToLower().Contains(model.Email.ToLower())) &&
                                                  (model.PhoneNumber.IsNullOrEmpty() || req.Phonenumber.ToLower().Contains(model.PhoneNumber.ToLower()))
                                            select new BlockRequestModel
                                            {
                                                PatientName = _context.Requests.FirstOrDefault(e => e.Requestid == Convert.ToInt32(req.Requestid)).Firstname,
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
                Blockrequest block = _context.Blockrequests.FirstOrDefault(e => e.Requestid == RequestId.ToString());
                block.Isactive = new BitArray(1);
                block.Isactive[0] = true;
                _context.Blockrequests.Update(block);
                _context.SaveChanges();

                DBEntity.DataModels.Request request = _context.Requests.FirstOrDefault(e => e.Requestid == RequestId);
                request.Status = 1;
                request.Modifieddate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
                Requeststatuslog rs = new()
                {
                    Status = 1,
                    Requestid = RequestId,
                    Adminid = admindata.Adminid,
                    Createddate = DateTime.Now
                };
                _context.Requeststatuslogs.Add(rs);
                _context.SaveChanges();

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
