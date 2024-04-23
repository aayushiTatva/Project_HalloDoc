using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class RecordsModel
    {
        public int UserId { get; set; }
        public string SearchInput { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatientName { get; set; }
        public short Status { get; set; }
        public int RequestType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PhysicianName { get; set; }
        public string Email { get; set; }
        public int EmailLogId { get; set; }
        public string PhoneNumber { get; set; }
        public int? AccountType { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime? SentDate { get; set; }
        public List<BlockRequestModel>? BlockedRequest { get; set; }
        public List<SMSLogsModel>? SMSLog { get; set; }
        public List<EmailLogModel>? EmailLog { get; set; }
        public List<SearchRecordsModel>? SearchRecords { get; set; }
        public List<User>? Users { get; set; }
        public int TotalPages { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 4;

    }
}
