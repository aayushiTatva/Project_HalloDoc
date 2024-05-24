using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDocMVC.DBEntity.ViewModels;

namespace HalloDocMVC.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        #region Configuration
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<Requestclient> _requestClientRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Region> _regionRepository;
        private readonly IGenericRepository<Encounterform> _encounterRepository;
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly IGenericRepository<User> _userRepository;

        public AdminDashboardService(IGenericRepository<Request> iRequestRepository, IGenericRepository<Requestclient> iRequestClientRepository, IGenericRepository<Physician> iPhysicianRepository,
        IGenericRepository<Region> iRegionRepository, IGenericRepository<Encounterform> iEncounterRepository, IGenericRepository<Aspnetuser> iAspNetUserRepository,
        IGenericRepository<User> iUserRepository)
        {
            _requestRepository = iRequestRepository;
            _requestClientRepository = iRequestClientRepository;
            _physicianRepository = iPhysicianRepository;
            _regionRepository = iRegionRepository;
            _encounterRepository = iEncounterRepository;
            _aspNetUserRepository = iAspNetUserRepository;
            _userRepository = iUserRepository;
        }
        #endregion

        #region CardData
        public PaginationModel CardData(int ProviderId)
        {
            if (ProviderId < 0)
            {
                return new PaginationModel()
                {
                    NewRequest = _requestRepository.GetAll().Where(r => r.Status == 1 && r.Isdeleted == new BitArray(1)).Count(),
                    PendingRequest = _requestRepository.GetAll().Where(r => r.Status == 2 && r.Isdeleted == new BitArray(1)).Count(),
                    ActiveRequest = _requestRepository.GetAll().Where(r => (r.Status == 4 || r.Status == 5) && r.Isdeleted == new BitArray(1)).Count(),
                    ConcludeRequest = _requestRepository.GetAll().Where(r => r.Status == 6 && r.Isdeleted == new BitArray(1)).Count(),
                    ToCloseRequest = _requestRepository.GetAll().Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8) && r.Isdeleted == new BitArray(1)).Count(),
                    UnpaidRequest = _requestRepository.GetAll().Where(r => r.Status == 9 && r.Isdeleted == new BitArray(1)).Count()
                };
            }
            return new PaginationModel
            {
                NewRequest = _requestRepository.GetAll().Where(r => r.Status == 1 && r.Physicianid == ProviderId && r.Isdeleted == new BitArray(1)).Count(),
                PendingRequest = _requestRepository.GetAll().Where(r => r.Status == 2 && r.Physicianid == ProviderId && r.Isdeleted == new BitArray(1)).Count(),
                ActiveRequest = _requestRepository.GetAll().Where((r => (r.Status == 4 || r.Status == 5) && r.Physicianid == ProviderId && r.Isdeleted == new BitArray(1))).Count(),
                ConcludeRequest = _requestRepository.GetAll().Where(r => r.Status == 6 && r.Physicianid == ProviderId && r.Isdeleted == new BitArray(1)).Count(),
            };
        }
        #endregion

        #region GetRequests
        public PaginationModel GetRequests(string Status, string Filter, PaginationModel pagination)
        {
            List<int> status = Status.Split(',').Select(int.Parse).ToList();
            List<int> filter = Filter.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _requestRepository.GetAll()
                                                join reqClient in _requestClientRepository.GetAll()
                                                on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _physicianRepository.GetAll()
                                                on req.Physicianid equals phys.Physicianid into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _regionRepository.GetAll()
                                                on rc.Regionid equals reg.Regionid into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where req.Isdeleted == new BitArray(1) && status.Contains(req.Status) && filter.Contains(req.Requesttypeid) && (pagination.SearchInput == null ||
                                                        rc.Firstname.Contains(pagination.SearchInput) || rc.Lastname.Contains(pagination.SearchInput) ||
                                                        req.Firstname.Contains(pagination.SearchInput) || req.Lastname.Contains(pagination.SearchInput) ||
                                                        rc.Email.Contains(pagination.SearchInput) || rc.Phonenumber.Contains(pagination.SearchInput) ||
                                                        rc.Address.Contains(pagination.SearchInput) || rc.Notes.Contains(pagination.SearchInput) ||
                                                        p.Firstname.Contains(pagination.SearchInput) || p.Lastname.Contains(pagination.SearchInput) ||
                                                        rg.Name.Contains(pagination.SearchInput)) && (pagination.RegionId == null || rc.Regionid == pagination.RegionId)
                                                orderby req.Createddate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.Requestid,
                                                    RequestTypeId = req.Requesttypeid,
                                                    Requestor = req.Firstname + " " + req.Lastname,
                                                    PatientName = rc.Firstname + " " + rc.Lastname,
                                                    DateOfBirth = new DateTime((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                    RequestedDate = req.Createddate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderId = req.Physicianid,
                                                    ProviderName = p.Firstname + " " + p.Lastname,
                                                    PatientPhoneNumber = rc.Phonenumber,
                                                    Address = rc.Address + " " + rc.Street + " " + rc.City + " " + rc.State + " " + rc.Zipcode,
                                                    Notes = rc.Notes,
                                                    RequestorPhoneNumber = req.Phonenumber,
                                                    RequestorAspId = _aspNetUserRepository.GetAll().Where(e => e.Id == CV.ID()).FirstOrDefault().Id,
                                                    RequestAspId = _userRepository.GetAll().Where(e => e.Userid == req.Userid).FirstOrDefault().Aspnetuserid
                                                })
                                                   .ToList();
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagination.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();

            PaginationModel paginatedViewModel = new PaginationModel
            {
                list = list1,
                CurrentPage = pagination.CurrentPage,
                TotalPages = totalPages,
                SearchInput = pagination.SearchInput
            };
            return paginatedViewModel;
        }
        #endregion

        #region GetRequestsforProvider
        public PaginationModel GetRequests(string Status, string Filter, PaginationModel data, int ProviderId)
        {
            List<int> statusdata = Status.Split(',').Select(int.Parse).ToList();
            List<int> filter = Filter.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _requestRepository.GetAll()
                                                join reqClient in _requestClientRepository.GetAll()
                                                on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _physicianRepository.GetAll()
                                                on req.Physicianid equals phys.Physicianid into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _regionRepository.GetAll()
                                                on rc.Regionid equals reg.Regionid into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where req.Isdeleted == new BitArray(1) && statusdata.Contains(req.Status) && filter.Contains(req.Requesttypeid) && (req.Isdeleted == new BitArray(1)) && (data.SearchInput == null ||
                                                         rc.Firstname.Contains(data.SearchInput) || rc.Lastname.Contains(data.SearchInput) ||
                                                         req.Firstname.Contains(data.SearchInput) || req.Lastname.Contains(data.SearchInput) ||
                                                         rc.Email.Contains(data.SearchInput) || rc.Phonenumber.Contains(data.SearchInput) ||
                                                         rc.Address.Contains(data.SearchInput) || rc.Notes.Contains(data.SearchInput) ||
                                                         p.Firstname.Contains(data.SearchInput) || p.Lastname.Contains(data.SearchInput) ||
                                                         rg.Name.Contains(data.SearchInput)) && (data.RegionId == null || rc.Regionid == data.RegionId)
                                                         && (data.RequestType == null || req.Requesttypeid == data.RequestType) && req.Physicianid == ProviderId

                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.Requestid,
                                                    RequestTypeId = req.Requesttypeid,
                                                    Requestor = req.Firstname + " " + req.Lastname,
                                                    PatientName = rc.Firstname + " " + rc.Lastname,
                                                    DateOfBirth = new DateTime((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                    RequestedDate = req.Createddate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.Firstname + " " + p.Lastname,
                                                    PatientPhoneNumber = rc.Phonenumber,
                                                    ProviderEncounterStatus = req.Status,
                                                    IsFinalize = _encounterRepository.GetAll().Any(ef => ef.Requestid == req.Requestid && ef.Isfinalize),
                                                    Address = rc.Address,
                                                    Notes = rc.Notes,
                                                    ProviderId = req.Physicianid,
                                                    RequestorPhoneNumber = req.Phonenumber
                                                }).ToList();

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            PaginationModel paginatedViewModel = new()
            {
                list = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput,
                TotalItemCount = totalItemCount,
            };
            return paginatedViewModel;
        }
        #endregion

        #region Export
        public List<AdminDashboardList> Export(string status)
        {
            List<int> statusdata = status.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _requestRepository.GetAll()
                                                join reqClient in _requestClientRepository.GetAll()
                                                on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _physicianRepository.GetAll()
                                                on req.Physicianid equals phys.Physicianid into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _regionRepository.GetAll()
                                                on rc.Regionid equals reg.Regionid into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where statusdata.Contains(req.Status)
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.Requestid,
                                                    RequestTypeId = req.Requesttypeid,
                                                    Requestor = req.Firstname + " " + req.Lastname,
                                                    PatientName = rc.Firstname + " " + rc.Lastname,
                                                    DateOfBirth = new DateTime((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                    RequestedDate = req.Createddate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.Firstname + " " + p.Lastname,
                                                    PatientPhoneNumber = rc.Phonenumber,
                                                    Address = rc.Address,
                                                    Notes = rc.Notes,
                                                    ProviderId = req.Physicianid,
                                                    RequestorPhoneNumber = req.Phonenumber
                                                }).ToList();
            return allData;
        }
        #endregion
    }
}
