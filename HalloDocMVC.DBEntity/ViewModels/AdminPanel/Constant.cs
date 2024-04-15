using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class Constant
    {
        public enum RequestType
        {
            All=0,
            Business = 1,
            Patient,
            Family,
            Concierge
        }
        public enum AdminDashboardStatus
        {
            New = 1,
            Pending,
            Active,
            Conclude,
            ToClose,
            Unpaid
        }
        public enum Status
        {
            Unassigne = 1,
            Accepted,
            Cancelled,
            MDEnRoute,
            MDONSite,
            Conclude,
            CancelledByPatients,
            Closed,
            Unpaid,
            Clear,
            Block

        }
        public enum AdminStatus
        {
            Active = 1,
            Pending,
            NotActive
        }
        public enum AccountType
        {
            All = 0,
            Admin,
            Physician,
            Patient
        }
    }
}