using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using System.Web.WebPages;

namespace HalloDocMVC.Controllers.AdminController
{
    public class InvoicingController : Controller
    {
        #region Configuration
        private readonly IInvoicingService _IInvoicingService;
        public InvoicingController (IInvoicingService invoicingService)
        {
            _IInvoicingService = invoicingService;
        }
        #endregion
        public IActionResult Index()
        {
            return View("../AdminPanel/Admin/Invoicing/Index");
        }
        public IActionResult Timesheets()
        {
            return View("../AdminPanel/Admin/Invoicing/Timesheets");
        }
        public async Task<IActionResult> Payrate(int Id)
        {
            var model = await _IInvoicingService.GetPayrateByProvider(Id);
            return View("../AdminPanel/Admin/Invoicing/Payrate", model);
        }

        public async Task<IActionResult> EditPayrate(PayrateModel pm,int categoryId, int id)
        {
            var model = await _IInvoicingService.EditPayrate(pm, categoryId, id);
            return View("../AdminPanel/Admin/Invoicing/Payrate", model);
        }
    }
}