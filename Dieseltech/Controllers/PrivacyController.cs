using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [HandleError]
    public class PrivacyController : Controller
    {
        Utility ut = new Utility();
        // GET: Account
        string error = "";
        string name;
        string message = string.Empty;
        string qry = "";
        // GET: Privacy
        [Customexception]
        public ActionResult Index()
        {
            qry = "select ColorName from tblThemeColor";
            Session["Color"] = ut.ExecuteScalar(qry);
            return View();
        }
    }
}