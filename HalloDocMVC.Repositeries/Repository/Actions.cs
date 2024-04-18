using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Globalization;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class Actions : IActions
    {
        private readonly HalloDocContext _context;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Actions(HalloDocContext context, EmailConfiguration emailConfiguration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailConfiguration = emailConfiguration;
            _httpContextAccessor = httpContextAccessor;
        }

        #region GetRequestForViewCase
        public ViewCaseModel GetRequestForViewCase(int id)
        {
            var n = _context.Requests.FirstOrDefault(E => E.Requestid == id);
            var l = _context.Requestclients.FirstOrDefault(E => E.Requestid == id);
            var region = _context.Regions.FirstOrDefault(E => E.Regionid == l.Regionid);
            ViewCaseModel requestforviewcase = new()
            {
                RequestId = id,
                Region = region.Name,
                RequestTypeId = n.Requesttypeid,
                FirstName = l.Firstname,
                LastName = l.Lastname,
                ConfirmationNumber = n.Confirmationnumber,
                PhoneNumber = l.Phonenumber,
                Email = l.Email,
                Address = l.Street + "," + l.City + "," + l.State,
                Notes = l.Notes,
                Room = l.Address,
                DateOfBirth = new DateTime((int)l.Intyear, DateTime.ParseExact(l.Strmonth, "MMMM", new CultureInfo("en-us")).Month, (int)l.Intdate),
            };
            return requestforviewcase;
        }
        #endregion
        #region EditCase
        public bool EditCase(ViewCaseModel model)
        {
            try
            {
                int monthnum = model.DateOfBirth.Month;
                int year = model.DateOfBirth.Year;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = model.DateOfBirth.Day;
                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == model.RequestId);
                if (client != null)
                {
                    client.Firstname = model.FirstName;
                    client.Lastname = model.LastName;
                    client.Email = model.Email;
                    client.Phonenumber = model.PhoneNumber;
                    client.Intdate = model.DateOfBirth.Day; //or date
                    client.Intyear = model.DateOfBirth.Year;
                    client.Strmonth = monthName;
                    client.Notes = model.Notes;
                    List<string> location = model.Address.Split(',').ToList(); //It splits the model's Address property by the comma character and converts the resulting array into a list of strings. It assigns this list to a variable named location.
                    client.Street = location[0];
                    client.City = location[1];
                    client.State = location[2];
                    client.Address = model.Room;
                    _context.Requestclients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region AssignProvider
        public async Task<bool> AssignProvider(int RequestId, int ProviderId, string notes)
        {

            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            request.Physicianid = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new()
            {
                Requestid = RequestId,
                Physicianid = ProviderId,
                Status=2,
                Createddate = DateTime.Now,
            };
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();

            return true;
        }
        #endregion

        #region Cancel Case
        public bool CancelCase(int RequestId, string Note, string CaseTag)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestId);
                if (requestData != null)
                {
                    requestData.Casetag = CaseTag;
                    requestData.Status = 8;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestId,
                        Notes = Note,
                        Status = 3,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Clear Case
        public bool ClearCase(int RequestID)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(req => req.Requestid == RequestID);
                if (request != null)
                {
                    request.Status = 10;
                    _context.Requests.Update(request);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new()
                    {
                        Requestid = RequestID,
                        Status = 10,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Block Case
        public bool BlockCase(int RequestID, string Note)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Blockrequest blc = new Blockrequest
                    {
                        Requestid = requestData.Requestid.ToString(),
                        Phonenumber = requestData.Phonenumber,
                        Email = requestData.Email,
                        Reason = Note,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now
                    };
                    _context.Blockrequests.Add(blc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region TransferPhysician
        public async Task<bool> TransferPhysician(int RequestId, int ProviderId, string Note)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);

            Requeststatuslog rsl = new()
            {
                Requestid = RequestId,
                Status = 2,
                Physicianid = request.Physicianid,
                Transtophysicianid = ProviderId,
                Notes = Note,
                Createddate = DateTime.Now
            };
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();

            request.Physicianid = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return true;
        }
        #endregion TransferPhysician

        #region getNotes
        public ViewNotesModel getNotes(int id)
        {
            var request = _context.Requests.FirstOrDefault(e => e.Requestid == id);
            var symptoms = _context.Requestclients.FirstOrDefault(e => e.Requestid == id);
            var transfer = (from rs in _context.Requeststatuslogs
                            join py in _context.Physicians on rs.Physicianid equals py.Physicianid into pyGroup
                            from py in pyGroup.DefaultIfEmpty()
                            join p in _context.Physicians on rs.Transtophysicianid equals p.Physicianid into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            join a in _context.Admins on rs.Adminid equals a.Adminid into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            where rs.Requestid == id && rs.Status == 2
                            select new TransferNotesModel
                            {
                                TransToPhysician = p.Firstname,
                                Admin = a.Firstname,
                                Physician = py.Firstname,
                                RequestId = rs.Requestid,
                                Notes = rs.Notes,
                                Status = rs.Status,
                                PhysicianId = rs.Physicianid,
                                CreatedDate = rs.Createddate,
                                RequestStatusLogId = rs.Requeststatuslogid,
                                TransToAdmin = rs.Transtoadmin,
                                TransToPhysicianId = rs.Transtophysicianid
                            }).ToList();
            var cancelbyprovider = _context.Requeststatuslogs.Where(e => e.Requestid == id && (e.Transtoadmin != null));
            var cancelbyadmin = _context.Requeststatuslogs.Where(e => e.Requestid == id && (e.Status == 7 || e.Status == 3));
            var model = _context.Requestnotes.FirstOrDefault(e => e.Requestid == id);
            ViewNotesModel vn = new ViewNotesModel();
            vn.RequestId = id;
            vn.PatientNotes = symptoms.Notes;
            if (model == null)
            {
                vn.PhysicianNotes = "-";
                vn.AdminNotes = "-";
            }
            else
            {
                vn.Status = request.Status;
                vn.RequestNotesId = model.Requestnotesid;
                vn.PhysicianNotes = model.Physiciannotes ?? "-";
                vn.AdminNotes = model.Adminnotes ?? "-";
            }

            List<TransferNotesModel> transnotes = new List<TransferNotesModel>();
            foreach (var item in transfer)
            {
                transfer.Add(new TransferNotesModel
                {
                    TransToPhysician = item.TransToPhysician,
                    Admin = item.Admin,
                    Physician = item.Physician,
                    RequestId = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.PhysicianId,
                    CreatedDate = item.CreatedDate,
                    RequestStatusLogId = item.RequestStatusLogId,
                    TransToAdmin = item.TransToAdmin,
                    TransToPhysicianId = item.TransToPhysicianId
                });
            }
            vn.transfernotes = transfer;
            List<TransferNotesModel> cancelbyphysician = new List<TransferNotesModel>();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotesModel
                {
                    RequestId = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.Physicianid,
                    CreatedDate = item.Createddate,
                    RequestStatusLogId = item.Requeststatuslogid,
                    TransToAdmin = item.Transtoadmin,
                    TransToPhysicianId = item.Transtophysicianid
                });
            }
            vn.cancelbyphysician = cancelbyphysician;

            List<TransferNotesModel> cancelrq = new List<TransferNotesModel>();
            foreach (var item in cancelbyadmin)
            {
                cancelrq.Add(new TransferNotesModel
                {
                    RequestId = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.Physicianid,
                    CreatedDate = item.Createddate,
                    RequestStatusLogId = item.Requeststatuslogid,
                    TransToAdmin = item.Transtoadmin,
                    TransToPhysicianId = item.Transtophysicianid
                });
            }
            vn.cancel = cancelrq;

            return vn;
        }
        #endregion

        #region Edit_notes
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID)
        {
            try
            {
                Requestnote notes = _context.Requestnotes.FirstOrDefault(E => E.Requestid == RequestID);
                if (notes != null)
                {
                    if (physiciannotes != null)
                    {
                        if (notes != null)
                        {
                            notes.Physiciannotes = physiciannotes;
                            notes.Modifieddate = DateTime.Now;
                            _context.Requestnotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (adminnotes != null)
                    {
                        if (notes != null)
                        {
                            notes.Adminnotes = adminnotes;
                            notes.Modifieddate = DateTime.Now;
                            _context.Requestnotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Requestnote rn = new Requestnote
                    {
                        Requestid = RequestID,
                        Adminnotes = adminnotes,
                        Physiciannotes = physiciannotes,
                        Createddate = DateTime.Now,
                        Createdby = "001e35a5 - cd12 - 4ec8 - a077 - 95db9d54da0f"
                    };
                    _context.Requestnotes.Add(rn);
                    _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region GetDocuments
        public async Task<ViewUploadModel> GetDocument(int? id, ViewUploadModel model)
        {
            var requests = _context.Requests.FirstOrDefault(req => req.Requestid == id);
            var requestClient = _context.Requestclients.FirstOrDefault(reqclient => reqclient.Requestid == id);
            List<Documents> result = (from requestWiseFile in _context.Requestwisefiles
                                      join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                                      join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                                      from phys in physicianGroup.DefaultIfEmpty()
                                      join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                                      from adm in adminGroup.DefaultIfEmpty()
                                      where request.Requestid == id && requestWiseFile.Isdeleted == new BitArray(1)
                                      select new Documents
                                      {
                                          Uploader = requestWiseFile.Physicianid != null ? phys.Firstname : (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                                          isDeleted = requestWiseFile.Isdeleted.ToString(),
                                          RequestwisefileId = requestWiseFile.Requestwisefileid,
                                          Status = requestWiseFile.Doctype,
                                          CreatedDate = requestWiseFile.Createddate,
                                          filename = requestWiseFile.Filename,
                                      }).ToList();
            int totalItemCount = result.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)model.PageSize);
            List<Documents> list1 = result.Skip((model.CurrentPage - 1) * model.PageSize).Take(model.PageSize).ToList();

            ViewUploadModel upload = new()
            {
                ConfirmationNumber = requests.Confirmationnumber,
                RequestId = requests.Requestid,
                FirstName = requestClient.Firstname,
                LastName = requestClient.Lastname,
                documents = list1,
                CurrentPage = model.CurrentPage,
                PageSize = model.PageSize,
                TotalItemCount = totalItemCount,
                TotalPages = totalPages
            };
            return upload;
        }
        #endregion

        #region UploadDocuments
        public Boolean UploadDocuments(int Requestid, IFormFile file)
        {
            string upload = SaveFileModel.UploadDocument(file, Requestid);
            var requestwisefile = new Requestwisefile
            {
                Requestid = Requestid,
                Filename = upload,
                Isdeleted = new BitArray(1),
                Adminid = 10,
                Createddate = DateTime.Now
            };
            _context.Requestwisefiles.Add(requestwisefile);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region DeleteDocuments
        public async Task<bool> DeleteDocuments(string ids)
        {
            List<int> delete = ids.Split(',').Select(int.Parse).ToList();
            foreach (int item in delete)
            {
                if (item > 0)
                {
                    var data = await _context.Requestwisefiles.Where(e => e.Requestwisefileid == item).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.Isdeleted[0] = true;
                        _context.Requestwisefiles.Update(data);
                        _context.SaveChanges();
                    }
                }
            }
            return true;
        }
        #endregion

        #region SendOrder
        public Healthprofessional SelectProfessionalById(int VendorId)
        {
            return _context.Healthprofessionals.FirstOrDefault(p => p.Vendorid == VendorId);
        }
        public bool SendOrders(SendOrderModel sendOrder)
        {
            try
            {
                Orderdetail od = new Orderdetail
                {
                    Requestid = sendOrder.RequestID,
                    Vendorid = sendOrder.VendorID,
                    Faxnumber = sendOrder.FaxNumber,
                    Email = sendOrder.Email,
                    Businesscontact = sendOrder.BusinessContact,
                    Prescription = sendOrder.Prescription,
                    Noofrefill = sendOrder.NoOfRefill,
                    Createddate = DateTime.Now,
                    Createdby = "02ae2720-3e7c-4fff-b83f-038f29013420"
                };
                _context.Orderdetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.Requestid == sendOrder.RequestID);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region SendAgreement
        public Boolean SendAgreement(int Requestid)
        {
            var r = _context.Requestclients.FirstOrDefault(E => E.Requestid == Requestid);
            var agreementUrl = "localhost:5171/SendAgreement?RequestID=" + Requestid;
            _emailConfiguration.SendMail(r.Email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree/ Disagree</a>");
            Emaillog emaillog = new();
            { 
                emaillog.Emailtemplate = "Outlook";
                emaillog.Subjectname = "Send Agreement";
                emaillog.Emailid = r.Email;
                emaillog.Requestid = r.Requestid;
                emaillog.Createdate = DateTime.Now;
                emaillog.Sentdate = DateTime.Now;
            }

            
            _context.Emaillogs.Add(emaillog);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region SendAgreementAccept
        public Boolean SendAgreementAccept(int RequestId)
        {
            var req = _context.Requests.Find(RequestId);
            if (req != null)
            {
                req.Status = 4;
                _context.Requests.Update(req);
                _context.SaveChanges();

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = RequestId;

                rsl.Status = 4;

                rsl.Createddate = DateTime.Now;

                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion

        #region SendAgreementReject
        public Boolean SendAgreementReject(int RequestId, string Notes)
        {
            var request = _context.Requests.Find(RequestId);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = RequestId;
                rsl.Status = 7;
                rsl.Notes = Notes;
                rsl.Createddate = DateTime.Now;
                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion

        #region CloseCase
        public CloseCaseModel GetRequestForCloseCase(int RequestID)
        {
            CloseCaseModel ccm = new CloseCaseModel();
            var result = from requestwisefile in _context.Requestwisefiles
                         join request in _context.Requests on requestwisefile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestwisefile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == RequestID
                         select new
                         {
                             uploader = requestwisefile.Physicianid != null ? phys.Firstname : (requestwisefile.Adminid != null ? adm.Firstname : request.Firstname),
                             requestwisefile.Filename,
                             requestwisefile.Createddate,
                             requestwisefile.Requestwisefileid
                         };
            List<Documents> docs = new List<Documents>();
            foreach (var item in result)
            {
                docs.Add(new Documents
                {
                    CreatedDate = item.Createddate,
                    Uploader = item.uploader,
                    filename = item.Filename,
                    RequestwisefileId = item.Requestwisefileid
                });
            }
            ccm.documents = docs;
            Request req = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
            ccm.FirstName = req.Firstname;
            ccm.LastName = req.Lastname;
            ccm.RequestID = req.Requestid;

            Requestclient requestclient = _context.Requestclients.FirstOrDefault(e => e.Requestid == RequestID);
            ccm.RC_FirstName = requestclient.Firstname;
            ccm.RC_LastName = requestclient.Lastname;
            ccm.RC_DateOfBirth = new DateTime((int)requestclient.Intyear, DateTime.ParseExact(requestclient.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)requestclient.Intdate);
            ccm.RC_PhoneNumber = requestclient.Phonenumber;
            ccm.RC_Email = requestclient.Email;
            return ccm;
        }
        #endregion

        #region EditCloseCase
        public bool EditCloseCase(CloseCaseModel model)
        {
            try
            {
                Requestclient requestclient = _context.Requestclients.FirstOrDefault(e => e.Requestid == model.RequestID);
                if(requestclient != null)
                {
                    requestclient.Phonenumber = model.RC_PhoneNumber;
                    requestclient.Email = model.RC_Email;
                    _context.Requestclients.Update(requestclient);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CloseCase(int RequestID)
        {
            try
            {
                var data = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if(data != null)
                {
                    data.Status = 9;
                    data.Modifieddate = DateTime.Now;
                    _context.Requests.Update(data);
                    _context.SaveChanges();
                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,
                        Status = 9,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        

        #region GetEncounterData
        public EncounterModel GetEncounterData(int RequestId)
        {
            var datareq = _context.Requestclients.FirstOrDefault(e => e.Requestid == RequestId);
            var Data = _context.Encounterforms.FirstOrDefault(e => e.Requestid == RequestId);
            DateTime? fd = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate);
            if (Data != null)
            {
                EncounterModel enc = new EncounterModel
                {
                    ABD = Data.Abd,
                    EncounterId = Data.Encounterformid,
                    Allergies = Data.Allergies,
                    BloodPressureD = Data.Bloodpressurediastolic,
                    BloodPressureS = Data.Bloodpressurediastolic,
                    Chest = Data.Chest,
                    CV = Data.Cv,
                    DateOfBirth = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                    Date = DateTime.Now,
                    Diagnosis = Data.Diagnosis,
                    Hr = Data.Hr,
                    HistoryOfMedical = Data.Medicalhistory,
                    Heent = Data.Heent,
                    Extr = Data.Extremities,
                    PhoneNumber = datareq.Phonenumber,
                    Email = datareq.Email,
                    HistoryOfIllness = Data.Historyofpresentillnessorinjury,
                    FirstName = datareq.Firstname,
                    LastName = datareq.Lastname,
                    Followup = Data.Followup,
                    Location = datareq.Location,
                    Medications = Data.Medications,
                    MedicationsDispensed = Data.Medicaldispensed,
                    Neuro = Data.Neuro,
                    O2 = Data.O2,
                    Other = Data.Other,
                    Pain = Data.Pain,
                    Procedures = Data.Procedures,
                    Isfinalize = Data.Isfinalize,
                    RequesId = RequestId,
                    Rr = Data.Rr,
                    Skin = Data.Skin,
                    Temp = Data.Temp,
                    Treatment = Data.TreatmentPlan
                };
                return enc;
            }
            else
            {
                if (datareq != null)
                {
                    EncounterModel enc = new EncounterModel
                    {
                        FirstName = datareq.Firstname,
                        PhoneNumber = datareq.Phonenumber,
                        LastName = datareq.Lastname,
                        Location = datareq.Location,
                        DateOfBirth = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                        Date = DateTime.Now,
                        RequesId = RequestId,
                        Email = datareq.Email,
                    };
                    return enc;
                }
                else
                {
                    return new EncounterModel();
                }
            }
        }
        #endregion GetEncounterData

        #region EditEncounterData
        public bool EditEncounterData(EncounterModel Data, string id)
        {
                
                if (Data.EncounterId == 0)
                {
                    Encounterform enc = new Encounterform
                    {
                        Abd = Data.ABD,
                        Encounterformid = (int)Data.EncounterId,
                        Allergies = Data.Allergies,
                        Bloodpressurediastolic = Data.BloodPressureD,
                        Bloodpressuresystolic = Data.BloodPressureS,
                        Chest = Data.Chest,
                        Cv = Data.CV,
                        Diagnosis = Data.Diagnosis,
                        Hr = Data.Hr,
                        Medicalhistory = Data.HistoryOfMedical,
                        Heent = Data.Heent,
                        Extremities = Data.Extr,
                        Historyofpresentillnessorinjury = Data.HistoryOfIllness,
                        Followup = Data.Followup,
                        Medications = Data.Medications,
                        Medicaldispensed = Data.MedicationsDispensed,
                        Neuro = Data.Neuro,
                        O2 = Data.O2,
                        Other = Data.Other,
                        Pain = Data.Pain,
                        Procedures = Data.Procedures,
                        Requestid = Data.RequesId,
                        Rr = Data.Rr,
                        Skin = Data.Skin,
                        Temp = Data.Temp,
                        TreatmentPlan = Data.Treatment,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                    };
                    _context.Encounterforms.Add(enc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var encdetails = _context.Encounterforms.FirstOrDefault(e => e.Encounterformid == Data.EncounterId);
                    if (encdetails != null)
                    {
                        encdetails.Abd = Data.ABD;
                        encdetails.Encounterformid = (int)Data.EncounterId;
                        encdetails.Allergies = Data.Allergies;
                        encdetails.Bloodpressurediastolic = Data.BloodPressureD;
                        encdetails.Bloodpressuresystolic = Data.BloodPressureS;
                        encdetails.Chest = Data.Chest;
                        encdetails.Cv = Data.CV;
                        encdetails.Diagnosis = Data.Diagnosis;
                        encdetails.Hr = Data.Hr;
                        encdetails.Medicalhistory = Data.HistoryOfMedical;
                        encdetails.Heent = Data.Heent;
                        encdetails.Extremities = Data.Extr;
                        encdetails.Historyofpresentillnessorinjury = Data.HistoryOfIllness;
                        encdetails.Followup = Data.Followup;
                        encdetails.Medications = Data.Medications;
                        encdetails.Medicaldispensed = Data.MedicationsDispensed;
                        encdetails.Neuro = Data.Neuro;
                        encdetails.O2 = Data.O2;
                        encdetails.Other = Data.Other;
                        encdetails.Pain = Data.Pain;
                        encdetails.Procedures = Data.Procedures;
                        encdetails.Requestid = Data.RequesId;
                        encdetails.Rr = Data.Rr;
                        encdetails.Skin = Data.Skin;
                        encdetails.Temp = Data.Temp;
                        encdetails.TreatmentPlan = Data.Treatment;
                        encdetails.Modifieddate = DateTime.Now;
                        _context.Encounterforms.Update(encdetails);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            

        }
        #endregion EditEncounterData

        #region SendLink
        public bool SendLink(string FirstName, string LastName, string Email, string PhoneNumber)
        {
            var baseUrl = "localhost:5171/CreateRequest/Index";
            _emailConfiguration.SendMail(Email, "Create New Request", FirstName + "" + LastName + "" + PhoneNumber + "" + $"<a href='{baseUrl}'>Create New Request</a>");
            Emaillog emaillog = new();
            {
                emaillog.Emailtemplate = "Outlook";
                emaillog.Subjectname = "Send Mail to patient for submit request";
                emaillog.Emailid = Email;
                emaillog.Createdate = DateTime.Now;
                emaillog.Sentdate = DateTime.Now;
            }


            _context.Emaillogs.Add(emaillog);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region AcceptPhysician
        public async Task<bool> AcceptPhysician(int requestid, string note, int ProviderId)
        {

            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == requestid);
            request.Status = 2;
            request.Accepteddate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new()
            {
                Requestid = requestid,
                Createddate = DateTime.Now,
                Status = 2,
                Notes = note,
                Transtophysicianid = ProviderId
            };
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion AcceptPhysician

        #region TransfertoAdmin
        public async Task<bool> TransfertoAdmin(int RequestID, string Note, int ProviderId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestID);
            request.Status = 1;
            request.Physicianid = null;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new()
            {
                Requestid = RequestID,
                Notes = Note,
                Createddate = DateTime.Now,
                Status = 1,

                Physicianid = ProviderId,
                Transtoadmin = new BitArray(1)
            };
            rsl.Transtoadmin[0] = true;
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion TransfertoAdmin

        #region CaseFinalized
        public bool CaseFinalized(EncounterModel Data)
        {
            try
            {
                var data = _context.Encounterforms.FirstOrDefault(e => e.Encounterformid == Data.EncounterId);
                data.Isfinalize = true;
                data.Modifieddate = DateTime.Now;
                _context.Encounterforms.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region Housecall
        public bool Housecall(int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.Requestid == RequestId);
            request.Status = 5;
            request.Modifieddate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = RequestId;
            rsl.Createddate = DateTime.Now;
            rsl.Status = 5;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Consult
        public bool Consult(int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.Requestid == RequestId);
            request.Status = 6;
            request.Modifieddate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = RequestId;
            rsl.Createddate = DateTime.Now;
            rsl.Status = 6;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region ConcludeCare
        public bool ConcludeCare(int RequestId, string Notes)
        {
            var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestId);
            requestData.Status = 8;
            requestData.Modifieddate = DateTime.Now;
            _context.Requests.Update(requestData);
            _context.SaveChanges();

            Requeststatuslog rsl = new()
            {
                Requestid = RequestId,
                Notes = Notes,
                Status = 8,
                Createddate = DateTime.Now
            };
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion ConcludeCare
    }

}