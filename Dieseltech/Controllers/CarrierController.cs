using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class CarrierController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        string qry = "";
        DataTable dt = new DataTable();
        // GET: Load
        // GET: Carrier
        [Customexception]
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        [Customexception]
        public ActionResult UpdateCarrierMark(int status,int companyid, string carrierid,string Year,string search )
        {
            string Query = "";
            try
            {

                TempData["JqueryTableSearchValue"] = search;
                TempData["carrierid"] = carrierid;
                Query = "Exec Sp_Update_Carrier_Marked  " + status + " , " + companyid + ", '"+ carrierid + "', '" + Year + "'";

                ut.InsertUpdate(Query);

                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [Customexception]
        public ActionResult AnnualTotal(int Year = 0, int ddlmonth = 0)
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
                Days Obj11 = new Days { ID = "12", Day = "Dec" };

                List<Days> Days = new List<Days>(new Days[] { Obj, Obj1, Obj2, Obj3, Obj4, Obj5, Obj6, Obj7, Obj8, Obj9, Obj10, Obj11 });
                ViewBag.Days = Days;

                if (TempData["JqueryTableSearchValue"] == null || TempData["JqueryTableSearchValue"].ToString() == "")
                {
                    ViewBag.JqueryTableSearchValue = "";
                }
                else
                {
                    ViewBag.JqueryTableSearchValue = TempData["JqueryTableSearchValue"].ToString();

                }

                if (TempData["carrierid"] == null || TempData["carrierid"].ToString() == "")
                {
                    ViewBag.carrierscroll = "";
                }
                else
                {
                    ViewBag.carrierscroll = TempData["carrierid"].ToString();

                }




                if (ddlmonth == null || ddlmonth == 0)
                {
                    DateTime myDateTime = DateTime.Now;
                    //ddlmonth = myDateTime.Month;
                    //ViewBag.ddlmonth = ddlmonth;
                }


                int CurrentYear = DateTime.Now.Year;
                if (Year == null || Year == 0)
                {
                    DateTime myDateTime = DateTime.Now;
                    Year = myDateTime.Year;
                    ViewBag.CurrentYear = CurrentYear;
                }
                else
                {
                    ViewBag.CurrentYear = Year;
                }

                var AnnualTotals = deEntity.Sp_Get_Annual_Totals(Year, ddlmonth).ToList();
                ViewBag.AnnualTotals = AnnualTotals;

               
               
                List<SelectListItem> ddlYears = new List<SelectListItem>();
                for (int i = 2019; i <= CurrentYear; i++)
                {
                    ddlYears.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                ViewBag.ddlYears = ddlYears;
                ViewBag.CurrentMonth = ddlmonth;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur : " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult ReloadAnnualTotal(int ddlyear = 0, int ddlmonth = 0)
        {

            return Json(new { url = Url.Action("AnnualTotal", "Carrier", new { Year = ddlyear, ddlmonth = ddlmonth }) });
        }
    }
}