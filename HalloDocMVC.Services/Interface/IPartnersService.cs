using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IPartnersService
    {
        public PaginationVendor GetPartners(int? ProfessionId, string? SearchInput, PaginationVendor paginationVendor);
        public Task<VendorsModel> GetPartnerById(int? Id);
        public Task<bool> EditVendorInfo(VendorsModel vendor);
        public Task<bool> AddVendor(VendorsModel data);
        public Task<bool> DeleteVendor(int? VendorId);
    }
}
