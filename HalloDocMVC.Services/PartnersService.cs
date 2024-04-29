using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class PartnersService : IPartnersService
    {
        #region Configuration
        private readonly IGenericRepository<Healthprofessional> _healthprofessionalRepository;
        private readonly IGenericRepository<Healthprofessionaltype> _healthprofessionaltypeRepository;

        public PartnersService(IGenericRepository<Healthprofessional> healthprofessionalRepository, IGenericRepository<Healthprofessionaltype> healthprofessionaltypeRepository)
        {
            _healthprofessionalRepository = healthprofessionalRepository;
            _healthprofessionaltypeRepository = healthprofessionaltypeRepository;
        }
        #endregion

        #region GetPartners
        public PaginationVendor GetPartners(int? ProfessionId, string? SearchInput, PaginationVendor paginationVendor)
        {
            List<VendorsModel> vendor = (from hp in _healthprofessionalRepository.GetAll()
                                         join hpt in _healthprofessionaltypeRepository.GetAll()
                                         on hp.Profession equals hpt.Healthprofessionalid into VendorGroup
                                         from v in VendorGroup.DefaultIfEmpty()
                                         where hp.Isdeleted == new BitArray(1) && (hp.Profession == ProfessionId || ProfessionId == null) &&
                                         (SearchInput == null || hp.Email.Contains(SearchInput))
                                         select new VendorsModel
                                         {
                                             VendorId = hp.Vendorid,
                                             VendorName = hp.Vendorname,
                                             FaxNumber = hp.Faxnumber,
                                             Address = hp.Address,
                                             City = hp.City,
                                             State = hp.State,
                                             ZipCode = hp.Zip,
                                             PhoneNumber = hp.Phonenumber,
                                             Email = hp.Email,
                                             BusinessContact = hp.Businesscontact,
                                             BusinessName = hp.Businessname,
                                             ProfessionName = v.Professionname
                                         }).ToList();
            int totalCount = vendor.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationVendor.PageSize);
            List<VendorsModel> list = vendor.Skip((paginationVendor.CurrentPage - 1) * paginationVendor.PageSize).Take(paginationVendor.PageSize).ToList();

            PaginationVendor roles1 = new()
            {
                VendorList = list,
                CurrentPage = paginationVendor.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }
        #endregion

        #region GetPartnerById
        public async Task<VendorsModel> GetPartnerById(int? Id)
        {
            var vendor = await (from hp in _healthprofessionalRepository.GetAll()
                                join hpt in _healthprofessionaltypeRepository.GetAll()
                                on hp.Profession equals hpt.Healthprofessionalid into VendorGroup
                                from v in VendorGroup.DefaultIfEmpty()
                                where hp.Isdeleted == new BitArray(1) && hp.Vendorid == Id
                                select new VendorsModel
                                {
                                    VendorId = hp.Vendorid,
                                    VendorName = hp.Vendorname,
                                    FaxNumber = hp.Faxnumber,
                                    Address = hp.Address,
                                    City = hp.City,
                                    State = hp.State,
                                    ZipCode = hp.Zip,
                                    PhoneNumber = hp.Phonenumber,
                                    Email = hp.Email,
                                    BusinessContact = hp.Businesscontact,
                                    BusinessName = hp.Businessname,
                                    ProfessionName = v.Professionname,
                                    ProfessionId = (int)hp.Profession
                                }).FirstOrDefaultAsync();
            return vendor;
        }
        #endregion

        #region EditVendorInfo
        public async Task<bool> EditVendorInfo(VendorsModel vendor)
        {
            if (vendor == null)
            {
                return false;
            }
            else
            {
                var DataForChange = await _healthprofessionalRepository.GetAll().Where(w => w.Vendorid == vendor.VendorId).FirstOrDefaultAsync();
                if (DataForChange != null)
                {
                    DataForChange.Businessname = vendor.BusinessName;
                    DataForChange.Profession = vendor.ProfessionId;
                    DataForChange.Faxnumber = vendor.FaxNumber;
                    DataForChange.Phonenumber = vendor.PhoneNumber;
                    DataForChange.Email = vendor.Email;
                    DataForChange.Businesscontact = vendor.BusinessContact;
                    DataForChange.Address = vendor.Address;
                    DataForChange.City = vendor.City;
                    DataForChange.State = vendor.State;
                    DataForChange.Zip = vendor.ZipCode;
                    _healthprofessionalRepository.Update(DataForChange);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region AddVendors
        public async Task<bool> AddVendor(VendorsModel data)
        {
            if (data.VendorId == 0)
            {
                Healthprofessional addhp = new()
                {
                    Vendorname = data.VendorName,
                    Profession = data.ProfessionId,
                    Faxnumber = data.FaxNumber,
                    Address = data.Address,
                    City = data.City,
                    State = data.State,
                    Zip = data.ZipCode,
                    Createddate = DateTime.Now,
                    Phonenumber = data.PhoneNumber,
                    Isdeleted = new BitArray(1),
                    Email = data.Email,
                    Businesscontact = data.BusinessContact,
                    Businessname = data.BusinessName
                };
                _healthprofessionalRepository.Add(addhp);

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region DeleteVendor
        public async Task<bool> DeleteVendor(int? VendorId)
        {
            if (VendorId != 0)
            {
                var data = _healthprofessionalRepository.GetAll().FirstOrDefault(v => v.Vendorid == VendorId);
                data.Isdeleted = new BitArray(1);
                data.Isdeleted[0] = true;
                _healthprofessionalRepository.Update(data);

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
