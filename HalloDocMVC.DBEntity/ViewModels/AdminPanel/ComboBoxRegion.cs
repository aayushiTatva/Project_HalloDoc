using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ComboBoxRegion
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
    }
}

