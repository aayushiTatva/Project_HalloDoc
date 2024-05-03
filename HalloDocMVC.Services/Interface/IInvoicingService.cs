using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IInvoicingService
    {
        public List<PayrateModel> GetPayrateByProvider(int Id, PayrateModel model);
        public Task<bool> EditPayrate(PayrateModel pm, int categoryId, int id);
    }
}
