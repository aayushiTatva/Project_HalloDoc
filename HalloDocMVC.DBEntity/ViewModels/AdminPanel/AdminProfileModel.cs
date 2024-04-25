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
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        public short? Status { get; set; }
        public int? RoleId { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "ConfirmEmail Is Required!")]
        [Compare("Email", ErrorMessage = "Email and Confirm Email must match")]
        public string? ConfirmEmail { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string? PhoneNumber { get; set; }
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string? AltPhoneNumber { get; set; }
        [Required(ErrorMessage = "Address1 is required")]
        public string? Address1 { get; set; }
        [Required(ErrorMessage = "Address2 is required")]
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        /*[Required(ErrorMessage = "State is required")]*/
        public string? State { get; set; }
        [Required(ErrorMessage = "Zipcode is required")]
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
