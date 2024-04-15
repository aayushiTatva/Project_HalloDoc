using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SaveFileModel
    {
        public static string UploadDocument(IFormFile UploadFile, int Requestid)
        {
            string upload_path = null;
            if(UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\" + Requestid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
                string filenamewithpath = Path.Combine(path, newfilename);
                upload_path = FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename;
                using (var stream = new FileStream(filenamewithpath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return upload_path;
        }
    }
}
