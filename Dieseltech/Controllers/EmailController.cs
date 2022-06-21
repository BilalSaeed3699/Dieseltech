using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [HandleError]
    public class EmailController : Controller
    {
        private DieseltechEntities db = new DieseltechEntities();
        Utility ut = new Utility();
        // GET: Email
        [Customexception]
        public ActionResult Index()
        {
            ViewBag.EmailSetting = db.tblEmailSettings.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult UpdateEmailSetting(string CCEmail, string Email,string Password, int Port, string Host , bool IsActives = false )
            //string Email, string Password, int Port,bool IsActives
        {
            string query = "";
            query = "Update tblEmailsetting set CCEmail ='"+ CCEmail + "',  Email='" + Email + "' , Password = '" + Password + "' ,  Port=" + Port + " ,SSLEnable='" + IsActives + "',Host ='"+ Host + "'";
            ut.InsertUpdate(query);
            ViewBag.EmailSetting = db.tblEmailSettings.ToList();
            return RedirectToAction("Index");
        }

        public ActionResult EmailHistory (string Filter, string Type)
        {

            try
            {
                Session["CarrierInsuranceExpirationList"] = db.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                if (Filter == null)
                {
                    //Filter = "0";
                    Filter = "A";
                }

                if (Type == null)
                {
                    //Type = "0";
                    Type = "M";
                }

                //View
                //LoadNumber = "2010001";
                ViewBag.EmailHistory = db.tblEmailHistories.ToList();
                ViewBag.LoadDetails = db.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                ViewBag.Type = Type;
                //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetails(LoadNumber).OrderBy(a => a.LoaderNumber).ToList();


                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Email History" + ex.Message;
            }
            return View();
        }
    }
}