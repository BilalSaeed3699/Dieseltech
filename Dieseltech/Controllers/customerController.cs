using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    public class customerController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        // GET: customer
        [Customexception]
        public ActionResult monthcomparison(string ddlcustomers = "")
        {
            var customerlist = deEntity.tblBrokers.ToList();
            ViewBag.customerlist = customerlist;
            ViewBag.Customer = ddlcustomers;
            var comparisonlist = deEntity.Sp_get_Customer_Month_Comparison(ddlcustomers).ToList();
            ViewBag.comparisonlist = comparisonlist;
            return View();
        }
        [Customexception]
        public ActionResult Reloadcomparison(string ddlcustomers ="")
        {

            return Json(new { url = Url.Action("monthcomparison", "customer", new { ddlcustomers = ddlcustomers }) });
        }
    }
}