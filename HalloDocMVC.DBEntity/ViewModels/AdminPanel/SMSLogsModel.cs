using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SMSLogsModel
    {
        public int SMSLogId { get; set; }
        public string SMSTemplate { get; set; }
        public string Recipient { get; set; }
        public string PhoneNumber { get; set; }
        public string? ConfirmationNumber { get; set; }
        public int? RoleId { get; set; }
        public int? RequestId { get; set; }
        public int? AdminId { get; set; }
        public int? PhysicianId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public BitArray? IsSMSSent { get; set; }
        public int? SentTries { get; set; }
        public int? Action { get; set; }
    }
}
