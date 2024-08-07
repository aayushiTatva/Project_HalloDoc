﻿using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IProviderService
    {
        PaginationProvider GetContacts(PaginationProvider paginationProvider);
        PaginationProvider PhysicianByRegion(int? region, PaginationProvider paginationProvider);
        Task<bool> ChangeNotification(Dictionary<int, bool> changeValueDict);
        Task<ProviderModel> GetPhysicianById(int id);
        Task<bool> AddPhysician(ProviderModel physiciandata, string AdminId);
        Task<bool> EditPhysicianInfo(ProviderModel vm, string AdminId);
        Task<bool> EditAccountInfo(ProviderModel vm);
        Task<bool> ResetPassword(string Password, int PhysicianId);
        Task<bool> EditMailBillingInfo(ProviderModel vm, string AdminId);
        Task<bool> EditProviderProfile(ProviderModel vm, string AdminId);
        Task<bool> EditProviderOnbording(ProviderModel vm, string AdminId);
        Task<List<ProviderLocation>> FindPhysicianLocation();
        bool RequestToAdmin(int ProviderId, string Notes);
    }
}
