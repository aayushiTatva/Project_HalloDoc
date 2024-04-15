using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Patient.Repository.Interface
{
    public interface IConfirmationNumber
    {
        public int GetCountOfTodayRequests();
        public string GetConfirmationNumber(string state, string firstname, string lastname);
    }
}
