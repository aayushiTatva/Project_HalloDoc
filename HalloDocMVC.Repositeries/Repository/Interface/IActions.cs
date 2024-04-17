//
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IActions
    {
        public ViewCaseModel GetRequestForViewCase(int id); 
        public bool EditCase(ViewCaseModel model);
        Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
        public bool BlockCase(int RequestID, string Note);
        public Task<bool> TransferPhysician(int RequestID,int ProviderId,  string Note);
        public bool ClearCase(int RequestID);
        public ViewNotesModel getNotes(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        Task<ViewUploadModel> GetDocument(int? id, ViewUploadModel model);
        public Boolean UploadDocuments(int Requestid, IFormFile formFile);
        Task<bool> DeleteDocuments(string ids);
        public Healthprofessional SelectProfessionalById(int VendorId);
        public bool SendOrders(SendOrderModel sendOrder);
        public Boolean SendAgreement(int Requestid);
        public Boolean SendAgreementAccept(int RequestId);
        public Boolean SendAgreementReject(int RequestId, string Notes);
        public CloseCaseModel GetRequestForCloseCase(int RequestID);
        public bool EditCloseCase(CloseCaseModel model);
        public bool CloseCase(int RequestID);
        EncounterModel GetEncounterData(int RequestId);
        bool EditEncounterData(EncounterModel Data, string id);
        bool SendLink(string FirstName, string LastName, string Email, string PhoneNumber);
    }
}
