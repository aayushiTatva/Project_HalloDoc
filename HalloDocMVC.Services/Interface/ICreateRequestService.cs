using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface ICreateRequestService
    {
        Task<bool> CreatePatientRequest(ViewDataPatientRequestModel viewDataPatientRequest);
        Task<bool> CreateFamilyRequest(ViewDataFamilyRequestModel viewDataFamilyRequest);
        Task<bool> CreateConciergeRequest(ViewDataConciergeRequestModel viewDataConciergeRequest);
        Task<bool> CreateBusinessRequest(ViewDataBusinessRequestModel viewDataBusinessRequest);
    }
}
