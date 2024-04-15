using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.DataModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly HalloDocContext _context;
        public AdminDashboard(HalloDocContext context)
        {
            _context = context;
        }
        public PaginationModel CardData()
        {
            return new PaginationModel()
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => r.Status == 4 || r.Status == 5).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => r.Status == 3 || r.Status == 7 || r.Status == 8).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count()
            };
        }
        public PaginationModel GetRequests(string Status, string Filter, PaginationModel pagination)
        {
            List<int> status = Status.Split(',').Select(int.Parse).ToList();
            List<int> filter = Filter.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                join reqClient in _context.Requestclients
                                                on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _context.Physicians
                                                on req.Physicianid equals phys.Physicianid into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _context.Regions
                                                on rc.Regionid equals reg.Regionid into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where status.Contains(req.Status) && filter.Contains(req.Requesttypeid) &&  (pagination.SearchInput == null ||
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
                                                    RequestorPhoneNumber = req.Phonenumber
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
    }
}

