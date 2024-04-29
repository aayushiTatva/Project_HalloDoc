using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class ComboBoxService : IComboBoxService
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IGenericRepository<Physician> _physicianRepository;
        private readonly IGenericRepository<Aspnetrole> _aspNetRoleRepository;
        private readonly IGenericRepository<Casetag> _caseTagRepository;
        private readonly IGenericRepository<Healthprofessional> _healthprofessionalRepository;
        private readonly IGenericRepository<Healthprofessionaltype> _healthprofessionaltypeRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<Region> _regionRepository;

        public ComboBoxService(HalloDocContext context, IGenericRepository<Physician> physicianRepository, IGenericRepository<Aspnetrole> aspNetRoleRepository, IGenericRepository<Casetag> caseTagRepository, IGenericRepository<Healthprofessional> healthprofessionalRepository, IGenericRepository<Healthprofessionaltype> healthprofessionaltypeRepository, IGenericRepository<Role> roleRepository, IGenericRepository<Region> regionRepository)
        {
            _context = context;
            _physicianRepository = physicianRepository;
            _aspNetRoleRepository = aspNetRoleRepository;
            _caseTagRepository = caseTagRepository;
            _healthprofessionalRepository = healthprofessionalRepository;
            _healthprofessionaltypeRepository = healthprofessionaltypeRepository;
            _roleRepository = roleRepository;
            _regionRepository = regionRepository;
        }


        #endregion

        #region ComboBoxRegions
        public async Task<List<ComboBoxRegion>> ComboBoxRegions()
        {
            return await _regionRepository.GetAll().Select(region => new ComboBoxRegion()
            {
                RegionId = region.Regionid,
                RegionName = region.Name
            })
            .OrderBy(region => region.RegionName)
            .ToListAsync();
        }
        #endregion ComboBoxRegions

        #region ComboBoxCaseReasons
        public async Task<List<ComboBoxCaseReason>> ComboBoxCaseReasons()
        {
            return await _caseTagRepository.GetAll().Select(ct => new ComboBoxCaseReason()
            {
                CaseReasonId = ct.Casetagid,
                CaseReasonName = ct.Name
            })
            .OrderBy(ct => ct.CaseReasonName)
            .ToListAsync();
        }
        #endregion ComboBoxCaseReasons

        #region ProviderByRegion
        public List<Physician> ProviderByRegion(int? regionId)
        {
            var data = _physicianRepository.GetAll()
                .Where(r => r.Regionid == regionId)
                .OrderByDescending(r => r.Createddate).ToList();
            return data;
        }
        #endregion ProviderByRegion

        #region ComboBoxHealthProfessionalType
        public async Task<List<ComboBoxHealthProfessionalType>> ComboBoxHealthProfessionalType()
        {
            return await _healthprofessionaltypeRepository.GetAll().Select(hpt => new ComboBoxHealthProfessionalType()
            {
                HealthProfessionId = hpt.Healthprofessionalid,
                ProfessionName = hpt.Professionname
            })
            .OrderBy(hpt => hpt.ProfessionName)
            .ToListAsync();
        }
        #endregion ComboBoxHealthProfessionalType

        #region ComboBoxHealthProfession
        public async Task<List<ComboBoxHealthProfession>> ComboBoxHealthProfession()
        {
            return await _healthprofessionalRepository.GetAll().Select(hp => new ComboBoxHealthProfession()
            {
                VendorId = hp.Vendorid,
                VendorName = hp.Vendorname
            })
            .OrderBy(hp => hp.VendorName)
            .ToListAsync();
        }
        #endregion ComboBoxHealthProfession

        #region ProfessionByType
        public List<ComboBoxHealthProfession> ProfessionByType(int? HealthProfessionId)
        {
            var data = _healthprofessionalRepository.GetAll()
                        .Where(r => r.Profession == HealthProfessionId)
                        .Select(req => new ComboBoxHealthProfession()
                        {
                            VendorId = req.Vendorid,
                            VendorName = req.Vendorname
                        })
                        .ToList();
            return data;
        }
        #endregion ProfessionByType

        #region ComboBoxUserRole
        public async Task<List<ComboBoxUserRole>> ComboBoxUserRole()
        {
            return await _aspNetRoleRepository.GetAll().Select(role => new ComboBoxUserRole()
            {
                RoleId = role.Id,
                RoleName = role.Name
            })
            .OrderBy(role => role.RoleName)
            .ToListAsync();
        }
        #endregion ComboBoxUserRole

        #region PhysicianRoleComboBox
        public async Task<List<ComboBoxUserRole>> PhysicianRoleComboBox()
        {
            return await _roleRepository.GetAll().Where(r => r.Accounttype == 2).Select(role => new ComboBoxUserRole()
            {
                RoleId = (role.Roleid).ToString(),
                RoleName = role.Name
            }).ToListAsync();
        }
        #endregion

        #region AdminRoleComboBox
        public async Task<List<ComboBoxUserRole>> AdminRoleComboBox()
        {
            return await _roleRepository.GetAll().Where(r => r.Accounttype == 1).Select(role => new ComboBoxUserRole()
            {
                RoleId = (role.Roleid).ToString(),
                RoleName = role.Name
            }).ToListAsync();
        }
        #endregion
    }
}
