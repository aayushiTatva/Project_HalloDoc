using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class RoleByMenuModel
    {
        public int RoleId { get; set; }
        [Required(ErrorMessage = "RoleName is required")]
        public string RoleName { get; set; }
        public short AccountType { get; set; }
        public  string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public BitArray Isdeleted = null!;
        public List<Menu> Menus { get; set; }
        public class Menu
        {
            public int Menuid { get; set; }
            public string name { get; set; }
            public string Checked { get; set; }
        }

    }

    public class PaginationRoles
    {
        public List<RoleByMenuModel> RolesList { get; set; }
        public int TotalPages { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
