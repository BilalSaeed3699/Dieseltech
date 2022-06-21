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
        public ActionResult companyrevenue(int ddlAgentvalue=0)
        {
            try
            {


                Int32 userid = 0;
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
                    userid = ddlAgentvalue;
                }

                ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();
                ViewBag.UserId = userid;
                var CompanyRevenue = deEntity.Sp_Get_Company_Revenue(ddlAgentvalue).ToList();
                ViewBag.CompanyRevenue = CompanyRevenue;


                var CompanySummaryRevenue = deEntity.Sp_Get_Company_Summary_Revenue(ddlAgentvalue).ToList();
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
        public ActionResult ReloadCompanyreveneu(int? ddlAgentvalue)
        {

            return Json(new { url = Url.Action("companyrevenue", "revenue", new { ddlAgentvalue = ddlAgentvalue }) });
        }
    }
}