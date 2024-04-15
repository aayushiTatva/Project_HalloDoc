using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IContactYourProvider
    {
        PaginationProvider GetContacts(PaginationProvider paginationProvider);
        PaginationProvider PhysicianByRegion(int? region, PaginationProvider paginationProvider);
        Task<bool> ChangeNotification(Dictionary<int, bool> changeValueDict);
        Task<ProviderModel> GetPhysicianById(int id);
        Task<bool> AddPhysician(ProviderModel physiciandata, string AdminId);
        Task<bool> EditPhysicianInfo(ProviderModel vm);
        Task<bool> EditAccountInfo(ProviderModel vm);
        Task<bool> EditMailBillingInfo(ProviderModel vm, string AdminId);
        Task<bool> EditProviderProfile(ProviderModel vm, string AdminId);
        Task<bool> EditProviderOnbording(ProviderModel vm, string AdminId);
        Task<List<ProviderLocation>> FindPhysicianLocation();
    }
}
