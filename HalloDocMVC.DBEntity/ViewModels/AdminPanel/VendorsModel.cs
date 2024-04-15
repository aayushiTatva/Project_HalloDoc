using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class VendorsModel
    {
        public int VendorId { get; set; }
        [Required(ErrorMessage = "Vendor Name is required")]
        public string VendorName { get; set;}
        [Required(ErrorMessage = "Fax Number is required")]
        public string FaxNumber { get; set;}
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set;}
        [Required(ErrorMessage = "City is required")]
        public string City { get; set;}
        [Required(ErrorMessage = "State is required")]
        public string State { get; set;}
        [Required(ErrorMessage = "ZipCode is required")]
        public string ZipCode { get; set;}
        public DateTime CreatedDate { get; set;}
        public DateTime? ModifiedDate { get; set;}
        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set;}
        public bool IsDeleted { get; set;}
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Profession is required")]
        public int ProfessionId { get; set;}
        [Required(ErrorMessage = "Business Contact is required")]
        public string BusinessContact { get; set; }
        [Required(ErrorMessage = "Business Name is required")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Profession is required")]
        public string ProfessionName { get; set; }
    }
    public class PaginationVendor
    {
        public List<VendorsModel> VendorList { get; set; }
        public int TotalPages { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 6;

    }
}
