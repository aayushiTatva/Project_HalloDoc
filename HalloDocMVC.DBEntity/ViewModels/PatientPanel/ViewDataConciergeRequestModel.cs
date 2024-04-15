using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.PatientPanel
{
    public class ViewDataConciergeRequestModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string CON_FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string CON_LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string CON_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string CON_Email { get; set; }
        public string CON_Location { get; set; }
        [Required(ErrorMessage = "Street is required")]
        public string CON_Street { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string CON_City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string CON_State { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        public string CON_Zipcode { get; set; }
        public string Id { get; set; } = null!;
        [Required(ErrorMessage = "Symptoms is required")] 
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a phone number")]
        public string PhoneNumber { get; set; }
        public string RoomSuite { get; set; }
    }
}
