using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.PatientPanel
{
    public class ViewDataUserProfileModel
    {
        public int? Userid { get; set; }
        public string? Aspnetuserid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public BitArray? Ismobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Regionid { get; set; }
        public string? ZipCode { get; set; }
        public string? Strmonth { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Intyear { get; set; }
        public int? Intdate { get; set; }
        public string Createdby { get; set; } = null!;
        public DateTime Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public short? Status { get; set; }
        public string? Ip { get; set; }
    }
}
