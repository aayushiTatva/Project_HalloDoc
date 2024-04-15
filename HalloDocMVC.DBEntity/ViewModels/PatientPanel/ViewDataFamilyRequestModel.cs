using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.PatientPanel
{
    public class ViewDataFamilyRequestModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FF_FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string FF_LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string FF_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string FF_Email { get; set; }
        [Required(ErrorMessage = "Relation with patient is required")]
        public string FF_RelationWithPatient { get; set; }
        public string Id { get; set; } = null!;
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zipcode is required")]
        public string ZipCode { get; set; }
        public string RoomSuite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string UploadImage { get; set; }
    }
}
