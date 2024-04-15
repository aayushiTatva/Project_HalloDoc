using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class EmailLogModel
    {
        public int EmailLogId { get; set; }
        public string EmailTemplate { get; set; }
        public string Recipient { get; set; }
        public string SubjectName { get; set; }
        public string EmailId { get; set; }
        public string? ConfirmationNumber { get; set; }
        public string? FilePath { get; set; }
        public int? RoleId { get; set; }
        public int? RequestId { get; set; }
        public int? AdminId { get; set; }
        public int? PhysicianId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public BitArray? IsEmailSent { get; set; }
        public int? SentTries { get; set; }
        public int? Action { get; set; }
    }
}
