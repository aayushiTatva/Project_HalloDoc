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
        public Task<List<PayrateModel>> GetPayrateByProvider(int Id);
    }
}
