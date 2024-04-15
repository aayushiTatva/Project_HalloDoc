using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class BlockRequestModel
    {
        public int BlockRequestId { get; set; }
        public string? PatientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public BitArray? IsActive { get; set; }
        public string? Reason { get; set; }
        public int RequestId { get; set; }
        public string? Ip { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}