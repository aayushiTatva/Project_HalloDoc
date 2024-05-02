using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class InvoicingService : IInvoicingService
    {
        #region Configuration
        private readonly IGenericRepository<Request> _requestRepository;
        private readonly IGenericRepository<PayrateByProvider> _payrateByProviderRepository;
        private readonly IGenericRepository<PayrateCategory> _payrateCategoryRepository;
        private readonly IGenericRepository<Physician> _physicianRepository;

        public InvoicingService(IGenericRepository<Request> requestRepository, IGenericRepository<PayrateByProvider> payrateByProviderRepository,
            IGenericRepository<PayrateCategory> payrateCategoryRepository, IGenericRepository<Physician> physicianRepository)
        {
            _requestRepository = requestRepository;
            _payrateByProviderRepository = payrateByProviderRepository;
            _payrateCategoryRepository = payrateCategoryRepository;
            _physicianRepository = physicianRepository;
        }
        #endregion

        #region GetPayrateByProvider
        public async Task<PayrateModel> GetPayrateByProvider(int Id, PayrateModel model)
        {
            List<PayrateModel> model1 = (from pbp in _payrateByProviderRepository.GetAll()
                                        join pc in _payrateCategoryRepository.GetAll()
                                        on pbp.PayrateCategoryId equals pc.PayrateCategoryId into PayrateGroup
                                        from pg in PayrateGroup.DefaultIfEmpty()
                                        join phy in _physicianRepository.GetAll()
                                        on pbp.PhysicianId equals phy.Physicianid into PayratePhysicianGroup
                                        from phyGroup in PayratePhysicianGroup.DefaultIfEmpty()
                                        where pbp.PhysicianId == Id
                                        select new PayrateModel
                                        {
                                            Payrate = (int)pbp.Payrate,
                                            PayrateCategoryId = pbp.PayrateCategoryId,
                                            PayrateCategoryName = pg.CategoryName,
                                            PhysicianId = pbp.PhysicianId
                                        }).ToList();
            List<PayrateModel> roles = model1;
            return model;
        }
        #endregion

        #region EditPayrate
        public async Task<bool> EditPayrate(int payrate, int categoryId, int id)
        {
            var data = _payrateByProviderRepository.GetAll().FirstOrDefault(e => e.PhysicianId == id && e.PayrateCategoryId == categoryId);
            if(data != null)
            {
                data.PayrateCategoryId = categoryId;
                data.Payrate = payrate;
                _payrateByProviderRepository.Update(data);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
