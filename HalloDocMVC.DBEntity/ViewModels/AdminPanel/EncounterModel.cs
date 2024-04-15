using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class EncounterModel
    {
        public bool? Isfinalize { get; set; }
        public int EncounterId { get; set; }
        public int? RequesId { get; set; }
        public int? RequesClientId { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string? Location { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a contact number")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "History Of Illness is required")]
        public string? HistoryOfIllness { get; set; }
        [Required(ErrorMessage = "History Of Medical is required")]
        public string? HistoryOfMedical { get; set; }
        [Required(ErrorMessage = "Medications is required")]
        public string? Medications { get; set; }
        [Required(ErrorMessage = "Allergies is required")]
        public string? Allergies { get; set; }
        [Required(ErrorMessage = "Temp is required")]
        public string? Temp { get; set; }
        [Required(ErrorMessage = "HR is required")]
        public string? Hr { get; set; }
        [Required(ErrorMessage = "RR is required")]
        public string? Rr { get; set; }
        [Required(ErrorMessage = "Blood Pressure is required")]
        public string? BloodPressureS { get; set; }
        [Required(ErrorMessage = "Blood Pressure is required")]
        public string? BloodPressureD { get; set; }
        [Required(ErrorMessage = "O2 is required")]
        public string? O2 { get; set; }
        [Required(ErrorMessage = "Pain is required")]
        public string? Pain { get; set; }
        [Required(ErrorMessage = "Heent is required")]
        public string? Heent { get; set; }
        [Required(ErrorMessage = "CV is required")]
        public string? CV { get; set; }
        [Required(ErrorMessage = "Chest is required")]
        public string? Chest { get; set; }
        [Required(ErrorMessage = "ABD is required")]
        public string? ABD { get; set; }
        [Required(ErrorMessage = "Extr is required")]
        public string? Extr { get; set; }
        [Required(ErrorMessage = "Skin is required")]
        public string? Skin { get; set; }
        [Required(ErrorMessage = "Neuro is required")]
        public string? Neuro { get; set; }
        public string? Other { get; set; }
        [Required(ErrorMessage = "Diagnosis is required")]
        public string? Diagnosis { get; set; }
        [Required(ErrorMessage = "Treatment is required")]
        public string? Treatment { get; set; }
        [Required(ErrorMessage = "Medications Dispensed is required")]
        public string? MedicationsDispensed { get; set; }
        [Required(ErrorMessage = "Procedures are required")]
        public string? Procedures { get; set; }
        [Required(ErrorMessage = "Followup is required")]
        public string? Followup { get; set; }
    }
}