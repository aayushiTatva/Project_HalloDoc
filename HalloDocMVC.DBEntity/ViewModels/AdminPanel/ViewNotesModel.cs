using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ViewNotesModel
    {
        public int? RequestNotesId { get; set; }
        public int? RequestId { get; set; }
        public string? Strmonth { get; set; }
        public int? Intyear { get; set; }
        public int IntDate { get; set; }
        public string? AdminNotes { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? PatientNotes { get; set; }
        public string? CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public short Status { get; set; }
        public string? Ip { get; set; }
        public virtual Request Request { get; set; } = null!;
        public List<TransferNotesModel> transfernotes { get; set; } = null!;
        public List<TransferNotesModel> cancel { get; set; } = null!;
        public List<TransferNotesModel> cancelbyphysician { get; set; } = null!;
    }
    
}
