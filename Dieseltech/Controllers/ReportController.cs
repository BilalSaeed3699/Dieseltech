using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using HiQPdf;
using iTextSharp.text;
using Rotativa;

namespace Dieseltech.Controllers
{
    [HandleError]
    //[FilterConfig.AuthorizeActionFilter]
    public class ReportController : Controller
    {
        FormCollection m_formCollection;
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();



        [HttpPost]
        [Customexception]
        public ActionResult Advancesearch(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("Index", "Report", new { userid = userid, AdvanceFilterDateFroms = AdvanceFilterDateFroms, AdvanceFilterDateTo = AdvanceFilterDateTo, Filter = Filter, Type = Type, FilterDateFroms = FilterDateFroms, FilterDateTos = FilterDateTos, cwhere = cwhere, Custom = Custom }) });
        }


        [HttpPost]
        [Customexception]
        public ActionResult AdvanceLoaddetailsearch(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("LoadDetail", "Report", new { userid = userid, AdvanceFilterDateFroms = AdvanceFilterDateFroms, AdvanceFilterDateTo = AdvanceFilterDateTo, Filter = Filter, Type = Type, FilterDateFroms = FilterDateFroms, FilterDateTos = FilterDateTos, cwhere = cwhere, Custom = Custom }) });
        }

        [HttpPost]
        [Customexception]
        public ActionResult AdvanceInvoicingdetailsearch(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("InvoicingDetail", "Report", new { userid = userid, AdvanceFilterDateFroms = AdvanceFilterDateFroms, AdvanceFilterDateTo = AdvanceFilterDateTo, Filter = Filter, Type = Type, FilterDateFroms = FilterDateFroms, FilterDateTos = FilterDateTos, cwhere = cwhere, Custom = Custom }) });
        }



        [HttpPost]
        [Customexception]
        public ActionResult AgentLoadList(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("Index", "Report", new { userid, AdvanceFilterDateFroms, AdvanceFilterDateTo, previous, Next, Filter, Type, FilterDateFroms, FilterDateTos, cwhere, Custom }) });
        }

        [HttpPost]
        [Customexception]
        public ActionResult AgentLoadDetailsList(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("LoadDetail", "Report", new { userid, AdvanceFilterDateFroms, AdvanceFilterDateTo, previous, Next, Filter, Type, FilterDateFroms, FilterDateTos, cwhere, Custom }) });
        }


        [HttpPost]
        [Customexception]
        public ActionResult AgentInvoicingDetailsList(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {

            return Json(new { url = Url.Action("InvoicingDetail", "Report", new { userid, AdvanceFilterDateFroms, AdvanceFilterDateTo, previous, Next, Filter, Type, FilterDateFroms, FilterDateTos, cwhere, Custom }) });
        }

        // GET: Report
        [Customexception]
        public ActionResult Index(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)
        {
            int AccessLevel = 0;
            int LoginUserId = Convert.ToInt32(Session["User_id"]);
            try
            {
                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                if (userid == null)
                {
                    userid = Convert.ToInt32(Session["User_id"]);
                }
                ViewBag.UserId = userid;
                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                DateTime Daily;

                var Userinfo = deEntity.tblProfiles.Where(p => p.User_ID == LoginUserId).FirstOrDefault();

                if (Userinfo != null)
                {
                    AccessLevel = Convert.ToInt32(Userinfo.Accessid);
                }

                ViewBag.AccessLevel = AccessLevel;
                if (cwhere == null)
                {
                    cwhere = "";
                }
                ViewBag.cwhere = cwhere;

                //If Form is load first time 
                if (Filter == null)
                {
                    Filter = "A";
                }




                //If Form is already loaded and applied custom search 

                if (Custom == 1)
                {

                    string FullMonthName = FilterDateFroms.ToString("MMMM");
                    int year = FilterDateFroms.Year;

                    //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                    //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                    ViewBag.MonthDetails = FullMonthName + "  " + year;

                    ViewBag.LoadType = Type;
                    ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    ViewBag.LoadFilterDateTo = FilterDateTos;
                    ViewBag.IsDateFilter = "1";
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (cwhere != null && cwhere != "")
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", AdvanceFilterDateFroms, AdvanceFilterDateTo, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                    }
                    else
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, AdvanceFilterDateFroms, AdvanceFilterDateTo, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                    }
                }



                //If Form is load first time 
                if (previous == null && Next == null && Custom == null)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;
                    if (Type == null)
                    {

                        //ViewBag.LoadType = "Monthly";


                        ViewBag.LoadType = "Monthly";
                        FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;
                        string FullMonthName = Daily.ToString("MMMM");
                        int year = Daily.Year;
                        ViewBag.MonthDetails = FullMonthName + "  " + year;



                        FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        ViewBag.IsDateFilter = "1";



                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }





                        //Daily = DateTime.Now;
                        //ViewBag.LoadFilterDateFrom = Daily;
                        //ViewBag.LoadFilterDateTo = Daily;

                        ////DateTime now = DateTime.Now;
                        ////var startDate = new DateTime(now.Year, now.Month, 1);
                        ////var endDate = startDate.AddMonths(1).AddDays(-1);

                        ////ViewBag.DateFrom = startDate;
                        ////ViewBag.DateTo = endDate;


                        //ViewBag.IsDateFilter = "1";
                        ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, Daily, Daily).OrderBy(a => a.LoaderNumber).ToList();
                    }
                    else if (Type == "Daily")
                    {

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Weekly";

                            DateTime baseDate = DateTime.Today;
                            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";


                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //DateTime baseDate = DateTime.Today;
                            //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            //FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";


                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }





                    }

                    else if (Type == "Weekly")
                    {

                        //ViewBag.LoadType = "Monthly";

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Monthly";
                            FilterDateFroms = FilterDateFroms.AddDays(-30);

                            Daily = DateTime.Today;
                            string FullMonthName = Daily.ToString("MMMM");
                            int year = Daily.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";

                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }

                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //FilterDateFroms = FilterDateFroms.AddDays(-30);

                            //Daily = DateTime.Now;
                            //string FullMonthName = Daily.ToString("MMMM");
                            //int year = Daily.Year;
                            //ViewBag.MonthDetails = FullMonthName + "  " + year;
                            //FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            //FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);


                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";

                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }



                    }
                    else if (Type == "Monthly")
                    {
                        //ViewBag.LoadType = "Daily";



                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Daily";
                            Daily = DateTime.Today;
                            ViewBag.LoadFilterDateFrom = Daily;
                            ViewBag.LoadFilterDateTo = Daily;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;

                            FilterDateFroms = Daily;
                            FilterDateTos = Daily;
                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();


                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;
                            Daily = DateTime.Today;
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;


                            string FullMonthName = FilterDateFroms.ToString("MMMM");
                            int year = FilterDateFroms.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();


                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                            }

                        }
                    }
                }

                //If Form is already loaded  and previous button clicked 
                if (previous == 1)
                {

                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;



                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);




                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(-1);
                        FilterDateTos = FilterDateTos.AddDays(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }

                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(-7);
                        FilterDateTos = FilterDateTos.AddDays(-7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }
                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);


                        FilterDateFroms = FilterDateFroms.AddMonths(-1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }


                }

                //If Form is already loaded  and next  button clicked 
                if (Next == 1)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;

                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(1);
                        FilterDateTos = FilterDateTos.AddDays(1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }
                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(7);
                        FilterDateTos = FilterDateTos.AddDays(7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }


                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);


                        //FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays();
                        //FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter("S", FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();

                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, userid, cwhere).OrderBy(a => a.LoaderNumber).ToList();
                        }

                    }

                    //Daily = FilterDateFroms;
                    //FilterDateFroms = FilterDateFroms.AddDays(1);
                    //FilterDateTos = FilterDateTos.AddDays(1);


                    //ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    //ViewBag.LoadFilterDateTo = FilterDateFroms;

                    ////DateTime now = DateTime.Now;
                    ////var startDate = new DateTime(now.Year, now.Month, 1);
                    ////var endDate = startDate.AddMonths(1).AddDays(-1);

                    ////ViewBag.DateFrom = startDate;
                    ////ViewBag.DateTo = endDate;


                    //ViewBag.IsDateFilter = "1";
                    ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                    //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos,userid).OrderBy(a => a.LoaderNumber).ToList();

                }


                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Load List" + ex.Message;
            }


            //get Different dates from current date
            //DateTime baseDatess = DateTime.Today;

            //var today = baseDatess;
            //var yesterday = baseDate.AddDays(-1);
            //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            //var lastWeekStart = thisWeekStart.AddDays(-7);
            //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
            //var thisMonthStart = baseDatess.AddDays(1 - baseDatess.Day);
            //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            //var lastMonthStart = thisMonthStart.AddMonths(-1);
            //var lastMonthEnd = thisMonthStart.AddSeconds(-1);




            return View();


        }

        [Customexception]

        public ActionResult LoadDetail(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom)

        {


            try
            {
                int AccessLevel = 0;
                int LoginUserId = Convert.ToInt32(Session["User_id"]);
                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                if (userid == null)
                {
                    userid = Convert.ToInt32(Session["User_id"]);
                }
                ViewBag.UserId = userid;
                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                var Userinfo = deEntity.tblProfiles.Where(p => p.User_ID == LoginUserId).FirstOrDefault();

                if (Userinfo != null)
                {
                    AccessLevel = Convert.ToInt32(Userinfo.Accessid);
                }

                ViewBag.AccessLevel = AccessLevel;
                if (Filter == null)
                {
                    Filter = "A";
                }
                if (cwhere == null)
                {
                    cwhere = "";
                }
                ViewBag.cwhere = cwhere;



                //If Form is already loaded and applied custom search 

                if (Custom == 1)
                {

                    string FullMonthName = FilterDateFroms.ToString("MMMM");
                    int year = FilterDateFroms.Year;

                    //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                    //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                    ViewBag.MonthDetails = FullMonthName + "  " + year;

                    ViewBag.LoadType = Type;
                    ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    ViewBag.LoadFilterDateTo = FilterDateTos;
                    ViewBag.IsDateFilter = "1";
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (cwhere != null && cwhere != "")
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", AdvanceFilterDateFroms, AdvanceFilterDateTo, cwhere, userid).OrderBy(a => a.LoaderNumber).ToList();
                    }
                    else
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, AdvanceFilterDateFroms, AdvanceFilterDateTo, cwhere, userid).OrderBy(a => a.LoaderNumber).ToList();
                    }
                }


                DateTime Daily;
                if (previous == null && Next == null && Custom == null)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;
                    if (Type == null)
                    {

                        //ViewBag.LoadType = "Daily";

                        //Daily = DateTime.Now;
                        //ViewBag.LoadFilterDateFrom = Daily;
                        //ViewBag.LoadFilterDateTo = Daily;

                        ////DateTime now = DateTime.Now;
                        ////var startDate = new DateTime(now.Year, now.Month, 1);
                        ////var endDate = startDate.AddMonths(1).AddDays(-1);

                        ////ViewBag.DateFrom = startDate;
                        ////ViewBag.DateTo = endDate;


                        //ViewBag.IsDateFilter = "1";
                        ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        ///

                        ViewBag.LoadType = "Monthly";
                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;
                        string FullMonthName = Daily.ToString("MMMM");
                        int year = Daily.Year;
                        ViewBag.MonthDetails = FullMonthName + "  " + year;
                        FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        ViewBag.IsDateFilter = "1";

                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }
                    else if (Type == "Daily")
                    {

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Weekly";

                            DateTime baseDate = DateTime.Today;
                            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //DateTime baseDate = DateTime.Today;
                            //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            //FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }





                    }

                    else if (Type == "Weekly")
                    {

                        //ViewBag.LoadType = "Monthly";

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Monthly";
                            //FilterDateFroms = FilterDateFroms.AddDays(-30);

                            Daily = DateTime.Today;
                            string FullMonthName = Daily.ToString("MMMM");
                            int year = Daily.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //FilterDateFroms = FilterDateFroms.AddDays(-30);

                            //Daily = DateTime.Now;
                            //string FullMonthName = Daily.ToString("MMMM");
                            //int year = Daily.Year;
                            //ViewBag.MonthDetails = FullMonthName + "  " + year;
                            //FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            //FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);


                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }



                    }
                    else if (Type == "Monthly")
                    {
                        //ViewBag.LoadType = "Daily";



                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Daily";
                            Daily = DateTime.Today;
                            ViewBag.LoadFilterDateFrom = Daily;
                            ViewBag.LoadFilterDateTo = Daily;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;
                            FilterDateFroms = Daily;

                            FilterDateTos = Daily;
                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;
                            Daily = DateTime.Today;
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;


                            string FullMonthName = FilterDateFroms.ToString("MMMM");
                            int year = FilterDateFroms.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }




                    }



                }



                if (previous == 1)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;
                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;



                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);




                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(-1);
                        FilterDateTos = FilterDateTos.AddDays(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(-7);
                        FilterDateTos = FilterDateTos.AddDays(-7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }
                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);


                        FilterDateFroms = FilterDateFroms.AddMonths(-1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    ////Daily = FilterDateFroms;
                    //FilterDateFroms = FilterDateFroms.AddDays(-1);
                    //FilterDateTos = FilterDateTos.AddDays(-1);


                    //ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    //ViewBag.LoadFilterDateTo = FilterDateTos;

                    ////DateTime now = DateTime.Now;
                    ////var startDate = new DateTime(now.Year, now.Month, 1);
                    ////var endDate = startDate.AddMonths(1).AddDays(-1);

                    ////ViewBag.DateFrom = startDate;
                    ////ViewBag.DateTo = endDate;


                    //ViewBag.IsDateFilter = "1";
                    ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                    //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos,userid).OrderBy(a => a.LoaderNumber).ToList();

                }

                if (Next == 1)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;

                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(1);
                        FilterDateTos = FilterDateTos.AddDays(1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }
                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(7);
                        FilterDateTos = FilterDateTos.AddDays(7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }


                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Today;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);


                        //FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays();
                        //FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_PickupDelivery_WithAllFilter(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    //Daily = FilterDateFroms;
                    //FilterDateFroms = FilterDateFroms.AddDays(1);
                    //FilterDateTos = FilterDateTos.AddDays(1);


                    //ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    //ViewBag.LoadFilterDateTo = FilterDateFroms;

                    ////DateTime now = DateTime.Now;
                    ////var startDate = new DateTime(now.Year, now.Month, 1);
                    ////var endDate = startDate.AddMonths(1).AddDays(-1);

                    ////ViewBag.DateFrom = startDate;
                    ////ViewBag.DateTo = endDate;


                    //ViewBag.IsDateFilter = "1";
                    ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                    //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos,userid).OrderBy(a => a.LoaderNumber).ToList();

                }








                //if (FilterDateFroms !=null)
                //{
                //    Daily = DateTime.Now;

                //    ViewBag.Daily = Daily.ToString("MM/dd/yyyy");
                //    Daily = Convert.ToDateTime(FilterDateFroms);
                //    Daily = Daily.AddDays(-1);

                //    ViewBag.Daily = Daily;
                //    var date = Request.Form["FilterDateFroms"];
                //}



                //if (Next == 1)
                //{
                //    Daily = Convert.ToDateTime(FilterDateFroms);
                //    Daily = Daily.AddDays(1);
                //    ViewBag.Daily = Daily;
                //}


                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = Filter;
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";

                //if (Filter == "2")
                //{
                //    DateTime DateFrom = Convert.ToDateTime(Request.Form["FilterDateFrom"]);
                //    DateTime DateTo = Convert.ToDateTime(Request.Form["FilterDateTo"]);

                //    ViewBag.DateFrom = DateFrom;
                //    ViewBag.DateTo = DateTo;

                //    ViewBag.IsDateFilter = "1";
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, DateFrom, DateTo).OrderBy(a => a.LoaderNumber).ToList();
                //}
                //else
                //{
                //    DateTime now = DateTime.Now;
                //    var startDate = new DateTime(now.Year, now.Month, 1);
                //    var endDate = startDate.AddMonths(1).AddDays(-1);

                //    ViewBag.DateFrom = startDate;
                //    ViewBag.DateTo = endDate;


                //    ViewBag.IsDateFilter = "0";
                //    ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, startDate, endDate).OrderBy(a => a.LoaderNumber).ToList();
                //}

                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;


                return View();




                //Old Pattern
                //DateTime Daily = DateTime.Now;

                //ViewBag.Daily = Daily.ToString("MM/dd/yyyy");
                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = Filter;
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";

                //if (Filter == "2")
                //{
                //    DateTime DateFrom = Convert.ToDateTime(Request.Form["FilterDateFrom"]);
                //    DateTime DateTo = Convert.ToDateTime(Request.Form["FilterDateTo"]);

                //    ViewBag.DateFrom = DateFrom;
                //    ViewBag.DateTo = DateTo;

                //    ViewBag.IsDateFilter = "1";
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, DateFrom, DateTo).OrderBy(a => a.LoaderNumber).ToList();
                //}
                //else
                //{
                //    DateTime now = DateTime.Now;
                //    var startDate = new DateTime(now.Year, now.Month, 1);
                //    var endDate = startDate.AddMonths(1).AddDays(-1);

                //    ViewBag.DateFrom = startDate;
                //    ViewBag.DateTo = endDate;


                //    ViewBag.IsDateFilter = "0";
                //    ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, startDate, endDate).OrderBy(a => a.LoaderNumber).ToList();
                //}

                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;


                //return View();

                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = "A";
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";
                //ViewBag.LoadDetails = deEntity.Sp_Get_LoadPickupDelivery_Detail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();


                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;
                ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetails(LoadNumber).OrderBy(a => a.LoaderNumber).ToList();


                //return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Load Details" + ex.Message;
            }

            return View();

        }

        [HttpGet]
        [Customexception]
        public ActionResult GetLoadEmailHistory(string LoadNumber)
        {
            List<tblEmailHistory> EmailHistory = new List<tblEmailHistory>();
            try
            {

                EmailHistory = deEntity.tblEmailHistories.Where(a => a.LoadNumber == LoadNumber).ToList();

                return Json(EmailHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)

            {
                ViewBag.Error = "Exception occur while getting Load Email History " + ex.Message;
            }

            return Json(EmailHistory, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Customexception]

        public ActionResult GetAgentStats(int AgentId, string FilterType, string FilterDateFroms, string FilterDateTos)
        {
            DateTime FilterDateFrom = Convert.ToDateTime(FilterDateFroms);
            DateTime FilterDateTo = Convert.ToDateTime(FilterDateFroms);
            DataTable dt = new DataTable();
            string qry = "Exec Sp_Get_Agent_Statistics  '" + FilterType + "' ," + AgentId + " ";

            dt = ut.GetDatatable(qry);


            List<AgentStats> AgentStat = new List<AgentStats>();

            foreach (DataRow dr in dt.Rows)

            {

                AgentStat.Add(new AgentStats
                {
                    AgentName = dr["DispatcherName"].ToString(),
                    TotalQuantity = Convert.ToInt32(dr["TotalQuanity"]),
                    GrossProfit = Convert.ToInt32(dr["GrossProfit"]),
                });

            }

            return Json(AgentStat, JsonRequestBehavior.AllowGet);



        }
        [Customexception]
        public ActionResult ReportPdfDownload(string LoadNumberModel)
        {


            ViewBag.LoadNumber = LoadNumberModel;
            ViewBag.RequestURL = Request.Url.AbsoluteUri;
            if (Request.QueryString["LoadNumberModel"] != null)
            {
                string LoadQuerystring = Request.QueryString["LoadNumberModel"];
                LoadNumberModel = LoadQuerystring;
            }

            try
            {
                string Query = "";
                DataTable dt = new DataTable();
                ViewBag.LoadHead = deEntity.Sp_Get_LoadHeadInformation_Edit(LoadNumberModel).ToList();
                ViewBag.LoadInformation = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoadNumberModel).ToList();
                //ViewBag.LoadPickup = deEntity.tblLoadPickups.ToList().Where(d => d.LoadNumber == LoadNumberModel).ToList();
                //ViewBag.LoadDelivery = deEntity.tblLoadDeliveries.ToList().Where(d => d.LoadNumber == LoadNumberModel).ToList();
                ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoaderNumber == LoadNumberModel).ToList();


                //Pickup Information
                Query = "Exec Sp_Get_Load_Pickup_Delivery_Information '" + LoadNumberModel + "','P'";
                List<PickupDeliveryInformation> PI = new List<PickupDeliveryInformation>();
                dt = ut.GetDatatable(Query);
                foreach (DataRow dr in dt.Rows)

                {
                    PI.Add(new PickupDeliveryInformation
                    {
                        ShipperName = (dr["ShipperName"]).ToString(),
                        ShipperAddress = (dr["ShipperAddress"]).ToString(),
                        CityName = (dr["CityName"]).ToString(),
                        StateCode = (dr["StateCode"]).ToString(),
                        ZipCode = (dr["ZipCode"]).ToString(),
                        ShipperPhone = (dr["ShipperPhone"]).ToString(),
                        DateTime = (dr["DateTime"]).ToString(),
                        DateTimeTo = (dr["DateTimeTo"]).ToString(),
                        PickupNumber = (dr["PickupNumber"]).ToString(),
                        Comment = (dr["Comment"]).ToString(),
                        ordernumber = Convert.ToInt32((dr["ordernumber"])),
                        Name = (dr["name"]).ToString(),

                    });

                }
                ViewBag.LoadPickup = PI;


                //Delivery Information
                Query = "Exec Sp_Get_Load_Pickup_Delivery_Information '" + LoadNumberModel + "','D'";
                List<PickupDeliveryInformation> DI = new List<PickupDeliveryInformation>();
                dt = ut.GetDatatable(Query);
                foreach (DataRow dr in dt.Rows)

                {
                    DI.Add(new PickupDeliveryInformation
                    {
                        ShipperName = (dr["ShipperName"]).ToString(),
                        ShipperAddress = (dr["ShipperAddress"]).ToString(),
                        CityName = (dr["CityName"]).ToString(),
                        StateCode = (dr["StateCode"]).ToString(),
                        ZipCode = (dr["ZipCode"]).ToString(),
                        ShipperPhone = (dr["ShipperPhone"]).ToString(),
                        DateTime = (dr["DateTime"]).ToString(),
                        DateTimeTo = (dr["DateTimeTo"]).ToString(),
                        PickupNumber = (dr["PickupNumber"]).ToString(),
                        Comment = (dr["Comment"]).ToString(),
                        ordernumber = Convert.ToInt32((dr["ordernumber"])),
                        Name = (dr["name"]).ToString(),
                    });

                }
                ViewBag.LoadDelivery = DI;

                Query = "SELECT CarrierId FROM tblLoadHead Where LoaderNumber = '" + LoadNumberModel + "' ";
                string CarrierAssignId = ut.ExecuteScalar(Query);
                ViewBag.CarrierInformation = deEntity.tblCarriers.ToList().Where(d => d.AssignID == CarrierAssignId).ToList();


                string qry = "update tblLoadHead Set IsPrint =1 Where LoaderNumber ='" + LoadNumberModel + "' ";
                ut.InsertUpdate(qry);


                var agentid = (from loadheadinfo in deEntity.tblLoadHeads
                               where loadheadinfo.LoaderNumber == LoadNumberModel
                               select loadheadinfo.User_ID).SingleOrDefault();

                string agentname = "";
                string agentemail = "";
                string agentphone = "";
                var us = deEntity.tblProfiles.Where(u => u.User_ID == agentid).FirstOrDefault();
                if (us != null)
                {
                    agentname = us.Profile_name;
                    agentemail = us.Email;
                    agentphone = us.phoneNo;
                }
                ViewBag.agentname = agentname;
                ViewBag.agentemail = agentemail;
                ViewBag.agentphone = agentphone;


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While getting Load Information :" + ex.Message;
            }

            return View();


        }

        [Customexception]
        public ActionResult DriverLocations(string LoadNumber)
        {
            try
            {
                ViewBag.LoadNumber = LoadNumber;
                ViewBag.DriverLocation = deEntity.tblDriverLocations.Where(dl => dl.LoadNumber == LoadNumber).ToList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while getting driver location for load: " + ex.Message;
            }

            return View();
        }
        [Customexception]
        public byte[] ExportPdf(string LoadNumberModel)
        {

            //Method3
            var actionPDF = new Rotativa.ActionAsPdf("ReportPdfDownload", new { LoadNumberModel = LoadNumberModel }) //some route values)
            {
                FileName = "TestView.pdf",
                //PageSize = Size.A4,
                //PageOrientation = Rotativa.Options.Orientation.Landscape,
                //PageMargins = { Left = 1, Right = 1 }
            };
            byte[] applicationPDFData = actionPDF.BuildPdf(ControllerContext);

            return applicationPDFData;

            //Method2
            //var report = new Rotativa.ActionAsPdf("ReportPdfDownload", new { LoadNumberModel = LoadNumberModel });
            //return report;


            //Method1
            //return new ActionAsPdf("ReportPdfDownload", new { LoadNumberModel = LoadNumberModel })
            //{
            //    FileName = Server.MapPath("~/Content/ExportToPdf.pdf")
            //};

        }
        [Customexception]
        public ActionResult DownloadPdf(string LoadNumberModel)
        {
            //Method1
            return new ActionAsPdf("ReportPdfDownload", new { LoadNumberModel = LoadNumberModel })
            {
                FileName = "Load_" + LoadNumberModel + ".pdf"

                //FileName = Server.MapPath("~/Content/" + LoadNumberModel + ".pdf")
            };


        }


        [HttpGet]
        [Customexception]
        public ActionResult GetLoadLocations(string LoadNumber)
        {
            string qry = "";
            DataTable dt = new DataTable();
            qry = " Exec Sp_Get_Load_Locations  '" + LoadNumber + "' ";

            //qry = " select Latitude ,Longitude,'' as ShipperAddress , 'P' as Type FROM tblDriverLocation Where LoadNumber = '" + LoadNumber + "' ";



            dt = ut.GetDatatable(qry);


            //List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();
            List<LoadLocations> LoadLocation = new List<LoadLocations>();


            foreach (DataRow dr in dt.Rows)

            {

                LoadLocation.Add(new LoadLocations
                {

                    Longitude = (dr["Longitude"]).ToString(),
                    Latitude = (dr["Latitude"]).ToString(),
                    Address = (dr["ShipperAddress"]).ToString(),
                    Type = (dr["Type"]).ToString(),

                });

            }
            return Json(LoadLocation, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        [Customexception]
        public ActionResult GetLoadLocationsReatlTracking(string LoadNumber)
        {
            string qry = "";
            DataTable dt = new DataTable();
            //qry = " Exec Sp_Get_Load_Locations  '" + LoadNumber + "' ";

            qry = " select Latitude ,Longitude,'' as ShipperAddress , 'P' as Type FROM tblDriverLocation Where LoadNumber = '" + LoadNumber + "' ";



            dt = ut.GetDatatable(qry);


            //List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();
            List<LoadLocations> LoadLocation = new List<LoadLocations>();


            foreach (DataRow dr in dt.Rows)

            {

                LoadLocation.Add(new LoadLocations
                {

                    Longitude = (dr["Longitude"]).ToString(),
                    Latitude = (dr["Latitude"]).ToString(),
                    Address = (dr["ShipperAddress"]).ToString(),
                    Type = (dr["Type"]).ToString(),

                });

            }
            return Json(LoadLocation, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ValidateInput(false)]
        //public FileResult Export(string GridHtml)
        //{

        //    //using (MemoryStream stream = new System.IO.MemoryStream())
        //    //{
        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        StringReader sr = new StringReader(GridHtml);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc,stream);
        //        pdfDoc.Open();
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        pdfDoc.Close();



        //        //using (MailMessage mm = new MailMessage("info@consultdoc.co.uk", "hasanbilal369@gmail.com"))
        //        //    {
        //        //        //string link = Request.Url.ToString();
        //        //        //link = link.Replace("ForgetPassword", "ChangePassword");
        //        //        mm.Subject = "Prescription (Consultdoc Limited)";
        //        //        string body = "Hello ,";
        //        //        body += "<br/>";
        //        //        body += "<br/>";

        //        //        //body += "<p><strong >GMC:</strong>" + dt.Rows[0]["Category"].ToString() + " Specialist</p>";

        //        //        body += "<p>" + Session["Address"] + "</p>";

        //        //        //body += "<p>Consuldoc Limited , C/O Lighthall Consult, Boardman House, 64 The Broadway, </p>";
        //        //        //body += "<p>London, United Kingdom, E15 1NT</p>";



        //        //        //body += "<br /><br />Thanks for choosing Consuldoc Limited. You’ve just taken an exciting step in your wellness journey, and we’re so glad to be a part of it.";
        //        //        //body += "<br />Your appointment is booked for  <strong>" + dt.Rows[0]["StartTime"].ToString() + "</strong> on  <strong>  " + dt.Rows[0]["AppointmentDate"].ToString() + " </strong>";
        //        //        //body += ", With  Consultant  <strong>" + dt.Rows[0]["DoctorName"].ToString() + "</strong> ";

        //        //        //body += "<br /><br />Best Regards,  ";
        //        //        //body += "<br />" + dt.Rows[0]["DoctorName"].ToString() + "  ";
        //        //        mm.Body = body;
        //        //        mm.Attachments.Add(new Attachment(new MemoryStream(stream.ToArray()), "Invoice.pdf"));
        //        //        mm.IsBodyHtml = true;
        //        //        SmtpClient smtp = new SmtpClient();
        //        //        smtp.Host = "consultdoc.co.uk";
        //        //        smtp.EnableSsl = false;
        //        //        NetworkCredential NetworkCred = new NetworkCredential("info@consultdoc.co.uk", "Secret123!@#");



        //        //        smtp.UseDefaultCredentials = true;
        //        //        smtp.Credentials = NetworkCred;
        //        //        smtp.Port = 25;
        //        //        smtp.Send(mm);
        //        //    }
        //        return File(stream.ToArray(), "application/pdf", "Grid.pdf");
        //    }



        //    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //    //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //    //pdfDoc.Open();
        //    //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //    //pdfDoc.Close();



        //    //using (MailMessage mm = new MailMessage("info@consultdoc.co.uk", "hasanbilal369@gmail.com"))
        //    //{
        //    //    //string link = Request.Url.ToString();
        //    //    //link = link.Replace("ForgetPassword", "ChangePassword");
        //    //    mm.Subject = "Prescription (Consultdoc Limited)";
        //    //    string body = "Hello ,";
        //    //    body += "<br/>";
        //    //    body += "<br/>";

        //    //    //body += "<p><strong >GMC:</strong>" + dt.Rows[0]["Category"].ToString() + " Specialist</p>";

        //    //    body += "<p>" + Session["Address"] + "</p>";

        //    //    //body += "<p>Consuldoc Limited , C/O Lighthall Consult, Boardman House, 64 The Broadway, </p>";
        //    //    //body += "<p>London, United Kingdom, E15 1NT</p>";



        //    //    //body += "<br /><br />Thanks for choosing Consuldoc Limited. You’ve just taken an exciting step in your wellness journey, and we’re so glad to be a part of it.";
        //    //    //body += "<br />Your appointment is booked for  <strong>" + dt.Rows[0]["StartTime"].ToString() + "</strong> on  <strong>  " + dt.Rows[0]["AppointmentDate"].ToString() + " </strong>";
        //    //    //body += ", With  Consultant  <strong>" + dt.Rows[0]["DoctorName"].ToString() + "</strong> ";

        //    //    //body += "<br /><br />Best Regards,  ";
        //    //    //body += "<br />" + dt.Rows[0]["DoctorName"].ToString() + "  ";
        //    //    mm.Body = body;
        //    //    mm.Attachments.Add(new Attachment(new MemoryStream(stream.ToArray()), "Invoice.pdf"));
        //    //    mm.IsBodyHtml = true;
        //    //    SmtpClient smtp = new SmtpClient();
        //    //    smtp.Host = "consultdoc.co.uk";
        //    //    smtp.EnableSsl = false;
        //    //    NetworkCredential NetworkCred = new NetworkCredential("info@consultdoc.co.uk", "Secret123!@#");
        //    //    smtp.UseDefaultCredentials = true;
        //    //    smtp.Credentials = NetworkCred;
        //    //    smtp.Port = 25;
        //    //    smtp.Send(mm);
        //    //}









        //    //return File(memmor.ToArray(), "application/pdf", "Grid.pdf");
        //    //}
        //}


        [HttpPost]
        [Customexception]
        public ActionResult FilterLodList(string Filter)
        {
            DataTable dt = new DataTable();

            //LoadNumber = "2010001";
            ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
            //return RedirectToAction("Index");

            //List<LoadData> list = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
            return Json(new
            {
                list = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }

        [Customexception]
        public ActionResult ReportPreview(string LoadNumberModel)
        {
            ViewBag.LoadNumber = LoadNumberModel;
            ViewBag.RequestURL = Request.Url.AbsoluteUri;
            if (Request.QueryString["LoadNumberModel"] != null)
            {
                string LoadQuerystring = Request.QueryString["LoadNumberModel"];
                LoadNumberModel = LoadQuerystring;
            }

            try
            {
                string Query = "";

                Query = "Exec Sp_Get_Load_Carrier_Email '" + LoadNumberModel + "'";

                string CarrierEmail = ut.ExecuteScalar(Query);
                ViewBag.CarrierEmail = CarrierEmail;

                DataTable dt = new DataTable();
                ViewBag.LoadHead = deEntity.Sp_Get_LoadHeadInformation_Edit(LoadNumberModel).ToList();
                ViewBag.LoadInformation = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoadNumberModel).ToList();



                var agentid = (from loadheadinfo in deEntity.tblLoadHeads
                               where loadheadinfo.LoaderNumber == LoadNumberModel
                               select loadheadinfo.User_ID).SingleOrDefault();

                string agentname = "";
                string agentemail = "";
                string agentphone = "";
                var us = deEntity.tblProfiles.Where(u => u.User_ID == agentid).FirstOrDefault();
                if (us != null)
                {
                    agentname = us.Profile_name;
                    agentemail = us.Email;
                    agentphone = us.phoneNo;
                }
                ViewBag.agentname = agentname;
                ViewBag.agentemail = agentemail;
                ViewBag.agentphone = agentphone;

                //var agentname = deEntity.tblUsers.Where(u=> u.User_ID == Agentid.).fir


                //ViewBag.LoadPickup = deEntity.tblLoadPickups.ToList().Where(d => d.LoadNumber == LoadNumberModel).ToList();
                //ViewBag.LoadDelivery = deEntity.tblLoadDeliveries.ToList().Where(d => d.LoadNumber == LoadNumberModel).ToList();
                ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoaderNumber == LoadNumberModel).ToList();


                //Pickup Information
                Query = "Exec Sp_Get_Load_Pickup_Delivery_Information '" + LoadNumberModel + "','P'";
                List<PickupDeliveryInformation> PI = new List<PickupDeliveryInformation>();
                dt = ut.GetDatatable(Query);
                foreach (DataRow dr in dt.Rows)

                {
                    PI.Add(new PickupDeliveryInformation
                    {
                        ShipperName = (dr["ShipperName"]).ToString(),
                        ShipperAddress = (dr["ShipperAddress"]).ToString(),
                        CityName = (dr["CityName"]).ToString(),
                        StateCode = (dr["StateCode"]).ToString(),
                        ZipCode = (dr["ZipCode"]).ToString(),
                        ShipperPhone = (dr["ShipperPhone"]).ToString(),
                        DateTime = (dr["DateTime"]).ToString(),
                        DateTimeTo = (dr["DateTimeTo"]).ToString(),
                        PickupNumber = (dr["PickupNumber"]).ToString(),
                        Comment = (dr["Comment"]).ToString(),
                        ordernumber = Convert.ToInt32((dr["ordernumber"])),
                        Name = (dr["name"]).ToString(),
                    });

                }
                ViewBag.LoadPickup = PI;


                //Delivery Information
                Query = "Exec Sp_Get_Load_Pickup_Delivery_Information '" + LoadNumberModel + "','D'";
                List<PickupDeliveryInformation> DI = new List<PickupDeliveryInformation>();
                dt = ut.GetDatatable(Query);
                foreach (DataRow dr in dt.Rows)

                {
                    DI.Add(new PickupDeliveryInformation
                    {
                        ShipperName = (dr["ShipperName"]).ToString(),
                        ShipperAddress = (dr["ShipperAddress"]).ToString(),
                        CityName = (dr["CityName"]).ToString(),
                        StateCode = (dr["StateCode"]).ToString(),
                        ZipCode = (dr["ZipCode"]).ToString(),
                        ShipperPhone = (dr["ShipperPhone"]).ToString(),
                        DateTime = (dr["DateTime"]).ToString(),
                        DateTimeTo = (dr["DateTimeTo"]).ToString(),
                        PickupNumber = (dr["PickupNumber"]).ToString(),
                        Comment = (dr["Comment"]).ToString(),
                        ordernumber = Convert.ToInt32((dr["ordernumber"])),
                        Name = (dr["name"]).ToString(),

                    });

                }
                ViewBag.LoadDelivery = DI;

                Query = "SELECT CarrierId FROM tblLoadHead Where LoaderNumber = '" + LoadNumberModel + "' ";
                string CarrierAssignId = ut.ExecuteScalar(Query);
                ViewBag.CarrierInformation = deEntity.tblCarriers.ToList().Where(d => d.AssignID == CarrierAssignId).ToList();


                string qry = "update tblLoadHead Set IsPrint =1 Where LoaderNumber ='" + LoadNumberModel + "' ";
                ut.InsertUpdate(qry);


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While getting Load Information :" + ex.Message;
            }

            return View();
        }

        [HttpPost]
        [Customexception]
        public string SavePdf(string File)
        {
            return "";
        }

        [Customexception]
        public ActionResult GeneratePDF()
        {
            return new Rotativa.ActionAsPdf("ReportPreview");
        }
        //public ActionResult SendEmail(FormCollection collection)
        //{

        //    // read parameters from the webpage
        //    string url = Request.Url.ToString();

        //    url = url.Replace("SendEmail","ReportPreview");

        //    url = url + "?LoadNumberModel=2011001";
        //    // read parameters from the webpage
        //    //url = collection["LoadNumberModel"];

        //    string pdf_page_size = "1";
        //    HiQPdf.PdfPageSize pageSize = (HiQPdf.PdfPageSize)Enum.Parse(typeof(HiQPdf.PdfPageSize), pdf_page_size, true);

        //    //string pdf_orientation = ;


        //    PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(
        //        typeof(PdfPageOrientation), PdfPageOrientation.Portrait.ToString(), true);


        //    int webPageWidth = 2058;
        //    try
        //    {
        //        webPageWidth =2058;
        //    }
        //    catch { }

        //    int webPageHeight = 3058;
        //    try
        //    {
        //        webPageHeight = 3058;
        //    }
        //    catch { 

        //    }

        //    // instantiate a html to pdf converter object
        //    HtmlToPdf converter = new HtmlToPdf();

        //    // set converter options
        //    converter.Options.PdfPageSize = pageSize;
        //    converter.Options.PdfPageOrientation = pdfOrientation;
        //    converter.Options.WebPageWidth = webPageWidth;
        //    converter.Options.WebPageHeight = webPageHeight;

        //    // create a new pdf document converting an url
        //    //PdfDocument doc = converter.ConvertUrl(url);

        //    //// save pdf document
        //    //byte[] pdf = doc.Save();

        //    //// close pdf document
        //    //doc.Close();

        //    //// return resulted pdf document
        //    //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
        //    //fileResult.FileDownloadName = "Document.pdf";
        //    //return fileResult;
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}
        [Customexception]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string LoaderNumbers = "2010002";
                        string FolderPath = "/Uploads/Loads/LoaderNumbers/" + LoaderNumbers + "/";
                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        //Code to generate pdf and send email 
        [HttpPost]
        [ValidateInput(false)]
        [Customexception]
        public ActionResult ConvertToPdf(string LoadNumber, string Subject, string Email, string Detail, int userid)
        {

            try
            {
                DataTable dt = new DataTable();


                string ccEmailQuery = "select Email_Adress from tblUser where User_ID =" + userid + "";
                string ccemail = ut.ExecuteScalar(ccEmailQuery);

                string EmailQuery = "select * from tblEmailSetting";
                dt = ut.GetDatatable(EmailQuery);
                //m_formCollection = collection;

                // create the HTML to PDF converter
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

                // set a demo serial number
                htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

                // set browser width
                htmlToPdfConverter.BrowserWidth = int.Parse("1200");

                htmlToPdfConverter.HiddenHtmlElements = new string[]
                { "#btnsendemail" ,"#cmd"};


                // set browser height if specified, otherwise use the default
                //if (collection["textBoxBrowserHeight"].Length > 0)
                //    htmlToPdfConverter.BrowserHeight = int.Parse(collection["textBoxBrowserHeight"]);

                // set HTML Load timeout
                htmlToPdfConverter.HtmlLoadedTimeout = int.Parse("120");

                // set PDF page size and orientation
                htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;

                // set the PDF standard used by the document
                htmlToPdfConverter.Document.PdfStandard = PdfStandard.Pdf;

                // set PDF page margins
                htmlToPdfConverter.Document.Margins = new PdfMargins(10);

                // set whether to embed the true type font in PDF
                htmlToPdfConverter.Document.FontEmbedding = true;

                // set triggering mode; for WaitTime mode set the wait time before convert
                //switch (collection["dropDownListTriggeringMode"])
                //{
                //    case "Auto":
                //        htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;
                //        break;
                //    case "WaitTime":
                //        htmlToPdfConverter.TriggerMode = ConversionTriggerMode.WaitTime;
                //        htmlToPdfConverter.WaitBeforeConvert = int.Parse(collection["textBoxWaitTime"]);
                //        break;
                //    case "Manual":
                //        htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Manual;
                //        break;
                //    default:
                //        htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;
                //        break;
                //}

                htmlToPdfConverter.TriggerMode = ConversionTriggerMode.WaitTime;

                // set header and footer
                SetHeader(htmlToPdfConverter.Document);
                SetFooter(htmlToPdfConverter.Document);

                // set the document security
                htmlToPdfConverter.Document.Security.OpenPassword = "";
                htmlToPdfConverter.Document.Security.AllowPrinting = true;

                // set the permissions password too if an open password was set
                if (htmlToPdfConverter.Document.Security.OpenPassword != null && htmlToPdfConverter.Document.Security.OpenPassword != String.Empty)
                    htmlToPdfConverter.Document.Security.PermissionsPassword = htmlToPdfConverter.Document.Security.OpenPassword + "_admin";

                // convert HTML to PDF
                byte[] pdfBuffer = null;

                //if (collection["UrlOrHtmlCode"] == "radioButtonConvertUrl")
                //{
                // convert URL to a PDF memory buffer
                //string url = collection["textBoxUrl"];
                //string url = Request.Url.AbsoluteUri;
                //url = url + "?\\/?LoadNumberModel=" + LoadNumber + "";

                var baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                baseUrl = baseUrl + "Report/ReportPreview/" + "?LoadNumberModel=" + LoadNumber + "";
                pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(baseUrl);
                //}
                //else
                //{
                //    // convert HTML code
                //    string htmlCode = collection["textBoxHtmlCode"];
                //    string baseUrl = collection["textBoxBaseUrl"];

                //    // convert HTML code to a PDF memory buffer
                //    pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlCode, baseUrl);
                //}

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                //if (collection["checkBoxOpenInline"] == null)

                fileResult.FileDownloadName = LoadNumber + ".pdf";



                //using (MailMessage mm = new MailMessage("Jack@AlkaiosTransportation.com", Email))


                string[] AllEmail = Email.Split(',');

                //Iterate through each of the letters
                foreach (var letter in AllEmail)
                {
                    using (MailMessage mm = new MailMessage(dt.Rows[0]["Email"].ToString(), letter))
                    {
                        //string link = Request.Url.ToString();
                        //link = link.Replace("ForgetPassword", "ChangePassword");
                        mm.Subject = Subject;

                        mm.CC.Add(new MailAddress(ccemail));
                        mm.Headers.Add("In-Reply-To", ccemail);
                        //mm.CC.Add(new MailAddress(dt.Rows[0]["CCEmail"].ToString()));
                        //mm.Headers.Add("In-Reply-To", dt.Rows[0]["CCEmail"].ToString());

                        //ReplyTo "reply@adminsystem.com";
                        //mm.CC.Add(new MailAddress("Jack@alkaiostransportation.com"));
                        string body = "Load Number: " + LoadNumber + "";
                        body += "<br/>";
                        body += "<br/>";

                        //body += "<p><strong >GMC:</strong>" + dt.Rows[0]["Category"].ToString() + " Specialist</p>";

                        body += "<p>" + Detail + "</p>";

                        //body += "<p>Consuldoc Limited , C/O Lighthall Consult, Boardman House, 64 The Broadway, </p>";
                        //body += "<p>London, United Kingdom, E15 1NT</p>";



                        //body += "<br /><br />Thanks for choosing Consuldoc Limited. You’ve just taken an exciting step in your wellness journey, and we’re so glad to be a part of it.";
                        //body += "<br />Your appointment is booked for  <strong>" + dt.Rows[0]["StartTime"].ToString() + "</strong> on  <strong>  " + dt.Rows[0]["AppointmentDate"].ToString() + " </strong>";
                        //body += ", With  Consultant  <strong>" + dt.Rows[0]["DoctorName"].ToString() + "</strong> ";

                        //body += "<br /><br />Best Regards,  ";
                        //body += "<br />" + dt.Rows[0]["DoctorName"].ToString() + "  ";
                        mm.Body = body;
                        //mm.Attachments.Add(new Attachment(new MemoryStream(pdfBuffer), LoadNumber + ".pdf"));

                        mm.BodyEncoding = System.Text.Encoding.UTF8;
                        mm.SubjectEncoding = System.Text.Encoding.Default;

                        mm.Attachments.Add(new Attachment(new MemoryStream(ExportPdf(LoadNumber)), LoadNumber + ".pdf"));

                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = dt.Rows[0]["Host"].ToString();
                        smtp.EnableSsl = Convert.ToBoolean(dt.Rows[0]["SSLEnable"]);


                        //networkcredential networkcred = new networkcredential("jack@alkaiostransportation.com", "mamba851018@");

                        //NetworkCredential NetworkCred = new NetworkCredential("restock06@gmail.com", "Developer@123");

                        //NetworkCredential NetworkCred = new NetworkCredential("Jack@AlkaiosTransportation.com", "Mamba851018@");

                        NetworkCredential NetworkCred = new NetworkCredential(dt.Rows[0]["Email"].ToString(), dt.Rows[0]["Password"].ToString());
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = Convert.ToInt32(dt.Rows[0]["Port"]);
                        smtp.Send(mm);
                    }
                    string query = "Exec Sp_Insert_Update_EmailHistory 0,'" + LoadNumber + "' ,'" + letter + "','" + Subject + "' ,1,'Successfully Send','" + Detail + "' ";
                    ut.InsertUpdate(query);
                }





                //return fileResult;
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Sending Email:" + ex.Message;
                string query = "Exec Sp_Insert_Update_EmailHistory 0,'" + LoadNumber + "' ,'" + Email + "','" + Subject + "' ,0,'" + ex.Message + "','" + Detail + "'";
                ut.InsertUpdate(query);
                return Json("0", JsonRequestBehavior.AllowGet);

            }


            //return fileResult;
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [Customexception]
        private void SetHeader(PdfDocumentControl htmlToPdfDocument)
        {
            // enable header display
            htmlToPdfDocument.Header.Enabled = false;

            if (!htmlToPdfDocument.Header.Enabled)
                return;

            // set header height
            htmlToPdfDocument.Header.Height = 50;

            float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float headerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
            float headerHeight = htmlToPdfDocument.Header.Height;

            // set header background color
            htmlToPdfDocument.Header.BackgroundColor = System.Drawing.Color.WhiteSmoke;

            string headerImageFile = Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo.png";
            PdfImage logoHeaderImage = new PdfImage(5, 5, 40, System.Drawing.Image.FromFile(headerImageFile));
            htmlToPdfDocument.Header.Layout(logoHeaderImage);

            // layout HTML in header
            PdfHtml headerHtml = new PdfHtml(50, 5, @"<span style=""color:Navy; font-family:Times New Roman; font-style:italic"">
                            Quickly Create High Quality PDFs with </span><a href=""http://www.hiqpdf.com"">HiQPdf</a>", null);
            headerHtml.FitDestHeight = true;
            headerHtml.FontEmbedding = true;
            htmlToPdfDocument.Header.Layout(headerHtml);

            // create a border for header

            PdfRectangle borderRectangle = new PdfRectangle(1, 1, headerWidth - 2, headerHeight - 2);
            borderRectangle.LineStyle.LineWidth = 0.5f;
            borderRectangle.ForeColor = System.Drawing.Color.Navy;
            htmlToPdfDocument.Header.Layout(borderRectangle);
        }
        [Customexception]
        private void SetFooter(PdfDocumentControl htmlToPdfDocument)
        {
            // enable footer display
            htmlToPdfDocument.Footer.Enabled = false;

            if (!htmlToPdfDocument.Footer.Enabled)
                return;

            // set footer height
            htmlToPdfDocument.Footer.Height = 50;

            // set footer background color
            htmlToPdfDocument.Footer.BackgroundColor = System.Drawing.Color.WhiteSmoke;

            float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float footerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
            float footerHeight = htmlToPdfDocument.Footer.Height;

            // layout HTML in footer
            PdfHtml footerHtml = new PdfHtml(5, 5, @"<span style=""color:Navy; font-family:Times New Roman; font-style:italic"">
                            Quickly Create High Quality PDFs with </span><a href=""http://www.hiqpdf.com"">HiQPdf</a>", null);
            footerHtml.FitDestHeight = true;
            footerHtml.FontEmbedding = true;
            htmlToPdfDocument.Footer.Layout(footerHtml);

            // add page numbering
            System.Drawing.Font pageNumberingFont = new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"),
                                        8, System.Drawing.GraphicsUnit.Point);
            PdfText pageNumberingText = new PdfText(5, footerHeight - 12, "Page {CrtPage} of {PageCount}", pageNumberingFont);
            pageNumberingText.HorizontalAlign = PdfTextHAlign.Center;
            pageNumberingText.EmbedSystemFont = true;
            pageNumberingText.ForeColor = System.Drawing.Color.DarkGreen;
            htmlToPdfDocument.Footer.Layout(pageNumberingText);

            string footerImageFile = Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo.png";
            PdfImage logoFooterImage = new PdfImage(footerWidth - 40 - 5, 5, 40, System.Drawing.Image.FromFile(footerImageFile));
            htmlToPdfDocument.Footer.Layout(logoFooterImage);

            // create a border for footer
            PdfRectangle borderRectangle = new PdfRectangle(1, 1, footerWidth - 2, footerHeight - 2);
            borderRectangle.LineStyle.LineWidth = 0.5f;
            borderRectangle.ForeColor = System.Drawing.Color.DarkGreen;
            htmlToPdfDocument.Footer.Layout(borderRectangle);
        }
        [Customexception]
        private PdfPageSize GetSelectedPageSize()
        {
            switch (m_formCollection["dropDownListPageSizes"])
            {
                case "A0":
                    return PdfPageSize.A0;
                case "A1":
                    return PdfPageSize.A1;
                case "A10":
                    return PdfPageSize.A10;
                case "A2":
                    return PdfPageSize.A2;
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "A6":
                    return PdfPageSize.A6;
                case "A7":
                    return PdfPageSize.A7;
                case "A8":
                    return PdfPageSize.A8;
                case "A9":
                    return PdfPageSize.A9;
                case "ArchA":
                    return PdfPageSize.ArchA;
                case "ArchB":
                    return PdfPageSize.ArchB;
                case "ArchC":
                    return PdfPageSize.ArchC;
                case "ArchD":
                    return PdfPageSize.ArchD;
                case "ArchE":
                    return PdfPageSize.ArchE;
                case "B0":
                    return PdfPageSize.B0;
                case "B1":
                    return PdfPageSize.B1;
                case "B2":
                    return PdfPageSize.B2;
                case "B3":
                    return PdfPageSize.B3;
                case "B4":
                    return PdfPageSize.B4;
                case "B5":
                    return PdfPageSize.B5;
                case "Flsa":
                    return PdfPageSize.Flsa;
                case "HalfLetter":
                    return PdfPageSize.HalfLetter;
                case "Ledger":
                    return PdfPageSize.Ledger;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Letter11x17":
                    return PdfPageSize.Letter11x17;
                case "Note":
                    return PdfPageSize.Note;
                default:
                    return PdfPageSize.A4;
            }
        }
        [Customexception]
        private PdfPageOrientation GetSelectedPageOrientation()
        {
            return (m_formCollection["dropDownListPageOrientations"] == "Portrait") ?
                PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }


        //return RedirectToAction("Index");

        [Customexception]
        public ActionResult InvoicingDetail(int? userid, DateTime AdvanceFilterDateFroms, DateTime AdvanceFilterDateTo, int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, string cwhere, int? Custom, int? statemaintain = 0)
        {
            try
            {
                //if(statemaintain==1)
                //{
                //    Session["JqueryTableSearchValue"] = "";
                //    Session["LoadNumberscroll"] = "";
                //}

                //if(Session["JqueryTableSearchValue"] ==null || Session["JqueryTableSearchValue"].ToString()== "")
                //{
                //    ViewBag.JqueryTableSearchValue = "";
                //}
                //else
                //{
                //    ViewBag.JqueryTableSearchValue = Session["JqueryTableSearchValue"].ToString();

                //}

                //if (Session["LoadNumberscroll"] == null || Session["LoadNumberscroll"].ToString() == "")
                //{
                //    ViewBag.LoadNumberscroll = "";
                //}
                //else
                //{
                //    ViewBag.LoadNumberscroll = Session["LoadNumberscroll"].ToString();

                //}

                if (TempData["JqueryTableSearchValue"] == null || TempData["JqueryTableSearchValue"].ToString() == "")
                {
                    ViewBag.JqueryTableSearchValue = "";
                }
                else
                {
                    ViewBag.JqueryTableSearchValue = TempData["JqueryTableSearchValue"].ToString();

                }

                if (TempData["LoadNumberscroll"] == null || TempData["LoadNumberscroll"].ToString() == "")
                {
                    ViewBag.LoadNumberscroll = "";
                }
                else
                {
                    ViewBag.LoadNumberscroll = TempData["LoadNumberscroll"].ToString();

                }

                if (TempData["Rownumber"] == null || TempData["Rownumber"].ToString() == "")
                {
                    ViewBag.Rownumber = "";
                }
                else
                {
                    ViewBag.Rownumber = TempData["Rownumber"].ToString();

                }



                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                int AccessLevel = 0;
                int LoginUserId = Convert.ToInt32(Session["User_id"]);
                if (userid == null)
                {
                    userid = Convert.ToInt32(Session["User_id"]);
                }
                ViewBag.UserId = userid;
                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                if (Filter == null)
                {
                    Filter = "A";
                }


                if (cwhere == null)
                {
                    cwhere = "";
                }
                ViewBag.cwhere = cwhere;

                DateTime Daily;


                var Userinfo = deEntity.tblProfiles.Where(p => p.User_ID == LoginUserId).FirstOrDefault();

                if (Userinfo != null)
                {
                    AccessLevel = Convert.ToInt32(Userinfo.Accessid);
                }

                ViewBag.AccessLevel = AccessLevel;


                //If Form is already loaded and applied custom search 

                if (Custom == 1)
                {

                    string FullMonthName = FilterDateFroms.ToString("MMMM");
                    int year = FilterDateFroms.Year;

                    //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                    //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                    ViewBag.MonthDetails = FullMonthName + "  " + year;

                    ViewBag.LoadType = Type;
                    ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    ViewBag.LoadFilterDateTo = FilterDateTos;
                    ViewBag.IsDateFilter = "1";
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (cwhere != null && cwhere != "")
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", AdvanceFilterDateFroms, AdvanceFilterDateTo, cwhere, userid).OrderBy(a => a.LoaderNumber).ToList();
                    }
                    else
                    {
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, AdvanceFilterDateFroms, AdvanceFilterDateTo, cwhere, userid).OrderBy(a => a.LoaderNumber).ToList();
                    }
                }

                if (previous == null && Next == null && Custom == null)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;
                    if (Type == null)
                    {


                        ViewBag.LoadType = "Monthly";
                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Now;
                        string FullMonthName = Daily.ToString("MMMM");
                        int year = Daily.Year;
                        ViewBag.MonthDetails = FullMonthName + "  " + year;
                        FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        ViewBag.IsDateFilter = "1";


                        //ViewBag.LoadType = "Daily";

                        //Daily = DateTime.Now;
                        //ViewBag.LoadFilterDateFrom = Daily;
                        //ViewBag.LoadFilterDateTo = Daily;

                        ////DateTime now = DateTime.Now;
                        ////var startDate = new DateTime(now.Year, now.Month, 1);
                        ////var endDate = startDate.AddMonths(1).AddDays(-1);

                        ////ViewBag.DateFrom = startDate;
                        ////ViewBag.DateTo = endDate;


                        //ViewBag.IsDateFilter = "1";
                        ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                    }
                    else if (Type == "Daily")
                    {

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Weekly";

                            DateTime baseDate = DateTime.Today;
                            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //DateTime baseDate = DateTime.Today;
                            //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                            //FilterDateFroms = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                            //FilterDateTos = thisWeekStart.AddDays(7).AddSeconds(-1);
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }





                    }

                    else if (Type == "Weekly")
                    {

                        //ViewBag.LoadType = "Monthly";

                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Monthly";
                            FilterDateFroms = FilterDateFroms.AddDays(-30);

                            Daily = DateTime.Now;
                            string FullMonthName = Daily.ToString("MMMM");
                            int year = Daily.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);

                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            //FilterDateFroms = FilterDateFroms.AddDays(-30);

                            //Daily = DateTime.Now;
                            //string FullMonthName = Daily.ToString("MMMM");
                            //int year = Daily.Year;
                            //ViewBag.MonthDetails = FullMonthName + "  " + year;
                            //FilterDateFroms = Daily.AddDays(1 - Daily.Day);
                            //FilterDateTos = FilterDateFroms.AddMonths(1).AddSeconds(-1);


                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }



                    }
                    else if (Type == "Monthly")
                    {
                        //ViewBag.LoadType = "Daily";



                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Daily";
                            Daily = DateTime.Now;
                            ViewBag.LoadFilterDateFrom = Daily;
                            ViewBag.LoadFilterDateTo = Daily;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;
                            FilterDateFroms = Daily;

                            FilterDateTos = Daily;

                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;
                            Daily = DateTime.Now;
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;

                            //DateTime now = DateTime.Now;
                            //var startDate = new DateTime(now.Year, now.Month, 1);
                            //var endDate = startDate.AddMonths(1).AddDays(-1);

                            //ViewBag.DateFrom = startDate;
                            //ViewBag.DateTo = endDate;


                            string FullMonthName = FilterDateFroms.ToString("MMMM");
                            int year = FilterDateFroms.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            ViewBag.IsDateFilter = "1";
                            //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                            if (cwhere != null && cwhere != "")
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                            else
                            {
                                ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                            }
                        }




                    }



                }



                if (previous == 1)
                {

                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;
                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;



                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);




                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(-1);
                        FilterDateTos = FilterDateTos.AddDays(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(-7);
                        FilterDateTos = FilterDateTos.AddDays(-7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }
                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Now;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);


                        FilterDateFroms = FilterDateFroms.AddMonths(-1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    ////Daily = FilterDateFroms;
                    //FilterDateFroms = FilterDateFroms.AddDays(-1);
                    //FilterDateTos = FilterDateTos.AddDays(-1);


                    //ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    //ViewBag.LoadFilterDateTo = FilterDateTos;

                    ////DateTime now = DateTime.Now;
                    ////var startDate = new DateTime(now.Year, now.Month, 1);
                    ////var endDate = startDate.AddMonths(1).AddDays(-1);

                    ////ViewBag.DateFrom = startDate;
                    ////ViewBag.DateTo = endDate;


                    //ViewBag.IsDateFilter = "1";
                    ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                    //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos,userid).OrderBy(a => a.LoaderNumber).ToList();

                }

                if (Next == 1)
                {
                    ViewBag.AdvanceFilterDateFroms = AdvanceFilterDateFroms;
                    ViewBag.AdvanceFilterDateTo = AdvanceFilterDateTo;

                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;

                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);

                        //FilterDateFroms = FilterDateFroms.AddDays(-1);
                        //FilterDateTos = FilterDateTos.AddDays(-1);
                        FilterDateFroms = FilterDateFroms.AddDays(1);
                        FilterDateTos = FilterDateTos.AddDays(1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }
                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        //var today = baseDate;
                        //var yesterday = baseDate.AddDays(-1);
                        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        //var lastMonthStart = thisMonthStart.AddMonths(-1);
                        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);


                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);
                        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        //var lastWeekStart = thisWeekStart.AddDays(-7);
                        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);



                        FilterDateFroms = FilterDateFroms.AddDays(7);
                        FilterDateTos = FilterDateTos.AddDays(7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }


                    else if (Type == "Monthly")
                    {


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        ViewBag.LoadType = Type;

                        //FilterDateFroms = FilterDateFroms.AddDays(-30);

                        Daily = DateTime.Now;


                        //DateTime curDate = DateTime.Now;
                        //DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
                        //DateTime endDate = startDate.AddMonths(1).AddDays(-1);




                        FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);


                        //FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays();
                        //FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;

                        //var thisMonthStart = Daily.AddDays(1 - Daily.Day);
                        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;
                        //DateTime now = DateTime.Now;
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);

                        //ViewBag.DateFrom = startDate;
                        //ViewBag.DateTo = endDate;


                        ViewBag.IsDateFilter = "1";
                        //ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                        if (cwhere != null && cwhere != "")
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List("S", FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }
                        else
                        {
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Invoice_Load_List(Filter, FilterDateFroms, FilterDateTos, cwhere, userid).ToList();
                        }

                    }

                    //Daily = FilterDateFroms;
                    //FilterDateFroms = FilterDateFroms.AddDays(1);
                    //FilterDateTos = FilterDateTos.AddDays(1);


                    //ViewBag.LoadFilterDateFrom = FilterDateFroms;
                    //ViewBag.LoadFilterDateTo = FilterDateFroms;

                    ////DateTime now = DateTime.Now;
                    ////var startDate = new DateTime(now.Year, now.Month, 1);
                    ////var endDate = startDate.AddMonths(1).AddDays(-1);

                    ////ViewBag.DateFrom = startDate;
                    ////ViewBag.DateTo = endDate;


                    //ViewBag.IsDateFilter = "1";
                    ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                    //ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_WithAllFilter(Filter, FilterDateFroms, FilterDateTos,userid).OrderBy(a => a.LoaderNumber).ToList();

                }








                //if (FilterDateFroms !=null)
                //{
                //    Daily = DateTime.Now;

                //    ViewBag.Daily = Daily.ToString("MM/dd/yyyy");
                //    Daily = Convert.ToDateTime(FilterDateFroms);
                //    Daily = Daily.AddDays(-1);

                //    ViewBag.Daily = Daily;
                //    var date = Request.Form["FilterDateFroms"];
                //}



                //if (Next == 1)
                //{
                //    Daily = Convert.ToDateTime(FilterDateFroms);
                //    Daily = Daily.AddDays(1);
                //    ViewBag.Daily = Daily;
                //}


                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = Filter;
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";

                //if (Filter == "2")
                //{
                //    DateTime DateFrom = Convert.ToDateTime(Request.Form["FilterDateFrom"]);
                //    DateTime DateTo = Convert.ToDateTime(Request.Form["FilterDateTo"]);

                //    ViewBag.DateFrom = DateFrom;
                //    ViewBag.DateTo = DateTo;

                //    ViewBag.IsDateFilter = "1";
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, DateFrom, DateTo).OrderBy(a => a.LoaderNumber).ToList();
                //}
                //else
                //{
                //    DateTime now = DateTime.Now;
                //    var startDate = new DateTime(now.Year, now.Month, 1);
                //    var endDate = startDate.AddMonths(1).AddDays(-1);

                //    ViewBag.DateFrom = startDate;
                //    ViewBag.DateTo = endDate;


                //    ViewBag.IsDateFilter = "0";
                //    ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, startDate, endDate).OrderBy(a => a.LoaderNumber).ToList();
                //}

                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;


                return View();




                //Old Pattern
                //DateTime Daily = DateTime.Now;

                //ViewBag.Daily = Daily.ToString("MM/dd/yyyy");
                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = Filter;
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";

                //if (Filter == "2")
                //{
                //    DateTime DateFrom = Convert.ToDateTime(Request.Form["FilterDateFrom"]);
                //    DateTime DateTo = Convert.ToDateTime(Request.Form["FilterDateTo"]);

                //    ViewBag.DateFrom = DateFrom;
                //    ViewBag.DateTo = DateTo;

                //    ViewBag.IsDateFilter = "1";
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, DateFrom, DateTo).OrderBy(a => a.LoaderNumber).ToList();
                //}
                //else
                //{
                //    DateTime now = DateTime.Now;
                //    var startDate = new DateTime(now.Year, now.Month, 1);
                //    var endDate = startDate.AddMonths(1).AddDays(-1);

                //    ViewBag.DateFrom = startDate;
                //    ViewBag.DateTo = endDate;


                //    ViewBag.IsDateFilter = "0";
                //    ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();
                //    ViewBag.LoadDetailsMonth = deEntity.Sp_Get_LoadDetail_FilterMonthWise(Filter, startDate, endDate).OrderBy(a => a.LoaderNumber).ToList();
                //}

                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;


                //return View();

                //if (Filter == null)
                //{
                //    //Filter = "0";
                //    Filter = "A";
                //}

                //if (Type == null)
                //{
                //    //Type = "0";
                //    Type = "M";
                //}

                ////LoadNumber = "2010001";
                //ViewBag.LoadDetails = deEntity.Sp_Get_LoadPickupDelivery_Detail_FilterWise(Filter).OrderBy(a => a.LoaderNumber).ToList();


                //ViewBag.EmailHistory = deEntity.tblEmailHistories.ToList();
                //ViewBag.Type = Type;
                ////ViewBag.LoadDetails = deEntity.Sp_Get_LoadDetails(LoadNumber).OrderBy(a => a.LoaderNumber).ToList();


                //return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Invoicing Detail" + ex.Message;

            }
            return View();

        }

        [HttpPost]
        [Customexception]
        public ActionResult UpdateInvoice(string LoadNumber, int LoadStatus, string LoadType, string JqueryTableSearchValue, int rowindex = 0)
        {
            string Query = "";
            try
            {

                TempData["JqueryTableSearchValue"] = JqueryTableSearchValue;
                TempData["LoadNumberscroll"] = LoadNumber;
                TempData["Rownumber"] = rowindex;

                //Session["JqueryTableSearchValue"] = JqueryTableSearchValue;
                //Session["LoadNumberscroll"] = LoadNumber;
                ViewBag.JqueryTableSearchValue = JqueryTableSearchValue;


                if (LoadType == "IC")
                {
                    Query = "update tblloadhead set IsInvoiceCustomer = " + LoadStatus + "  Where LoaderNumber = '" + LoadNumber + "'";
                }
                else if (LoadType == "PR")
                {
                    Query = "update tblloadhead set IsPaymentRecieved = " + LoadStatus + "  Where LoaderNumber = '" + LoadNumber + "'";
                }
                else if (LoadType == "PC")
                {
                    Query = "update tblloadhead set IsPaidCarrier = " + LoadStatus + "  Where LoaderNumber = '" + LoadNumber + "'";
                }
                else if (LoadType == "FA")
                {
                    Query = "update tblloadhead set IsFuelAdvance = " + LoadStatus + "  Where LoaderNumber = '" + LoadNumber + "'";
                }
                ut.InsertUpdate(Query);



                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [Customexception]
        public ActionResult UpdateQuickPay(string LoadNumber, int LoadStatus, string LoadType, double QuickPaypercentage, string JqueryTableSearchValue, int rowindex = 0)
        {

            TempData["JqueryTableSearchValue"] = JqueryTableSearchValue;
            TempData["LoadNumberscroll"] = LoadNumber;
            TempData["Rownumber"] = rowindex;

            //Session["JqueryTableSearchValue"] = JqueryTableSearchValue;
            //Session["LoadNumberscroll"] = LoadNumber;
            ViewBag.JqueryTableSearchValue = JqueryTableSearchValue;


            string Query = "";
            try
            {

                Query = "update tblloadhead set  QuickPaypercentage=" + QuickPaypercentage + "  Where LoaderNumber = '" + LoadNumber + "'";
                ut.InsertUpdate(Query);
                Query = "Exec Sp_Calculate_Quick_Pay  " + LoadStatus + " , '" + LoadNumber + "'," + QuickPaypercentage + "";

                ut.InsertUpdate(Query);

                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [Customexception]
        public ActionResult UpdateLoadFuelAmount(string LoadNumber, double QuickPaypercentage, int? FuelAmount = 0, string JqueryTableSearchValue = "", int rowindex = 0)
        {
            string Query = "";
            try
            {
                TempData["JqueryTableSearchValue"] = JqueryTableSearchValue;
                TempData["LoadNumberscroll"] = LoadNumber;
                TempData["Rownumber"] = rowindex;
                //Session["JqueryTableSearchValue"] = JqueryTableSearchValue;
                //Session["LoadNumberscroll"] = LoadNumber;
                ViewBag.JqueryTableSearchValue = JqueryTableSearchValue;

                Query = "update tblloadhead set FuelAdvanceAmount = " + FuelAmount + ",QuickPaypercentage=" + QuickPaypercentage + "  Where LoaderNumber = '" + LoadNumber + "'";
                ut.InsertUpdate(Query);



                Query = "Exec Sp_Calculate_Quick_Pay  -1 , '" + LoadNumber + "'," + QuickPaypercentage + "";

                ut.InsertUpdate(Query);

                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [Customexception]
        public ActionResult UpdateLoadInvoiceNumber(string LoadNumber, string InvoiceNumber, string JqueryTableSearchValue)
        {
            string Query = "";
            try
            {


                TempData["JqueryTableSearchValue"] = JqueryTableSearchValue;
                TempData["LoadNumberscroll"] = LoadNumber;

                //Session["JqueryTableSearchValue"] = JqueryTableSearchValue;
                //Session["LoadNumberscroll"] = LoadNumber;
                ViewBag.JqueryTableSearchValue = JqueryTableSearchValue;

                if (InvoiceNumber == null)
                {
                    InvoiceNumber = "";
                }

                Query = "update tblloadhead set InvoiceNumber = '" + InvoiceNumber + "'  Where LoaderNumber = '" + LoadNumber + "'";


                ut.InsertUpdate(Query);

                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }


        public ActionResult collection()
        {
            try
            {
                ViewBag.customerlist = deEntity.tblBrokers.ToList();
                ViewBag.carrierlist = deEntity.tblCarriers.ToList();
                ViewBag.Userextensionlist = deEntity.Sp_Get_User_Extensions_List().ToList();
                ViewBag.Collectionlist = deEntity.Sp_get_collection_list().ToList();

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
        public ActionResult savecollection(int ddlcollectiontype, string ddlcustomer,string ddlcarrier,int amount)
        {
            try
            {
                string qry = "";
                if (ddlcollectiontype == 1)
                {
                    qry = "Exec SP_InsertUpdate_Collection " + ddlcollectiontype + ", '"+ ddlcustomer + "',"+ amount + ","+ Convert.ToInt32(Session["User_id"]) + " ";
                }
                else if (ddlcollectiontype == 2)
                {
                    qry = "Exec SP_InsertUpdate_Collection " + ddlcollectiontype + ", '" + ddlcarrier + "'," + amount + "," + Convert.ToInt32(Session["User_id"]) + " ";
                }
                ut.InsertUpdate(qry);


                return RedirectToAction("collection");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("collection");
        }

        [HttpPost]
        public JsonResult UpdateIspaid(int id, int status,string type)
        {

            string Query = "";
            try
            {

                if(type == "Customer")
                {
                    Query = "update tblcustomercollection set  ispaid ="+status+"  Where id = " + id + "";
                }
                else if (type == "Carrier")
                {
                    Query = "update tblcarriercollection set  ispaid =" + status + "  Where id = " + id + "";
                }

                ut.InsertUpdate(Query);
                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

    }
}