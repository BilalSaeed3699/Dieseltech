using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    public class revenueController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        string qry = "";
        DataTable dt = new DataTable();
        // GET: revenue
        public ActionResult companyrevenue(int ddlAgentvalue=0, int Year = 0, int ddlmonth = 0, int CheckAjax = 0)
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

                Int32 userid = 0;
                if (Session["ddlAgent"] != null&& CheckAjax == 0&& ddlAgentvalue == 0)
                {
                    ddlAgentvalue = Convert.ToInt32(Session["ddlAgent"]);
                }


                if (ddlAgentvalue == 0 && CheckAjax == 0)
                {
                    userid = Convert.ToInt32(Session["User_id"]);
                    ddlAgentvalue = userid;
                }
                else
                {
                    userid = ddlAgentvalue;
                }


                string ddlAgentvalues = "";
                string Years = "";
                string ddlmonths = "";


                string Query = "";
                if (ddlAgentvalue != 0&& ddlAgentvalue != null)
                {
                    ddlAgentvalues = " and lh.User_ID =" + ddlAgentvalue + " ";
                }
                if (Year != 0&& Year != null)
                {
                    Years = " and year(lh.RegistrationDate) =" + Year + " ";
                }
                if (ddlmonth != 0&& ddlmonth != null)
                {
                    ddlmonths = " and month(lh.RegistrationDate) =" + ddlmonth + " ";
                }
                Query = " where 1=1   " + ddlAgentvalues + " " + Years + " " + ddlmonths + " ";
                

                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                ViewBag.UserId = userid;
                var CompanyRevenue = deEntity.Sp_Get_Company_Revenue(Query).ToList();
                ViewBag.CompanyRevenue = CompanyRevenue;


                var CompanySummaryRevenue = deEntity.Sp_Get_Company_Summary_Revenue(Query).ToList();
                ViewBag.CompanySummaryRevenue = CompanySummaryRevenue;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur : " + ex.Message;
            }
            return View();
        }


        [HttpPost]
        public ActionResult ReloadCompanyreveneu(int? ddlAgentvalue, int? Year = 0, int? ddlmonth = 0, int CheckAjax = 0)
        {

            return Json(new { url = Url.Action("companyrevenue", "revenue", new { ddlAgentvalue = ddlAgentvalue, Year= Year, ddlmonth= ddlmonth, CheckAjax = CheckAjax }) });
        }
        public ActionResult customerrevenue(int ddlAgentvalue=0, int Year = 0, int ddlmonth = 0, int CheckAjax = 0)
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

                //Int32 userid = 0;
                //if (Session["ddlAgent"] != null&& CheckAjax == 0&& ddlAgentvalue == 0)
                //{
                //    ddlAgentvalue = Convert.ToInt32(Session["ddlAgent"]);
                //}


                //if (ddlAgentvalue == 0 && CheckAjax == 0)
                //{
                //    userid = Convert.ToInt32(Session["User_id"]);
                //    ddlAgentvalue = userid;
                //}
                //else
                //{
                //    userid = ddlAgentvalue;
                //}


                //string ddlAgentvalues = "";
                string Years = "";
                string ddlmonths = "";


                string Query = "";
                //if (ddlAgentvalue != 0&& ddlAgentvalue != null)
                //{
                //    ddlAgentvalues = " and lh.User_ID =" + ddlAgentvalue + " ";
                //}
                if (Year != 0&& Year != null)
                {
                    Years = " and year(RegistrationDate) =" + Year + " ";
                }
                if (ddlmonth != 0&& ddlmonth != null)
                {
                    ddlmonths = " and month(RegistrationDate) =" + ddlmonth + " ";
                }
                Query = " where 1=1    " + Years + " " + ddlmonths + " ";
                

                //ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                //ViewBag.UserId = userid;
                List<Sp_Get_Customer_Revenue_Result> CustomerRevenue = deEntity.Sp_Get_Customer_Revenue(Query).ToList();
                ViewBag.CustomerRevenue = CustomerRevenue;


                //var CompanySummaryRevenue = deEntity.Sp_Get_Company_Summary_Revenue(Query).ToList();
                //ViewBag.CompanySummaryRevenue = CompanySummaryRevenue;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur : " + ex.Message;
            }
            return View();
        }


        [HttpPost]
        public ActionResult Reloadcustomerrevenue(int? ddlAgentvalue, int? Year = 0, int? ddlmonth = 0, int CheckAjax = 0)
        {

            return Json(new { url = Url.Action("customerrevenue", "revenue", new { ddlAgentvalue = ddlAgentvalue, Year= Year, ddlmonth= ddlmonth, CheckAjax = CheckAjax }) });
        }
    }
}