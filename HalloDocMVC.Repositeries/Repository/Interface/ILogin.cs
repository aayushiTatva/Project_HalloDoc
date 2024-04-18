using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface ILogin
    {
        Task<UserInformation> CheckAccessLogin(Aspnetuser aspNetUser);
        Task<bool> CheckRegisterEmail(string email);
        bool IsAccessGranted(int roleId, string menuName);
        public Task<bool> SavePassword(string email, string Password);
    }
}