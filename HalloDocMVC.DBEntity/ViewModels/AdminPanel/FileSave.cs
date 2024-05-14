using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class FileSave
    {
        #region UploadFile
        public static string UploadDoc(IFormFile UploadFile, int RequestId)
        {
            string uploadPath = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\" + RequestId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
                string fileNameWithPath = Path.Combine(path, newfilename);
                uploadPath = FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return uploadPath;
        }
        #endregion

        #region UploadProviderDoc
        public static string UploadProviderDoc(IFormFile UploadFile,int PhysicianId, string FileName)
        {
            string uploadPath = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\Physician\\" + PhysicianId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string newfilename = FileName;

                string fileNameWithPath = Path.Combine(path, newfilename);
                uploadPath = FilePath.Replace("wwwroot\\Upload\\Physician\\", "/Upload/Physician/") + "/" + newfilename;

                using(var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return uploadPath;
        }
        #endregion

        #region UploadTimesheetDoc
        public static string UploadTimesheetDoc(IFormFile UploadFile, int TimeSheetId)
        {
            string newfilename = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\TimeSheet\\" + TimeSheetId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}"; ;

                string fileNameWithPath = Path.Combine(path, newfilename);
                //upload_path = FilePath.Replace("wwwroot\\Upload\\TimeSheet\\", "/Upload/TimeSheet/") + "/" + newfilename;


                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return newfilename;
        }
        #endregion

    }
}
