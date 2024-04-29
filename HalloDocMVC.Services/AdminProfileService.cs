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
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.AdminProfileModel;

namespace HalloDocMVC.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        #region Configuration
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly IGenericRepository<Admin> _adminRepository;
        private readonly IGenericRepository<Aspnetuserrole> _aspNetUserRoleRepository;
        private readonly IGenericRepository<Region> _regionRepository;
        private readonly IGenericRepository<Adminregion> _adminRegionRepository;

        public AdminProfileService(IGenericRepository<Aspnetuser> iAspNetUserRepository, IGenericRepository<Admin> iAdminRepository,
        IGenericRepository<Aspnetuserrole> iAspNetUserRoleRepository, IGenericRepository<Region> iRegionRepository, IGenericRepository<Adminregion> iAdminRegionRepository)
        {
            _aspNetUserRepository = iAspNetUserRepository;
            _adminRepository = iAdminRepository;
            _aspNetUserRoleRepository = iAspNetUserRoleRepository;
            _regionRepository = iRegionRepository;
            _adminRegionRepository = iAdminRegionRepository;
        }
        #endregion

        #region GetProfile
        public async Task<AdminProfileModel> GetProfile(int UserId)
        {
            AdminProfileModel? profile = await (from req in _adminRepository.GetAll()
                                                join Aspnetuser in _aspNetUserRepository.GetAll()
                                                on req.Aspnetuserid equals Aspnetuser.Id into aspGroup
                                                from asp in aspGroup.DefaultIfEmpty()
                                                where req.Adminid == UserId
                                                select new AdminProfileModel
                                                {
                                                    AdminId = req.Adminid,
                                                    AspnetuserId = req.Aspnetuserid,
                                                    Status = req.Status,
                                                    RoleId = req.Roleid,
                                                    FirstName = req.Firstname,
                                                    LastName = req.Lastname,
                                                    Email = req.Email,
                                                    PhoneNumber = req.Mobile,
                                                    AltPhoneNumber = req.Altphone,
                                                    Address1 = req.Address1,
                                                    Address2 = req.Address2,
                                                    City = req.City,
                                                    State = req.State,
                                                    ZipCode = req.Zip,
                                                    RegionId = req.Regionid,
                                                    UserName = asp.Username,
                                                    Createdby = req.Createdby,
                                                    Createddate = req.Createddate,
                                                    Modifiedby = req.Modifiedby,
                                                    Modifieddate = req.Modifieddate
                                                })
                                         .FirstOrDefaultAsync();

            List<Regions> regions = new List<Regions>();
            regions = await _adminRegionRepository.GetAll()
                  .Where(r => r.Adminid == UserId)
                  .Select(req => new Regions()
                  {
                      RegionId = req.Regionid
                  })
                  .ToListAsync();
            profile.RegionIds = regions;
            return profile;
        }
        #endregion GetProfile

        #region ResetPassword
        public async Task<bool> ResetPassword(string Password, int AdminId)
        {
            var hasher = new PasswordHasher<string>();
            var request = await _adminRepository.GetAll().Where(a => a.Adminid == AdminId).FirstOrDefaultAsync();
            Aspnetuser? aspnetuser = await _aspNetUserRepository.GetAll().FirstOrDefaultAsync(u => u.Id == request.Aspnetuserid);
            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = hasher.HashPassword(null, Password);
                _aspNetUserRepository.Update(aspnetuser);

                return true;
            }
            return false;
        }
        #endregion

        #region EditAdministrationInformation
        public async Task<bool> EditAdministratorInfo(AdminProfileModel profile)
        {
            try
            {
                if (profile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _adminRepository.GetAll().Where(W => W.Adminid == profile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = profile.Email;
                        DataForChange.Firstname = profile.FirstName;
                        DataForChange.Lastname = profile.LastName;
                        DataForChange.Mobile = profile.PhoneNumber;
                        _adminRepository.Update(DataForChange);

                        List<int> regions = await _adminRegionRepository.GetAll().Where(r => r.Adminid == profile.AdminId).Select(req => req.Regionid).ToListAsync();
                        List<int> priceList = profile.RegionsId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                Adminregion ar = new()
                                {
                                    Regionid = item,
                                    Adminid = (int)profile.AdminId
                                };
                                _adminRegionRepository.Update(ar);

                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                Adminregion ar = await _adminRegionRepository.GetAll().Where(r => r.Adminid == profile.AdminId && r.Regionid == item).FirstAsync();
                                _adminRegionRepository.Remove(ar);

                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region BillingInformation
        public async Task<bool> EditBillingInfo(AdminProfileModel adminProfile)
        {
            try
            {
                if (adminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _adminRepository.GetAll().Where(W => W.Adminid == adminProfile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = adminProfile.Address1;
                        DataForChange.Address2 = adminProfile.Address2;
                        DataForChange.City = adminProfile.City;
                        DataForChange.State = adminProfile.State;
                        DataForChange.Altphone = adminProfile.AltPhoneNumber;
                        _adminRepository.Update(DataForChange);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Admin_Add
        public async Task<bool> AdminPost(AdminProfileModel admindata, string AdminId)
        {
            try
            {
                if (admindata.UserName != null && admindata.Password != null)
                {
                    //Aspnet_user
                    var Aspnetuser = new Aspnetuser();
                    var hasher = new PasswordHasher<string>();
                    Aspnetuser.Id = Guid.NewGuid().ToString();
                    Aspnetuser.Username = admindata.UserName;
                    Aspnetuser.Passwordhash = hasher.HashPassword(null, admindata.Password);
                    Aspnetuser.Email = admindata.Email;
                    Aspnetuser.CreatedDate = DateTime.Now;
                    _aspNetUserRepository.Add(Aspnetuser);


                    //aspnet_user_roles
                    var aspnetuserroles = new Aspnetuserrole();
                    aspnetuserroles.Userid = Aspnetuser.Id;
                    aspnetuserroles.Roleid = "Admin";
                    _aspNetUserRoleRepository.Add(aspnetuserroles);


                    //Admin
                    var Admin = new HalloDocMVC.DBEntity.DataModels.Admin();
                    Admin.Aspnetuserid = Aspnetuser.Id;
                    Admin.Firstname = admindata.FirstName;
                    Admin.Lastname = admindata.LastName;
                    Admin.Status = 1;
                    Admin.Roleid = admindata.RoleId;
                    Admin.Email = admindata.Email;
                    Admin.Mobile = admindata.PhoneNumber;
                    Admin.Isdeleted = new BitArray(1);
                    Admin.Isdeleted[0] = false;
                    Admin.Address1 = admindata.Address1;
                    Admin.Address2 = admindata.Address2;
                    Admin.City = admindata.City;
                    Admin.Zip = admindata.ZipCode;
                    Admin.Altphone = admindata.AltPhoneNumber;
                    Admin.Createddate = DateTime.Now;
                    Admin.Createdby = AdminId;
                    //Admin.Regionid = admindata.Regionid;
                    _adminRepository.Add(Admin);


                    //Admin_region
                    List<int> priceList = admindata.RegionsId.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Adminregion ar = new Adminregion();
                        ar.Regionid = item;
                        ar.Adminid = (int)Admin.Adminid;
                        _adminRegionRepository.Add(ar);

                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}
