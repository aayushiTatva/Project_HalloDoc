using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class ComboBox : IComboBox
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public ComboBox(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region ComboBoxRegions
        public async Task<List<ComboBoxRegion>> ComboBoxRegions()
        {
            return await _context.Regions.Select(region => new ComboBoxRegion()
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
            return await _context.Casetags.Select(ct => new ComboBoxCaseReason()
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
            var data = _context.Physicians
                .Where(r => r.Regionid == regionId)
                .OrderByDescending(r => r.Createddate).ToList();
            return data;
        }
        #endregion ProviderByRegion

        #region ComboBoxHealthProfessionalType
        public async Task<List<ComboBoxHealthProfessionalType>> ComboBoxHealthProfessionalType()
        {
            return await _context.Healthprofessionaltypes.Select(hpt => new ComboBoxHealthProfessionalType()
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
            return await _context.Healthprofessionals.Select(hp => new ComboBoxHealthProfession()
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
            var data = _context.Healthprofessionals
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
            return await _context.Aspnetroles.Select(role => new ComboBoxUserRole()
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
            return await _context.Roles.Where(r => r.Accounttype == 2).Select(role => new ComboBoxUserRole()
            {
                RoleId = (role.Roleid).ToString(),
                RoleName = role.Name
            }).ToListAsync();
        }
        #endregion

        #region AdminRoleComboBox
        public async Task<List<ComboBoxUserRole>> AdminRoleComboBox()
        {
            return await _context.Roles.Where(r => r.Accounttype == 1).Select(role => new ComboBoxUserRole()
            {
                RoleId = (role.Roleid).ToString(),
                RoleName = role.Name
            }).ToListAsync();
        }
        #endregion

    }
}


