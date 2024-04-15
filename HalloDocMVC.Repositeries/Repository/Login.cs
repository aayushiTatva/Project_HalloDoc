using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class Login : ILogin
    {
        #region Configuration
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HalloDocContext _context;
        public Login(HalloDocContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        #endregion Configuration

        #region CheckAccessLogin
        public async Task<UserInformation> CheckAccessLogin(Aspnetuser aspNetUser)
        {
            var user = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email && u.Passwordhash == aspNetUser.Passwordhash);
            UserInformation admin = new UserInformation();
            if (user != null)
            {
                var data = _context.Aspnetuserroles.FirstOrDefault(E => E.Userid == user.Id);
                var datarole = _context.Aspnetroles.FirstOrDefault(e => e.Id == data.Roleid);
                admin.UserName = user.Username;
                admin.FirstName = admin.FirstName ?? string.Empty;
                admin.LastName = admin.LastName ?? string.Empty;
                admin.Role = datarole.Name;
                admin.AspNetUserId = user.Id;
                if (admin.Role == "Admin")
                {
                    var admindata = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == user.Id);
                    admin.UserId = admindata.Adminid;
                }
                else if (admin.Role == "Patient")
                {
                    var admindata = _context.Users.FirstOrDefault(u => u.Aspnetuserid == user.Id);
                    admin.UserId = admindata.Userid;
                }
                else
                {
                    var admindata = _context.Physicians.FirstOrDefault(u => u.Aspnetuserid == user.Id);
                    admin.UserId = admindata.Physicianid;
                }
                return admin;
            }
            else
            {
                return admin;
            }
        }
        #endregion CheckAccessLogin
    }
}