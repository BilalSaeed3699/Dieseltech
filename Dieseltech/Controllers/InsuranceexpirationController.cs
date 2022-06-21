using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [HandleError]
    public class InsuranceexpirationController : Controller
    {
        private DieseltechEntities DbEntity = new DieseltechEntities();
        // GET: Insuranceexpiration
        [Customexception]
        public ActionResult Index()
        {
            try
            {
                DbEntity.Database.ExecuteSqlCommand("Exec  Sp_Insert_Insurance_Expiration_Date");
                ViewBag.Message = "Success";
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;

            }

            
            return View();
        }
    }
}