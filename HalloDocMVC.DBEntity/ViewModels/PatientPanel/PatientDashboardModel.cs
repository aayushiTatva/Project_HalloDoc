using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.PatientPanel
{
    public class PatientDashboardModel
    {
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public int RequestId { get; set; }
        public int DocumentCount { get; set; }
        public List<PatientDashboardModel>? PatientData { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItemCount { get; set; } = 0;
        public bool? IsAscending { get; set; } = false;
    }
}