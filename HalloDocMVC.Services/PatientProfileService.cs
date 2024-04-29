using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class PatientProfileService : IPatientProfileService
    {
        #region Configuration
        private readonly IGenericRepository<User> _userRepository;

        public PatientProfileService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region GetProfile
        public ViewDataUserProfileModel GetProfile()
        {
            var userProfile = _userRepository.GetAll()
                                 .Where(r => r.Userid == Convert.ToInt32(CV.UserID()))
                                .Select(r => new ViewDataUserProfileModel
                                {
                                    Userid = r.Userid,
                                    FirstName = r.Firstname,
                                    LastName = r.Lastname,
                                    PhoneNumber = r.Mobile,
                                    Email = r.Email,
                                    Street = r.Street,
                                    State = r.State,
                                    City = r.City,
                                    ZipCode = r.Zipcode,
                                    DateOfBirth = new DateTime((int)r.Intyear, DateTime.ParseExact(r.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)r.Intdate)
                                })
                                .FirstOrDefault();

            return userProfile;
        }
        #endregion GetProfile

        #region Edit
        public Task<bool> EditProfile(ViewDataUserProfileModel userprofile)
        {
            User userToUpdate = _userRepository.GetAll().FirstOrDefault(e => e.Userid == userprofile.Userid);

            userToUpdate.Firstname = userprofile.FirstName;
            userToUpdate.Lastname = userprofile.LastName;
            userToUpdate.Mobile = userprofile.PhoneNumber;
            userToUpdate.Email = userprofile.Email;
            userToUpdate.State = userprofile.State;
            userToUpdate.Street = userprofile.Street;
            userToUpdate.City = userprofile.City;
            userToUpdate.Zipcode = userprofile.ZipCode;
            userToUpdate.Intdate = userprofile.DateOfBirth.Day;
            userToUpdate.Intyear = userprofile.DateOfBirth.Year;
            userToUpdate.Strmonth = userprofile.DateOfBirth.ToString("MMMM");
            userToUpdate.Modifiedby = userprofile.Createdby;
            userToUpdate.Modifieddate = DateTime.Now;
            _userRepository.Update(userToUpdate);
            return Task.FromResult(true);
        }
        #endregion 
    }
}
