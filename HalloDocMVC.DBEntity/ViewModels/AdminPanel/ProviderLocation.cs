using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ProviderLocation
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? ProviderName { get; set; }
        public int? LocationId { get; set; }
    }
}
