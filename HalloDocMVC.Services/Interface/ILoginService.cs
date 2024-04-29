using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface ILoginService
    {
        Task<UserInformation> CheckAccessLogin(Aspnetuser aspNetUser);
        Task<bool> CheckRegisterEmail(string email);
        bool IsAccessGranted(int roleId, string menuName);
        public Task<bool> SavePassword(string email, string Password);
        public Task<bool> CreateNewAccount(string Email, string Password);

    }
}
