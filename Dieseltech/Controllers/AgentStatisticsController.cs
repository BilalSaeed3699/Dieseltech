using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class AgentStatisticsController : Controller
    {
        FormCollection m_formCollection;
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();



        [HttpPost]
        [Customexception]
        public ActionResult ReloadAgentStatistics(string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, int? ddlAgent)
        {

            Session["ddlAgent"] = ddlAgent;
            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, ddlAgent).ToList();
            //return Json(new { url = Url.Action("Index", "AgentStatistics", new { Filter = Filter, Type = Type, FilterDateFroms = FilterDateFroms, FilterDateTos = FilterDateTos,ddlAgent = ddlAgent }) });
            return Json(ViewBag.LoadDetailsMonth, JsonRequestBehavior.AllowGet);



        }


        [HttpPost]
        [Customexception]
        public JsonResult UpdateAgentCommission(int Agentid, int agentcommissionpercentage,int salary)

        {
            try
            {

                // Query for a specific customer.
                bool exists = (from users in deEntity.tblAgentcommissions
                               where users.AgentId == Agentid
                               select users).Any();

                if (exists == true)
                {
                    // Query for a specific customer.
                    var agentcommission =
                        (from c in deEntity.tblAgentcommissions
                         where c.AgentId == Agentid
                         select c).First();

                    // Change the name of the contact.
                  
                    agentcommission.Commissionpercentage = agentcommissionpercentage;
                    agentcommission.salary = salary;

                    // Ask the DataContext to save all the changes.
                    deEntity.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (exists == false)
                {
                    tblAgentcommission ta = new tblAgentcommission();
                    ta.AgentId = Agentid;
                    ta.Commissionpercentage = agentcommissionpercentage;
                    ta.salary = salary;
                    deEntity.tblAgentcommissions.Add(ta);

                    deEntity.SaveChanges();
                }



                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Report
        [Customexception]
        public ActionResult Index(int? previous, int? Next, string Filter, string Type, DateTime FilterDateFroms, DateTime FilterDateTos, int? BackViewType, int? ddlAgentvalue, string Error)

        {

            Int32 userid = 0;
            try
            {
                ViewBag.error = Error;
                if (Session["ddlAgent"] != null)
                {
                    ddlAgentvalue = Convert.ToInt32(Session["ddlAgent"]);
                }


                if (ddlAgentvalue == null)
                {
                    userid = Convert.ToInt32(Session["User_id"]);
                }
                else
                {
                    userid = ddlAgentvalue ?? 0;
                }
                int Commissionpercentage = 0;
                int salary = 0;
                var AgentCommission = deEntity.tblAgentcommissions.Where(a => a.AgentId == userid).FirstOrDefault();
                if(AgentCommission!=null)
                {
                    //Commissionpercentage = AgentCommission.Commissionpercentage;
                    Commissionpercentage = Commissionpercentage / 100 * AgentCommission.Commissionpercentage;
                    ViewBag.Agentcommissionpercentage = AgentCommission.Commissionpercentage;
                    salary = AgentCommission.salary;
                    ViewBag.salary = salary;
                }
                else
                {
                    ViewBag.Agentcommissionpercentage = Commissionpercentage;
                    ViewBag.salary = salary;
                }
                




                ViewBag.UserId = userid;
                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                var UserName = (from user in deEntity.tblUsers.Where(a => a.User_ID == userid)
                                select user.UserName
                                ).FirstOrDefault();

                ViewBag.UserName = UserName;
                if (Filter == null)
                {
                    Filter = "A";
                }


                ViewBag.Filter = Filter;

                DateTime Daily;
                if (previous == null && Next == null)
                {

                    if (Type == null)
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
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();



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
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;

                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();
                        }





                    }

                    else if (Type == "Weekly")
                    {


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
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;


                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;
                            ViewBag.IsDateFilter = "1";
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();
                        }



                    }
                    else if (Type == "Monthly")
                    {


                        if (Filter == "A")
                        {

                            ViewBag.LoadType = "Daily";
                            Daily = DateTime.Now;
                            ViewBag.LoadFilterDateFrom = Daily;
                            ViewBag.LoadFilterDateTo = Daily;



                            ViewBag.IsDateFilter = "1";
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, Daily, Daily, userid).ToList();
                        }
                        else if (Filter != "A")
                        {
                            ViewBag.LoadType = Type;
                            Daily = DateTime.Now;
                            ViewBag.LoadFilterDateFrom = FilterDateFroms;
                            ViewBag.LoadFilterDateTo = FilterDateTos;


                            string FullMonthName = FilterDateFroms.ToString("MMMM");
                            int year = FilterDateFroms.Year;
                            ViewBag.MonthDetails = FullMonthName + "  " + year;
                            ViewBag.IsDateFilter = "1";
                            ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();
                        }




                    }



                }



                if (previous == 1)
                {

                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;



                        DateTime baseDate = DateTime.Today;

                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

                        FilterDateFroms = FilterDateFroms.AddDays(-1);
                        FilterDateTos = FilterDateTos.AddDays(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;


                        ViewBag.IsDateFilter = "1";
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }

                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;

                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);



                        FilterDateFroms = FilterDateFroms.AddDays(-7);
                        FilterDateTos = FilterDateTos.AddDays(-7);
                        //FilterDateTos = FilterDateFroms.AddSeconds(-1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;


                        ViewBag.IsDateFilter = "1";
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }
                    else if (Type == "Monthly")
                    {




                        ViewBag.LoadType = Type;


                        Daily = DateTime.Now;




                        FilterDateFroms = FilterDateFroms.AddMonths(-1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);





                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;


                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;


                        ViewBag.IsDateFilter = "1";

                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }



                }

                if (Next == 1)
                {


                    if (Type == "Daily")
                    {
                        ViewBag.LoadType = Type;

                        FilterDateFroms = FilterDateFroms.AddDays(1);
                        FilterDateTos = FilterDateTos.AddDays(1);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;


                        ViewBag.IsDateFilter = "1";

                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }
                    else if (Type == "Weekly")
                    {
                        ViewBag.LoadType = Type;
                        DateTime baseDate = DateTime.Today;



                        var thisWeekStart = FilterDateFroms.AddDays(-(int)FilterDateFroms.DayOfWeek);



                        FilterDateFroms = FilterDateFroms.AddDays(7);
                        FilterDateTos = FilterDateTos.AddDays(7);



                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;



                        ViewBag.IsDateFilter = "1";

                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }


                    else if (Type == "Monthly")
                    {



                        ViewBag.LoadType = Type;

                        Daily = DateTime.Now;





                        FilterDateFroms = FilterDateFroms.AddMonths(1).AddDays(1 - FilterDateFroms.Day);
                        FilterDateTos = FilterDateFroms.AddMonths(1).AddDays(-1);






                        string FullMonthName = FilterDateFroms.ToString("MMMM");
                        int year = FilterDateFroms.Year;


                        ViewBag.MonthDetails = FullMonthName + "  " + year;


                        ViewBag.LoadFilterDateFrom = FilterDateFroms;
                        ViewBag.LoadFilterDateTo = FilterDateTos;



                        ViewBag.IsDateFilter = "1";
                        ViewBag.LoadDetailsMonth = deEntity.Sp_Get_Agent_Statistics_New(Filter, FilterDateFroms, FilterDateTos, userid).ToList();

                    }


                }





                if (Next == 1)
                {
                    Daily = Convert.ToDateTime(FilterDateFroms);
                    Daily = Daily.AddDays(1);
                    ViewBag.Daily = Daily;
                }

                //ViewBag.Agents = deEntity.tblUsers.Where(x => x.IsActive == 1 && x.Isdeleted == 0).ToList();

                List<sp_Get_AgentCommisionList_Result> Data = deEntity.sp_Get_AgentCommisionList(userid).OrderBy(o => o.AffectiveDate).ToList();
                ViewBag.Get_AgentCommisionList_Result = Data;

                List<sp_Get_AgentSalaryList_Result> Data1 = deEntity.sp_Get_AgentSalaryList(userid).OrderBy(o => o.AffectiveDate).ToList();
                ViewBag.Get_AgentSalaryList_Result = Data1;

                return View();


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();

        }


        [Customexception]
        public ActionResult AgentCommission( string Error)
        {
            try
            {
                ViewBag.AgentsLatestCommision = deEntity.sp_Get_AgentsLatestCommision().ToList();
                ViewBag.Agents = deEntity.tblUsers.Where(x=>x.IsActive==1&&x.Isdeleted==0).ToList();
                // Query for a specific customer.

                ViewBag.error = Error;


                return View();
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        
        //[HttpPost]
        //public ActionResult AgentCommission(int AgentId)
        //{
        //    int i = 0;
        //    try
        //    {
        //        List<sp_Get_AgentCommisionList_Result> Data = deEntity.sp_Get_AgentCommisionList(AgentId).ToList();
        //        // Query for a specific customer.




        //        return Json(Data,JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public ActionResult AgentCommission(int AgentId)
        {
           

            try
            {
                List<sp_Get_AgentCommisionList_Result> Data = deEntity.sp_Get_AgentCommisionList(AgentId).OrderBy(o=>o.AffectiveDate).ToList();
                // Query for a specific customer.




                return Json(Data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }




            return Json(0);
        }
        
        [HttpPost]
        public ActionResult AgentSalary(int AgentId)
        {
           

            try
            {
                List<sp_Get_AgentSalaryList_Result> Data = deEntity.sp_Get_AgentSalaryList(AgentId).OrderBy(o=>o.AffectiveDate).ToList();
                // Query for a specific customer.




                return Json(Data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }




            return Json(0);
        }

        [HttpPost]
        public ActionResult AddAgentCommission(int AgentId,int Commision, DateTime AffectiveDate,int AgentStat=0)
        {
           

            try
            {
                tblAgentcommission Data = new tblAgentcommission();
                tblAgentcommission check = new tblAgentcommission();
                check = deEntity.tblAgentcommissions.Where(x => x.AffectiveDate == AffectiveDate && x.AgentId == AgentId).FirstOrDefault();
                if (check==null)
                {
                    Data.AgentId = AgentId;
                    Data.Commissionpercentage = Commision;
                    Data.AffectiveDate = AffectiveDate;
                    Data.CreatedDate = DateTime.Now;
                    deEntity.tblAgentcommissions.Add(Data);
                    deEntity.SaveChanges();
                }
                else
                {
                    ViewBag.error = "Same date already exist!!!";
                }



                if(AgentStat==1)
                {
                    return RedirectToAction("Index", "AgentStatistics", new { Error = ViewBag.error, FilterDateFroms=DateTime.Now, FilterDateTos = DateTime.Now });
                }
                else
                {
                    return RedirectToAction("AgentCommission",new {Error= ViewBag.error });
                }

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }




            return Json(0);
        }
        
        [HttpPost]
        public ActionResult AddAgentSalary(int AgentId,int salary, DateTime AffectiveDate,int AgentStat=0)
        {
           

            try
            {
                tblAgentSalary Data = new tblAgentSalary();
                tblAgentSalary check = new tblAgentSalary();
                check = deEntity.tblAgentSalaries.Where(x => x.AffectiveDate == AffectiveDate && x.AgentId == AgentId).FirstOrDefault();
                if (check==null)
                {
                    Data.AgentId = AgentId;
                    Data.salary = salary;
                    Data.AffectiveDate = AffectiveDate;
                    Data.CreatedDate = DateTime.Now;
                    deEntity.tblAgentSalaries.Add(Data);
                    deEntity.SaveChanges();
                }
                else
                {
                    ViewBag.error = "Same date already exist!!!";
                }



                if(AgentStat==1)
                {
                    return RedirectToAction("Index", "AgentStatistics", new { Error = ViewBag.error, FilterDateFroms=DateTime.Now, FilterDateTos = DateTime.Now });
                }
                else
                {
                    return RedirectToAction("AgentCommission",new {Error= ViewBag.error });
                }

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }




            return Json(0);
        }

        [HttpPost]
        public JsonResult DeleteAgentCommission(int AgentCommissionId)
        {
           

            try
            {
                tblAgentcommission check = new tblAgentcommission();
                check = deEntity.tblAgentcommissions.Where(x => x.AgentCommissionId == AgentCommissionId).FirstOrDefault();
                if (check!=null)
                {
                    deEntity.Entry(check).State = EntityState.Deleted;
                    deEntity.SaveChanges();
                }

                return Json(1);

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteAgentSalary(int AgentSalaryId)
        {
           

            try
            {
                tblAgentSalary check = new tblAgentSalary();
                check = deEntity.tblAgentSalaries.Where(x => x.AgentSalaryId == AgentSalaryId).FirstOrDefault();
                if (check!=null)
                {
                    deEntity.Entry(check).State = EntityState.Deleted;
                    deEntity.SaveChanges();
                }

                return Json(1);

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}