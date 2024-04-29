using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class AccessService : IAccessService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<Menu> _menuRepository;
        private readonly IGenericRepository<Rolemenu> _roleMenuRepository;
        private readonly IGenericRepository<Aspnetuser> _aspNetUserRepository;
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<Admin> _adminRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;

        public AccessService(IGenericRepository<Role> roleRepository, IGenericRepository<Menu> menuRepository, IGenericRepository<Rolemenu> roleMenuRepository, IGenericRepository<Aspnetuser> aspNetUserRepository, IGenericRepository<Request> requestRepository, IGenericRepository<Admin> adminRepository, IGenericRepository<Physician> physicianRepository)
        {
            _roleRepository = roleRepository;
            _menuRepository = menuRepository;
            _roleMenuRepository = roleMenuRepository;
            _aspNetUserRepository = aspNetUserRepository;
            _requestRepository = requestRepository;
            _adminRepository = adminRepository;
            _physicianRepository = physicianRepository;
        }



        #region GetRoleAccessDetails
        public PaginationRoles GetRoleAccessDetails(PaginationRoles paginationRoles)
        {
            List<RoleByMenuModel> v = (from r in _roleRepository.GetAll()
                                       where r.Isdeleted == new BitArray(1)
                                       select new RoleByMenuModel
                                       {
                                           RoleId = r.Roleid,
                                           RoleName = r.Name,
                                           AccountType = r.Accounttype,
                                           CreatedBy = r.Createdby,
                                           CreatedDate = r.Createddate,
                                           ModifiedBy = r.Modifiedby,
                                           ModifiedDate = r.Modifieddate,
                                           Isdeleted = r.Isdeleted
                                       })
                                        .ToList();
            int totalCount = v.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRoles.PageSize);
            List<RoleByMenuModel> list = v.Skip((paginationRoles.CurrentPage - 1) * paginationRoles.PageSize).Take(paginationRoles.PageSize).ToList();

            PaginationRoles roles1 = new()
            {
                RolesList = list,
                CurrentPage = paginationRoles.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetRoleByMenus
        public async Task<RoleByMenuModel> GetRoleByMenu(int roleid)
        {
            var r = await _roleRepository.GetAll()
                        .Where(r => r.Roleid == roleid)
                        .Select(r => new RoleByMenuModel
                        {
                            AccountType = r.Accounttype,
                            CreatedBy = r.Createdby,
                            RoleId = r.Roleid,
                            RoleName = r.Name,
                            Isdeleted = r.Isdeleted
                        })
                        .FirstOrDefaultAsync();
            return r;
        }
        #endregion

        #region GetMenuByAccount
        public async Task<List<HalloDocMVC.DBEntity.DataModels.Menu>> GetMenuByAccount(short AccountType)
        {
            return await _menuRepository.GetAll().Where(r => r.Accounttype == AccountType).ToListAsync();
        }
        #endregion

        #region CheckMenuByRole
        public async Task<List<int>> CheckMenuByRole(int roleid)
        {
            return await _roleMenuRepository.GetAll()
                        .Where(r => r.Roleid == roleid)
                        .Select(r => r.Menuid)
                        .ToListAsync();
        }
        #endregion

        #region PostRoleMenu
        public async Task<bool> PostRoleMenu(RoleByMenuModel role, string Menusid, string ID)
        {
            try
            {
                Role check = await _roleRepository.GetAll().Where(r => r.Name == role.RoleName).FirstOrDefaultAsync();
                if (check == null && role != null && Menusid != null)
                {
                    Role r = new Role();
                    r.Name = role.RoleName;
                    r.Accounttype = role.AccountType;
                    r.Createdby = ID;
                    r.Createddate = DateTime.Now;
                    r.Isdeleted = new System.Collections.BitArray(1);
                    r.Isdeleted[0] = false;
                    _roleRepository.Add(r);

                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Rolemenu ar = new Rolemenu();
                        ar.Roleid = r.Roleid;
                        ar.Menuid = item;
                        _roleMenuRepository.Add(ar);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region PutRoleMenu
        public async Task<bool> PutRoleMenu(RoleByMenuModel role, string Menusid, string ID)
        {
            try
            {
                Role check = await _roleRepository.GetAll().Where(r => r.Roleid == role.RoleId).FirstOrDefaultAsync();
                if (check != null && role != null && Menusid != null)
                {
                    check.Name = role.RoleName;
                    check.Accounttype = role.AccountType;
                    check.Modifiedby = ID;
                    check.Modifieddate = DateTime.Now;
                    _roleRepository.Update(check);

                    List<int> regions = await CheckMenuByRole(check.Roleid);
                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        if (regions.Contains(item))
                        {
                            regions.Remove(item);
                        }
                        else
                        {
                            Rolemenu ar = new Rolemenu();
                            ar.Menuid = item;
                            ar.Roleid = check.Roleid;
                            _roleMenuRepository.Update(ar);

                            regions.Remove(item);
                        }
                    }
                    if (regions.Count > 0)
                    {
                        foreach (var item in regions)
                        {
                            Rolemenu ar = await _roleMenuRepository.GetAll().Where(r => r.Roleid == check.Roleid && r.Menuid == item).FirstAsync();
                            _roleMenuRepository.Remove(ar);

                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region DeletePhysician
        public async Task<bool> DeleteRoles(int roleid, string AdminID)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                if (roleid == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _roleRepository.GetAll()
                        .Where(W => W.Roleid == roleid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Isdeleted = bt;
                        DataForChange.Isdeleted[0] = true;
                        DataForChange.Modifieddate = DateTime.Now;
                        DataForChange.Modifiedby = AdminID;
                        _roleRepository.Update(DataForChange);

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

        #region GetAllUserDetails
        public PaginationUserAccess GetAllUserDetails(int? User, PaginationUserAccess paginationUserAccess)
        {
            List<UserAccessModel> userDetails = (from user in _aspNetUserRepository.GetAll()
                                                 join admin in _adminRepository.GetAll() on user.Id equals admin.Aspnetuserid into adminGroup
                                                 from admin in adminGroup.DefaultIfEmpty()
                                                 join physician in _physicianRepository.GetAll() on user.Id equals physician.Aspnetuserid into physicianGroup
                                                 from physician in physicianGroup.DefaultIfEmpty()
                                                 where (admin != null || physician != null) &&
                                                 (admin.Isdeleted == new BitArray(1) || physician.Isdeleted == new BitArray(1))
                                                 select new UserAccessModel
                                                 {
                                                     UserName = user.Username,
                                                     FirstName = admin != null ? admin.Firstname : (physician != null ? physician.Firstname : null),
                                                     IsAdmin = admin != null,
                                                     UserId = admin != null ? admin.Adminid : (physician != null ? physician.Physicianid : null),
                                                     AccountType = admin != null ? 1 : (physician != null ? 2 : null),
                                                     Status = admin != null ? admin.Status : (physician != null ? physician.Status : null),
                                                     PhoneNumber = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                                                     PhysicianId = physician.Physicianid
                                                 }).ToList();
            var result = userDetails.Select(u => new UserAccessModel
            {
                UserName = u.UserName,
                FirstName = u.FirstName,
                IsAdmin = u.IsAdmin,
                UserId = u.UserId,
                AccountType = u.AccountType,
                Status = u.Status,
                PhoneNumber = u.PhoneNumber,
                OpenRequest = u.PhysicianId.HasValue ? _requestRepository.GetAll().Count(r => r.Physicianid == u.PhysicianId) : 0
            }).ToList();
            if (User.HasValue)
            {
                switch (User.Value)
                {
                    case 1:
                        result = result.Where(u => u.IsAdmin).ToList();
                        break;
                    case 2:
                        result = result.Where(u => !u.IsAdmin).ToList();
                        break;
                    case 3:
                        break;
                }
            }
            int totalCount = result.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationUserAccess.PageSize);
            List<UserAccessModel> list = result.Skip((paginationUserAccess.CurrentPage - 1) * paginationUserAccess.PageSize).Take(paginationUserAccess.PageSize).ToList();

            PaginationUserAccess roles1 = new()
            {
                UsersAccessList = list,
                CurrentPage = paginationUserAccess.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion
    }
}
