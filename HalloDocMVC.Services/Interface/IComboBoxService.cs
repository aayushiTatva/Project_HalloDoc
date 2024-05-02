using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IComboBoxService
    {
        Task<List<ComboBoxRegion>> ComboBoxRegions();
        Task<List<ComboBoxCaseReason>> ComboBoxCaseReasons();
        List<Physician> ProviderByRegion(int? regionId);
        Task<List<ComboBoxHealthProfessionalType>> ComboBoxHealthProfessionalType();
        Task<List<ComboBoxProvider>> ComboBoxProvider();
        Task<List<ComboBoxHealthProfession>> ComboBoxHealthProfession();
        List<ComboBoxHealthProfession> ProfessionByType(int? HealthProfessionId);
        Task<List<ComboBoxUserRole>> ComboBoxUserRole();
        Task<List<ComboBoxUserRole>> PhysicianRoleComboBox();
        Task<List<ComboBoxUserRole>> AdminRoleComboBox();
    }
}
