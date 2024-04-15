using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IAccess
    {
        public PaginationRoles GetRoleAccessDetails(PaginationRoles paginationRoles);
        public Task<RoleByMenuModel> GetRoleByMenu(int roleid);
        public Task<List<HalloDocMVC.DBEntity.DataModels.Menu>> GetMenuByAccount(short Accounttype);
        public Task<List<int>> CheckMenuByRole(int roleid);
        public Task<bool> PostRoleMenu(RoleByMenuModel role, string Menusid, string ID);
        public Task<bool> PutRoleMenu(RoleByMenuModel role, string Menusid, string ID);
        public Task<bool> DeleteRoles(int roleid, string AdminID);
        public PaginationUserAccess GetAllUserDetails(int? User, PaginationUserAccess paginationUserAccess);
    }
}
