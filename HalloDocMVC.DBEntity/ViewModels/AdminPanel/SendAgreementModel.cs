using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SendAgreementModel
    {
        public int? RequestId { get; set; }
        public int? RegionId { get; set; }
        public int? ReasonId { get; set; }
        public int? TransferToPhysicianId { get; set; }
        public string? PatientName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? Reason { get; set; }
    }
}

