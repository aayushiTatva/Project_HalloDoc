using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IConfirmationNumberService
    {
        public int GetCountOfTodayRequests();
        public string GetConfirmationNumber(string state, string firstname, string lastname);
    }
}
