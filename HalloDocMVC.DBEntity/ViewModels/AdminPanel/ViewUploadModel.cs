﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ViewUploadModel
    {
        public int RequestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<Documents> documents { get; set; }
        public class Documents
        {
            public short? Status { get; set; }
            public string? Uploader { get; set; }
            public string? filename { get; set; }
            public DateTime CreatedDate { get; set; }
            public int? RequestwisefileId { get; set; }
            public string isDeleted { get; set; }
        }
    }
}
