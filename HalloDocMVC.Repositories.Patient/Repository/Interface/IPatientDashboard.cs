using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Patient.Repository.Interface
{
    public interface IPatientDashboard
    {
        public PatientDashboardModel GetPatientData(string id, PatientDashboardModel model);
        Task<bool> UploadDoc(int RequestId, IFormFile? UploadFile);
        public ViewDataPatientRequestModel RequestForMe();
        Task<bool> PostMe(ViewDataPatientRequestModel viewpatientrequestforme);
        Task<bool> PostSomeoneElse(ViewDataPatientRequestModel viewpatientrequestforelse);
    }
}
