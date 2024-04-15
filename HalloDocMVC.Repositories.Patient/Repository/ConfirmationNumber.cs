using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Patient.Repository
{
    public class ConfirmationNumber : IConfirmationNumber
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public ConfirmationNumber(HalloDocContext context)
        {
            _context = context;
        }
        #endregion

        #region GenerateConfirmationNumber
        public int GetCountOfTodayRequests()
        {
            var currentDate = DateTime.Now;
            return _context.Requests.Where(u => u.Createddate ==  currentDate).Count();
        }

        public string GetConfirmationNumber(string state, string firstname, string lastname)
        {
            state = (state.Length >= 2) ? state.Substring(0, 2).ToUpperInvariant() : state.PadRight(2, 'X');
            firstname = (firstname.Length >= 2) ? firstname.Substring(0, 2).ToUpperInvariant() : firstname.PadRight(2, 'X');
            lastname = (lastname.Length >= 2) ? lastname.Substring(0, 2).ToUpperInvariant() : lastname.PadRight(2, 'X');

            string Region = state.Substring(0, 2).ToUpperInvariant();
            string NameAbbr = lastname.Substring(0,2).ToUpperInvariant() + firstname.Substring(0,2).ToUpperInvariant();
            DateTime requestDateTime = DateTime.Now;
            string datepart = requestDateTime.ToString("ddMMyy");
            int requestCount = GetCountOfTodayRequests() + 1;
            string newRequestCount = requestCount.ToString("D4");
            string ConfirmationNumber = Region + datepart + NameAbbr + newRequestCount;

            return ConfirmationNumber;
        }
        #endregion
    }
}
