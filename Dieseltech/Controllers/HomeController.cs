using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    
    [HandleError]
    public class HomeController : Controller
    {
        //private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        string qry = "";
        DataTable dt = new DataTable();
        private DieseltechEntities db = new DieseltechEntities();

        [Customexception]
        public ActionResult Index()
        {
            try
            {
                if (Session["User_id"] == null)
                {
                    return View("Index", "Account");
                }
                int i = Convert.ToInt32(Session["User_id"]);
                var user = from q in db.tblUsers where q.User_ID == i select q;
                var profile = from q in db.tblProfiles where q.User_ID == i select q;
                var ques = from q in db.tblProfiles join s in db.tblPackages on q.Packageid equals s.id where q.User_ID == i select s;
                DateTime date = Convert.ToDateTime(ques.FirstOrDefault().ValidUptodate);
                DateTime date2 = Convert.ToDateTime(user.FirstOrDefault().ValidUpto);
                DateTime date1 = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                if (date2 < date1)
                {
                    return RedirectToAction("AccountExpired");
                }
                Session["image"] = profile.FirstOrDefault().Image;
                Session["Menu"] = db.sp_getAccessMenusUser(Convert.ToInt32(Session["User_id"])).ToList();

                Session["CarrierInsuranceExpirationList"] = db.tblNotifications.Where(n=>n.NotificationType=="C").OrderBy(n=> n.IsRead).ThenByDescending(n=>n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId ==1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();


                ViewBag.ThemeColor = db.tblThemeColors.ToList();

                ViewBag.DashboardStats = db.Sp_Get_Dashboard_Stats().ToList();


                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }
        [Customexception]
        public ActionResult UpdateNotification(int Notificationid)
        {
            db.Database.ExecuteSqlCommand("update tblnotification set IsRead=1 Where NotificationId="+ Notificationid + "");
            return Json("1",JsonRequestBehavior.AllowGet);
        }

        [FilterConfig.AuthorizeActionFilter]
        [Customexception]
        public ActionResult ChangeThemeColor(string HexaColor)
        {
            qry = "Update tblThemeColor Set ColorName = " + HexaColor + "";
            ut.InsertUpdate(qry);
            //return Json("Index");
            qry = "select ColorName from tblThemeColor";
            Session["Color"] = ut.ExecuteScalar(qry);
            return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
        }
    }
}