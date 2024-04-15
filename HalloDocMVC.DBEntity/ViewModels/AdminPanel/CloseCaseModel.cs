using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;
namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class CloseCaseModel
    {
        public int RequestID { get; set; }
        public int RequestClientID { get; set; }
        public int RequestWiseFileID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RC_FirstName { get; set; }
        public string RC_LastName { get; set; }
        public DateTime RC_DateOfBirth { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a contact number")]
        public string RC_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string RC_Email { get; set; }
        public List<Documents> documents { get; set; } = null;
    }
}
