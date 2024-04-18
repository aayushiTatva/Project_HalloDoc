using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var user = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email );
            UserInformation admin = new UserInformation();
            if (user != null)
            {
                var hasher = new PasswordHasher<string>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(null, user.Passwordhash, aspNetUser.Passwordhash);
                if (result != PasswordVerificationResult.Success)
                {
                    return null;
                }
                else
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
                        admin.RoleId = (int)admindata.Roleid;
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
                        admin.RoleId = (int)admindata.Roleid;
                    }
                    return admin;
                }
            }
            else
            {
                return admin;
            }
        }
        #endregion CheckAccessLogin

        #region CheckRegisterEmail
        public async Task<bool> CheckRegisterEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if(!string.IsNullOrEmpty(email) && Regex.IsMatch(email, pattern))
            {
                var user = _context.Aspnetusers.FirstOrDefault(m => m.Email == email);
                if(user != null)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IsAccessGranted
        public bool IsAccessGranted(int roleId, string menuName)
        {
            IQueryable<int> menuIds = _context.Rolemenus.Where(e => e.Roleid == roleId).Select(e => e.Menuid);
            bool accessGranted = _context.Menus.Any(e => menuIds.Contains(e.Menuid) && e.Name == menuName);
            return accessGranted;
        }
        #endregion IsAccessGranted

        #region SavePassword
        public async Task<bool> SavePassword(string email, string Password)
        {
            try
            {
                Aspnetuser user = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == email);
                var hasher = new PasswordHasher<string>();
                user.Passwordhash = hasher.HashPassword(null, Password);
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
        #endregion
    }
}