using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IMyProfile
    {
        Task<AdminProfileModel> GetProfile(int UserId);
        Task<bool> ResetPassword(string Password, int AdminId);
        Task<bool> EditAdministratorInfo(AdminProfileModel adminProfile);
        Task<bool> EditBillingInfo(AdminProfileModel adminProfile);
        Task<bool> AdminPost(AdminProfileModel admindata, string AdminId);
    }
}
