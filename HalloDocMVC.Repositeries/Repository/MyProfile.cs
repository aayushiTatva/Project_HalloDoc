using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.AdminProfileModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class MyProfile : IMyProfile
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public MyProfile(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region GetProfile
        public async Task<AdminProfileModel> GetProfile(int UserId)
        {
            AdminProfileModel? profile = await (from req in _context.Admins
                                           join Aspnetuser in _context.Aspnetusers
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
                                               State=req.State,
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
            regions = await _context.Adminregions
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

        /*EditAdministratorInfo*/
        #region ResetPassword
        public async Task<bool> ResetPassword(string Password, int AdminId)
        {
            var request = await _context.Admins.Where(a => a.Adminid == AdminId).FirstOrDefaultAsync();
            Aspnetuser? aspnetuser = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Id == request.Aspnetuserid);
            if(aspnetuser != null)
            {
                aspnetuser.Passwordhash = Password;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
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
                    var DataForChange = await _context.Admins.Where(W => W.Adminid == profile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = profile.Email;
                        DataForChange.Firstname = profile.FirstName;
                        DataForChange.Lastname = profile.LastName;
                        DataForChange.Mobile = profile.PhoneNumber;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.Adminregions.Where(r => r.Adminid == profile.AdminId).Select(req => req.Regionid).ToListAsync();
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
                                _context.Adminregions.Update(ar);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                Adminregion ar = await _context.Adminregions.Where(r => r.Adminid == profile.AdminId && r.Regionid == item).FirstAsync();
                                _context.Adminregions.Remove(ar);
                                await _context.SaveChangesAsync();
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
                if(adminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.Adminid == adminProfile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = adminProfile.Address1;
                        DataForChange.Address2 = adminProfile.Address2;
                        DataForChange.City = adminProfile.City;
                        DataForChange.State = adminProfile.State;
                        DataForChange.Mobile = adminProfile.PhoneNumber;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
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
                    _context.Aspnetusers.Add(Aspnetuser);
                    _context.SaveChanges();

                    //aspnet_user_roles
                    var aspnetuserroles = new Aspnetuserrole();
                    aspnetuserroles.Userid = Aspnetuser.Id;
                    aspnetuserroles.Roleid = "Admin";
                    _context.Aspnetuserroles.Add(aspnetuserroles);
                    _context.SaveChanges();

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
                    _context.Admins.Add(Admin);
                    _context.SaveChanges();

                    //Admin_region
                    List<int> priceList = admindata.RegionsId.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Adminregion ar = new Adminregion();
                        ar.Regionid = item;
                        ar.Adminid = (int)Admin.Adminid;
                        _context.Adminregions.Add(ar);
                        _context.SaveChanges();
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
