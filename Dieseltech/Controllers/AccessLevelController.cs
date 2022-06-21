using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class AccessLevelController : Controller
    {
        
        private DieseltechEntities deEntity = new DieseltechEntities();
        // GET: AccessLevel
        [Customexception]
        public ActionResult Index()
        {
            try
            {
                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                ViewBag.POS = deEntity.tblAccess_Level.ToList();
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Access Level" + ex.Message;

            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Create(string Access_Level_Type)
        {
            Utility ul = new Utility();
            string query = "insert into tblAccess_Level values('" + Access_Level_Type + "')";
            ul.InsertUpdate(query);
            return RedirectToAction("Index");
        }
        [Customexception]
        public ActionResult Edit(int? id)
        {

            ViewBag.Menu = deEntity.sp_getAccessMenus(id).ToList();
            //  List<Menu> list = (List<Menu>)ViewBag.Menu;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccess_Level Access_Levels = deEntity.tblAccess_Level.Find(id);
            if (Access_Levels == null)
            {
                return HttpNotFound();
            }

            return View(Access_Levels);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Access_Level_Menu(int Access_ID, string Access_Level_Type, Array Menus)
        {

            Utility ul = new Utility();
            string query = "Update tblAccess_Level set Access_Level_Type='" + Access_Level_Type + "' where Access_ID=" + Access_ID;
            ul.InsertUpdate(query);
            query = "Delete from tblAccess_Level_Menu where Access_ID=" + Access_ID;
            ul.InsertUpdate(query);
            foreach (var customer in Menus)
            {
                query = "insert into tblAccess_Level_Menu values(" + Access_ID + "," + customer + ")";
                ul.InsertUpdate(query);
            }
            return RedirectToAction("Index");
        }
    }
}