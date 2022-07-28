using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [HandleError]
    public class BoardController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();

        // GET: Board
        [Customexception]
        public ActionResult Leaderboard(int ddlyear = 0, int ddlmonth = 0)
        {
            try
            {

                Days Obj = new Days { ID = "1", Day = "Jan" };
                Days Obj1 = new Days { ID = "2", Day = "Feb" };
                Days Obj2 = new Days { ID = "3", Day = "Mar" };
                Days Obj3 = new Days { ID = "4", Day = "Apr" };
                Days Obj4 = new Days { ID = "5", Day = "May" };
                Days Obj5 = new Days { ID = "6", Day = "Jun" };
                Days Obj6 = new Days { ID = "7", Day = "Jul" };
                Days Obj7 = new Days { ID = "8", Day = "Aug" };
                Days Obj8 = new Days { ID = "9", Day = "Sep" };
                Days Obj9 = new Days { ID = "10", Day = "Oct" };
                Days Obj10 = new Days { ID = "11", Day = "Nov" };
                Days Obj11= new Days { ID = "12", Day = "Dec" };

                List<Days> Days = new List<Days>(new Days[] { Obj, Obj1, Obj2, Obj3, Obj4, Obj5, Obj6, Obj7, Obj8, Obj9, Obj10, Obj11 });
                ViewBag.Days = Days;

                int CurrentYear = DateTime.Now.Year;
                if (ddlyear == null || ddlyear == 0)
                {
                    DateTime myDateTime = DateTime.Now;
                    ddlyear = myDateTime.Year;
                    ViewBag.CurrentYear = CurrentYear;
                }
                else
                {
                    ViewBag.CurrentYear = ddlyear;
                }
                List<SelectListItem> ddlYears = new List<SelectListItem>();
                for (int i = 2019; i <= CurrentYear; i++)
                {
                    ddlYears.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                if(ddlmonth ==null )
                {
                    DateTime myDateTime = DateTime.Now;
                    ddlmonth = myDateTime.Month;
                    ViewBag.ddlmonth = ddlmonth;
                }
               

                ViewBag.CurrentMonth = ddlmonth;
                ViewBag.ddlYears = ddlYears;
                var LoaderBoard = deEntity.Sp_Get_Leader_Board(ddlyear, ddlmonth).ToList();
                ViewBag.LoaderBoard = LoaderBoard;

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error occur: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult ReloadLeaderBoard(int ddlyear = 0, int ddlmonth = 0)
        {

            return Json(new { url = Url.Action("Leaderboard", "Board", new { ddlyear = ddlyear, ddlmonth = ddlmonth }) });
        }
    }
}