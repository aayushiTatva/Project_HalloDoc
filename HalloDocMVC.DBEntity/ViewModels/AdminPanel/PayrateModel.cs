using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class PayrateModel
    {
        public int PayrateId { get; set; }
        public int PayrateCategoryId { get; set; }
        public int PhysicianId { get; set; }
        public int Payrate { get; set; }
        public string PayrateCategoryName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<PayrateModel> PayrateList { get; set; }
    }
}
