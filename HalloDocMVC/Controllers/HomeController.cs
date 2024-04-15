using HalloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace HalloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        /*public IActionResult Index()
        {
            return View("../AdminPanel/Home/Login");
        }*/
        public IActionResult AdminIndex()
        {
            return View("../AdminPanel/Home/Login");
        }
        public IActionResult Index()
        {
            return View("../PatientPanel/Home/Index");
        }

        public IActionResult Profile()
        {
            return View("~/Views/AdminPanel/Admin/Profile/Index.cshtml");
        }
        public IActionResult ProfileK()
        {
            return View("~/Views/AdminPanel/Admin/Profile/Index1.cshtml");
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(string Email, string Passwordhash)
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HalloDocDB;User Id=postgres;Password=Aayushi03;Include Error Detail=True");
            string Query = "select * from aspnetusers au inner join aspnetuserroles aur on au.id = aur.userid inner join aspnetroles roles on aur.roleid = roles.id where email=@Email and passwordhash=@Passwordhash";
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Passwordhash", Passwordhash);
            NpgsqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            int numRows = dataTable.Rows.Count;
            if (numRows > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    HttpContext.Session.SetString("UserName", row["username"].ToString());
                    HttpContext.Session.SetString("UserID", row["Id"].ToString());
                    HttpContext.Session.SetString("RoleId", row["roleid"].ToString());
                }
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../AdminPanel//Home/Login");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AuthError()
        {
            return View("../AdminPanel/Home/AuthError");
        }*/
    }
}