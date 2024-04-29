using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IAdminDashboardService
    {
        PaginationModel CardData(int ProviderId);
        PaginationModel GetRequests(string Status, string Filter, PaginationModel data, int ProviderId);
        public PaginationModel GetRequests(string Status, string Filter, PaginationModel pagination);
        public List<AdminDashboardList> Export(string status);
    }
}
