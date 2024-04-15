using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SearchRecordsModel
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public int Requestor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public short Status { get; set; }
        public string PhysicianName { get; set; }
        public string CancelledByProviderNotes { get; set; }
        public string PhysicianNote { get; set; }
        public string AdminNotes { get; set; }
        public string PatientNotes { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IFormFile? FinalReport { get; set; }
        public int UserId { get; set; }
        public int VendorId { get; set; }
    }
}
