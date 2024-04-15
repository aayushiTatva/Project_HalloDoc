using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class UserAccessModel
    {
        public int? UserId { get; set; }
        public  string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public short? AccountType { get; set; }
        public short? Status { get; set; }
        public int? OpenRequest { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public int? PhysicianId { get; set; }
    }

    public class PaginationUserAccess
    {
        public List<UserAccessModel> UsersAccessList { get; set; }
        public int TotalPages { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int role { get; set; }
    }
}
