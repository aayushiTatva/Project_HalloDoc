using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class LoginService : ILoginService
    {
        #region Configuration
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly IGenericRepository<Aspnetrole> _aspNetRoleRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<Requestclient> _requestClientRepository;
        private readonly IGenericRepository<Aspnetuserrole> _aspNetUserRoleRepository;
        private readonly IGenericRepository<Rolemenu> _roleMenuRepository;
        private readonly IGenericRepository<Admin> _adminRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Menu> _menuRepository;

        public LoginService(IGenericRepository<Aspnetuser> aspNetUserRepository, IGenericRepository<Aspnetrole> aspNetRoleRepository, IGenericRepository<User> userRepository, IGenericRepository<Request> requestRepository, IGenericRepository<Requestclient> requestClientRepository, IGenericRepository<Aspnetuserrole> aspNetUserRoleRepository, IGenericRepository<Rolemenu> roleMenuRepository, IGenericRepository<Admin> adminRepository, IGenericRepository<Physician> physicianRepository, IGenericRepository<Menu> menuRepository)
        {
            _aspNetUserRepository = aspNetUserRepository;
            _aspNetRoleRepository = aspNetRoleRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _aspNetUserRoleRepository = aspNetUserRoleRepository;
            _roleMenuRepository = roleMenuRepository;
            _adminRepository = adminRepository;
            _physicianRepository = physicianRepository;
            _menuRepository = menuRepository;
        }
        #endregion

        #region CheckAccessLogin
        public async Task<UserInformation> CheckAccessLogin(Aspnetuser aspNetUser)
        {
            var user = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(u => u.Email == aspNetUser.Email);
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
                    var data = _aspNetUserRoleRepository.GetAll().FirstOrDefault(E => E.Userid == user.Id);
                    var datarole = _aspNetRoleRepository.GetAll().FirstOrDefault(e => e.Id == data.Roleid);
                    admin.UserName = user.Username;
                    admin.FirstName = admin.FirstName ?? string.Empty;
                    admin.LastName = admin.LastName ?? string.Empty;
                    admin.Role = datarole.Name;
                    admin.AspNetUserId = user.Id;
                    if (admin.Role == "Admin")
                    {
                        var admindata = _adminRepository.GetAll().FirstOrDefault(u => u.Aspnetuserid == user.Id);
                        admin.UserId = admindata.Adminid;
                        admin.RoleId = (int)admindata.Roleid;
                    }
                    else if (admin.Role == "Patient")
                    {
                        var admindata = _userRepository.GetAll().FirstOrDefault(u => u.Aspnetuserid == user.Id);
                        admin.UserId = admindata.Userid;
                    }
                    else
                    {
                        var admindata = _physicianRepository.GetAll().FirstOrDefault(u => u.Aspnetuserid == user.Id);
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
            if (!string.IsNullOrEmpty(email) && Regex.IsMatch(email, pattern))
            {
                var user = _aspNetUserRepository.GetAll().FirstOrDefault(m => m.Email == email);
                if (user != null)
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
            IQueryable<int> menuIds = _roleMenuRepository.GetAll().Where(e => e.Roleid == roleId).Select(e => e.Menuid);
            bool accessGranted = _menuRepository.GetAll().Any(e => menuIds.Contains(e.Menuid) && e.Name == menuName);
            return accessGranted;
        }
        #endregion IsAccessGranted

        #region SavePassword
        public async Task<bool> SavePassword(string email, string Password)
        {
            try
            {
                Aspnetuser user = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(m => m.Email == email);
                var hasher = new PasswordHasher<string>();
                user.Passwordhash = hasher.HashPassword(null, Password);
                _aspNetUserRepository.Update(user);/*
                await _context.SaveChangesAsync();*/
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
        #endregion

        #region CreateAccount
        public async Task<bool> CreateNewAccount(string Email, string Password)
        {
            try
            {
                Guid id = Guid.NewGuid();
                var hasher = new PasswordHasher<string>();
                Aspnetuser aspnetuser = new()
                {
                    Id = id.ToString(),
                    Email = Email,
                    Passwordhash = hasher.HashPassword(null, Password),
                    Username = Email,
                    CreatedDate = DateTime.Now,
                };
                _aspNetUserRepository.Add(aspnetuser);
                /*await _context.SaveChangesAsync();*/
                var U = await _requestClientRepository.GetAll().FirstOrDefaultAsync(m => m.Email == Email);
                var User = new User
                {
                    Aspnetuserid = aspnetuser.Id,
                    Firstname = U.Firstname,
                    Lastname = U.Lastname,
                    Mobile = U.Phonenumber,
                    Intdate = U.Intdate,
                    Intyear = U.Intyear,
                    Strmonth = U.Strmonth,
                    Email = Email,
                    Createdby = aspnetuser.Id,
                    Createddate = DateTime.Now,
                    Isrequestwithemail = new BitArray(1),
                };
                _userRepository.Add(User);
                /*await _context.SaveChangesAsync();*/

                var aspnetuserroles = new Aspnetuserrole
                {
                    Userid = User.Aspnetuserid,
                    Roleid = "3"
                };
                _aspNetUserRoleRepository.Add(aspnetuserroles);
                /* _context.SaveChanges();*/

                var rc = _requestClientRepository.GetAll().Where(e => e.Email == Email).ToList();

                foreach (var r in rc)
                {
                    _requestRepository.GetAll().Where(n => n.Requestid == r.Requestid)
                   .ExecuteUpdate(s => s.SetProperty(
                       n => n.Userid,
                       n => User.Userid));
                }
                if (rc.Count > 0)
                {
                    User.Intdate = rc[0].Intdate;
                    User.Intyear = rc[0].Intyear;
                    User.Strmonth = rc[0].Strmonth;
                    _userRepository.Update(User);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
