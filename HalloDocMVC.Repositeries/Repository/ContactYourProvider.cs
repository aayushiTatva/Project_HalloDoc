﻿using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class ContactYourProvider : IContactYourProvider
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly EmailConfiguration _emailConfiguration;
        public ContactYourProvider(HalloDocContext context, IHttpContextAccessor httpContextAccessor,EmailConfiguration emailConfiguration)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            _emailConfiguration = emailConfiguration;
        }
        #endregion
        public PaginationProvider GetContacts(PaginationProvider paginationProvider)
        {
            List<ProviderModel> upload =  (from r in _context.Physicians
                                                join Notifications in _context.Physiciannotifications
                                                on r.Physicianid equals Notifications.Physicianid into aspGroup
                                                from nof in aspGroup.DefaultIfEmpty()
                                                join role in _context.Roles
                                                on r.Roleid equals role.Roleid into roleGroup
                                                from roles in roleGroup.DefaultIfEmpty()
                                                where r.Isdeleted == new BitArray(1)
                                                select new ProviderModel
                                                {
                                                    NotificationId = nof.Id,
                                                    CreatedDate = r.Createddate,
                                                    PhysicianId = r.Physicianid,
                                                    Address1 = r.Address1,
                                                    Address2 = r.Address2,
                                                    AltPhoneNumber = r.Altphone,
                                                    BusinessName = r.Businessname,
                                                    BusinessWebsite = r.Businesswebsite,
                                                    City = r.City,
                                                    FirstName = r.Firstname,
                                                    LastName = r.Lastname,
                                                    Notification = nof.Isnotificationstopped,
                                                    RoleName = roles.Name,
                                                    Status = r.Status,
                                                    Email = r.Email,
                                                    PhoneNumber = r.Mobile,
                                                    Isnondisclosuredoc = r.Isnondisclosuredoc == null ? false : true
                                                }).ToList();
            int totalCount = upload.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationProvider.PageSize);
            List<ProviderModel> list = upload.Skip((paginationProvider.CurrentPage - 1) * paginationProvider.PageSize).Take(paginationProvider.PageSize).ToList();

            PaginationProvider roles1 = new()
            {
                ProvidersList = list,
                CurrentPage = paginationProvider.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;



        }
        public PaginationProvider PhysicianByRegion(int? region, PaginationProvider paginationProvider)
        {
            List<ProviderModel> details = (from pr in _context.Physicianregions
                                                 join ph in _context.Physicians
                                                 on pr.Physicianid equals ph.Physicianid into rGroup
                                                 from r in rGroup.DefaultIfEmpty()
                                                 join Notifications in _context.Physiciannotifications
                                                 on r.Physicianid equals Notifications.Physicianid into aspGroup
                                                 from nof in aspGroup.DefaultIfEmpty()
                                                 join role in _context.Roles
                                                 on r.Roleid equals role.Roleid into roleGroup
                                                 from roles in roleGroup.DefaultIfEmpty()
                                                 where pr.Regionid == region
                                                 select new ProviderModel
                                                 {
                                                     CreatedDate = r.Createddate,
                                                     PhysicianId = r.Physicianid,
                                                     Address1 = r.Address1,
                                                     Address2 = r.Address2,
                                                     AdminNotes = r.Adminnotes,
                                                     AltPhoneNumber = r.Altphone,
                                                     BusinessName = r.Businessname,
                                                     BusinessWebsite = r.Businesswebsite,
                                                     City = r.City,
                                                     FirstName = r.Firstname,
                                                     LastName = r.Lastname,
                                                     Notification = nof.Isnotificationstopped,
                                                     RoleName = roles.Name,
                                                     Status = r.Status
                                                 }).ToList();
            int totalCount = details.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationProvider.PageSize);
            List<ProviderModel> list = details.Skip((paginationProvider.CurrentPage - 1) * paginationProvider.PageSize).Take(paginationProvider.PageSize).ToList();

            PaginationProvider roles1 = new()
            {
                ProvidersList = list,
                CurrentPage = paginationProvider.CurrentPage,
                TotalPages = totalPages
            };
            return roles1;
        }

        #region ChangeNotification
        public async Task<bool> ChangeNotification(Dictionary<int, bool> changeValueDict)
        {
            try
            {
                if (changeValueDict == null)
                {
                    return false;
                }
                else
                {
                    foreach(var item in changeValueDict)
                    {
                        var ar = _context.Physiciannotifications.Where(r => r.Physicianid == item.Key).FirstOrDefault();
                        if (ar != null)
                        {
                            ar.Isnotificationstopped[0] = item.Value;
                            _context.Physiciannotifications.Update(ar);
                            _context.SaveChanges();
                        }
                        else
                        {
                            Physiciannotification pn = new Physiciannotification();
                            pn.Physicianid = item.Key;
                            pn.Isnotificationstopped = new BitArray(1);
                            pn.Isnotificationstopped[0] = item.Value;
                            _context.Physiciannotifications.Add(pn);
                            _context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region GetPhysicianById
        public async Task<ProviderModel> GetPhysicianById(int id)
        {
            ProviderModel? pm = await(from r in _context.Physicians
                                      join Aspnetuser in _context.Aspnetusers
                                      on r.Aspnetuserid equals Aspnetuser.Id into aspGroup
                                      from asp in aspGroup.DefaultIfEmpty()
                                      join Notifications in _context.Physiciannotifications
                                      on r.Physicianid equals Notifications.Physicianid into PhyNGroup
                                      from nof in PhyNGroup.DefaultIfEmpty()
                                      join role in _context.Roles
                                      on r.Roleid equals role.Roleid into roleGroup
                                      from roles in roleGroup.DefaultIfEmpty()
                                      where r.Physicianid == id
                                      select new ProviderModel
                                      {
                                          UserName = asp.Username,
                                          RoleId = r.Roleid,
                                          Status = r.Status,
                                          NotificationId = nof.Id,
                                          CreatedDate = r.Createddate,
                                          PhysicianId = r.Physicianid,
                                          Address1 = r.Address1,
                                          Address2 = r.Address2,
                                          AdminNotes = r.Adminnotes,
                                          AltPhoneNumber = r.Altphone,
                                          BusinessName = r.Businessname,
                                          BusinessWebsite = r.Businesswebsite,
                                          City = r.City,
                                          FirstName = r.Firstname,
                                          LastName = r.Lastname,
                                          Notification = nof.Isnotificationstopped,
                                          RoleName = roles.Name,
                                          Email = r.Email,
                                          PhoneNumber = r.Mobile,
                                          Photo = r.Photo,
                                          Signature = r.Signature,
                                          Isagreementdoc = r.Isagreementdoc[0],
                                          Isnondisclosuredoc = r.Isnondisclosuredoc == false ? false : true,
                                          Isbackgrounddoc = r.Isbackgrounddoc[0],
                                          Islicencedoc = r.Islicensedoc[0],
                                          Istrainingdoc = r.Istrainingdoc[0],
                                          MedicalLicence = r.Medicallicense,
                                          Npinumber = r.Npinumber,
                                          Syncmailaddredss = r.Syncemailaddress,
                                          ZipCode = r.Zip,
                                          RegionId = r.Regionid

                                      }).FirstOrDefaultAsync();
            List<ProviderModel.Regions> regions = new List<ProviderModel.Regions>();

            regions = _context.Physicianregions
                  .Where(r => r.Physicianid == pm.PhysicianId)
                  .Select(req => new ProviderModel.Regions()
                  {
                      regionid = req.Regionid
                  })
                  .ToList();

            pm.Regionids = regions;
            return pm;
        }
        #endregion

        #region PhysicianAddEdit
        public async Task<bool> AddPhysician(ProviderModel providermodel,string AdminId)
        {
            try
            {
                if(providermodel.UserName != null && providermodel.Password != null)
                {
                    var Aspnetuser = new Aspnetuser();
                    var hasher = new PasswordHasher<string>();
                    Aspnetuser.Id = Guid.NewGuid().ToString();
                    Aspnetuser.Username = providermodel.UserName;
                    Aspnetuser.Passwordhash = hasher.HashPassword(null, providermodel.Password);
                    Aspnetuser.Email = providermodel.Email;
                    Aspnetuser.CreatedDate = DateTime.Now;
                    _context.Aspnetusers.Add(Aspnetuser);
                    _context.SaveChanges();

                    var aspnetuserroles = new Aspnetuserrole();
                    aspnetuserroles.Userid = Aspnetuser.Id;
                    aspnetuserroles.Roleid = "2";
                    _context.Aspnetuserroles.Add(aspnetuserroles);
                    _context.SaveChanges();

                    var Physician = new Physician();
                    Physician.Aspnetuserid = Aspnetuser.Id;
                    Physician.Firstname = providermodel.FirstName;
                    Physician.Lastname = providermodel.LastName;
                    Physician.Status = providermodel.Status;
                    Physician.Roleid = providermodel.RoleId;
                    Physician.Email = providermodel.Email;
                    Physician.Mobile = providermodel.PhoneNumber;
                    Physician.Medicallicense = providermodel.MedicalLicence;
                    Physician.Npinumber = providermodel.Npinumber;
                    Physician.Syncemailaddress = providermodel.Syncmailaddredss;
                    Physician.Address1 = providermodel.Address1;
                    Physician.Address2 = providermodel.Address2;
                    Physician.City = providermodel.City;
                    Physician.Zip = providermodel.ZipCode;
                    Physician.Altphone = providermodel.AltPhoneNumber;
                    Physician.Businessname = providermodel.BusinessName;
                    Physician.Businesswebsite = providermodel.BusinessWebsite;
                    Physician.Createddate = DateTime.Now;
                    Physician.Createdby = AdminId;
                    Physician.Regionid = providermodel.RegionId;
                    Physician.Isagreementdoc = new BitArray(1);
                    Physician.Isbackgrounddoc = new BitArray(1);
                    Physician.Isnondisclosuredoc = false;
                    Physician.Islicensedoc = new BitArray(1);
                    Physician.Istrainingdoc = new BitArray(1);
                    Physician.Isdeleted = new BitArray(1);
                    Physician.Isagreementdoc[0] = providermodel.Isagreementdoc;
                    Physician.Isbackgrounddoc[0] = providermodel.Isbackgrounddoc;
                    Physician.Isnondisclosuredoc = false;
                    Physician.Islicensedoc[0] = providermodel.Islicencedoc;
                    Physician.Istrainingdoc[0] = providermodel.Istrainingdoc;
                    Physician.Isdeleted[0] = false;
                    Physician.Adminnotes = providermodel.AdminNotes;
                    Physician.Photo = providermodel.PhotoFile != null ? Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(providermodel.PhotoFile.FileName).Trim('.') : null;
                    Physician.Signature = providermodel.SignatureFile != null ? Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png" : null;
                    _context.Physicians.Add(Physician);
                    _context.SaveChanges();

                    FileSave.UploadProviderDoc(providermodel.Agreementdoc, Physician.Physicianid, "Agreementdoc.pdf");
                    FileSave.UploadProviderDoc(providermodel.BackGrounddoc, Physician.Physicianid, "BackGrounddoc.pdf");
                    FileSave.UploadProviderDoc(providermodel.NonDisclosuredoc, Physician.Physicianid, "NonDisclosure.pdf");
                    FileSave.UploadProviderDoc(providermodel.Licensedoc, Physician.Physicianid, "Agreementdoc.pdf");
                    FileSave.UploadProviderDoc(providermodel.Trainingdoc, Physician.Physicianid, "Training.pdf");
                    FileSave.UploadProviderDoc(providermodel.SignatureFile, Physician.Physicianid, Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhss") + "-Signature.png");
                    FileSave.UploadProviderDoc(providermodel.PhotoFile, Physician.Physicianid, Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(providermodel.PhotoFile.FileName).Trim('.'));
                    List<int> priceList = providermodel.RegionsId.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Physicianregion pr = new Physicianregion();
                        pr.Regionid = item;
                        pr.Physicianid = (int)Physician.Physicianid;
                        _context.Physicianregions.Add(pr);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    
                }
                return true;
            }
            catch(Exception e)
            {

            }
            return false;
        }
        #endregion

        #region EditAccountInfo
        public async Task<bool> EditAccountInfo(ProviderModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.PhysicianId)
                        .FirstOrDefaultAsync();
                    Aspnetuser User = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Id == DataForChange.Aspnetuserid);
                    if (DataForChange != null && User != null)
                    {
                        User.Username = vm.UserName;
                        DataForChange.Status = vm.Status;
                        DataForChange.Roleid = vm.RoleId;
                        _context.Physicians.Update(DataForChange);
                        _context.Aspnetusers.Update(User);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region EditPhysicianInfo
        public async Task<bool> EditPhysicianInfo(ProviderModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.PhysicianId)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Firstname = vm.FirstName;
                        DataForChange.Lastname = vm.LastName;
                        DataForChange.Email = vm.Email;
                        DataForChange.Mobile = vm.PhoneNumber;
                        DataForChange.Medicallicense = vm.MedicalLicence;
                        DataForChange.Npinumber = vm.Npinumber;
                        DataForChange.Syncemailaddress = vm.Syncmailaddredss;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.Physicianregions.Where(r => r.Physicianid == vm.PhysicianId).Select(req => req.Regionid).ToListAsync();
                        List<int> priceList = vm.RegionsId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                Physicianregion pr = new()
                                {
                                    Regionid = item,
                                    Physicianid = (int)vm.PhysicianId
                                };
                                _context.Physicianregions.Update(pr);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                Physicianregion pr = await _context.Physicianregions.Where(r => r.Physicianid == vm.PhysicianId && r.Regionid == item).FirstAsync();
                                _context.Physicianregions.Remove(pr);
                                await _context.SaveChangesAsync();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EditMailingInfo
        public async Task<bool> EditMailBillingInfo(ProviderModel vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.PhysicianId)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = vm.Address1;
                        DataForChange.Address2 = vm.Address2;
                        DataForChange.City = vm.City;
                        DataForChange.Regionid = vm.RegionId;
                        DataForChange.Zip = vm.ZipCode;
                        DataForChange.Altphone = vm.AltPhoneNumber;
                        DataForChange.Modifiedby = AdminId;
                        DataForChange.Modifieddate = DateTime.Now;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EditProviderProfile
        public async Task<bool> EditProviderProfile(ProviderModel vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.PhysicianId)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        if (vm.PhotoFile != null)
                        {
                            DataForChange.Photo = vm.PhotoFile != null ? vm.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.') : null;
                            FileSave.UploadProviderDoc(vm.PhotoFile, (int)vm.PhysicianId, vm.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.'));

                        }
                        if (vm.SignatureFile != null)
                        {
                            DataForChange.Signature = vm.SignatureFile != null ? vm.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png" : null;
                            FileSave.UploadProviderDoc(vm.SignatureFile, (int)vm.PhysicianId, vm.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png");
                        }
                        DataForChange.Businessname = vm.BusinessName;
                        DataForChange.Businesswebsite = vm.BusinessWebsite;
                        DataForChange.Modifiedby = AdminId;
                        DataForChange.Adminnotes = vm.AdminNotes;
                        DataForChange.Modifieddate = DateTime.Now;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EditOnboardingProfile
        public async Task<bool> EditProviderOnbording(ProviderModel vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.PhysicianId)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        FileSave.UploadProviderDoc(vm.Agreementdoc, (int)vm.PhysicianId, "Agreementdoc.pdf");
                        FileSave.UploadProviderDoc(vm.BackGrounddoc, (int)vm.PhysicianId, "BackGrounddoc.pdf");
                        FileSave.UploadProviderDoc(vm.NonDisclosuredoc, (int)vm.PhysicianId, "NonDisclosuredoc.pdf");
                        FileSave.UploadProviderDoc(vm.Licensedoc, (int)vm.PhysicianId, "Agreementdoc.pdf");
                        FileSave.UploadProviderDoc(vm.Trainingdoc, (int)vm.PhysicianId, "Trainingdoc.pdf");

                        DataForChange.Isagreementdoc = new BitArray(1);
                        DataForChange.Isbackgrounddoc = new BitArray(1);
                        DataForChange.Isnondisclosuredoc = false;
                        DataForChange.Islicensedoc = new BitArray(1);
                        DataForChange.Istrainingdoc = new BitArray(1);

                        DataForChange.Isagreementdoc[0] = vm.Isagreementdoc;
                        DataForChange.Isbackgrounddoc[0] = vm.Isbackgrounddoc;
                        DataForChange.Isnondisclosuredoc = vm.Isnondisclosuredoc;
                        DataForChange.Islicensedoc[0] = vm.Islicencedoc;
                        DataForChange.Istrainingdoc[0] = vm.Istrainingdoc;
                        DataForChange.Modifiedby = AdminId;
                        DataForChange.Modifieddate = DateTime.Now;

                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region FindPhysicianLocation
        public async Task<List<ProviderLocation>> FindPhysicianLocation()
        {
            List<ProviderLocation> pl = await _context.Physicianlocations.OrderByDescending(x => x.Physicianname)
                        .Select(r => new ProviderLocation
                        {
                            LocationId = r.Locationid,
                            Longitude = r.Longitude,
                            Latitude = r.Latitude,
                            ProviderName = r.Physicianname
                        }).ToListAsync();
            return pl;
        }
        #endregion FindPhysicianLocation
    }
}