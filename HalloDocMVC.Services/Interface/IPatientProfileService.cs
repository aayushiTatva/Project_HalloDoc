using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IPatientProfileService
    {
        public ViewDataUserProfileModel GetProfile();
        Task<bool> EditProfile(ViewDataUserProfileModel userprofile);
    }
}
