using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class AdminProfileModel
    {
        public int AdminId { get; set; }
        public string? AspnetuserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public short? Status { get; set; }
        public int? RoleId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        [Compare("Email", ErrorMessage = "Email and Confirm Email must match")]
        public string? ConfirmEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AltPhoneNumber { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public int? RegionId { get; set; }
        public string? RegionsId { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public string? Ip { get; set; }
        public class Regions
        {
            public int? RegionId { get; set; }
            public string? RegionName { get; set; }
        }
        public List<Regions>? RegionIds { get; set; }
    }
}
