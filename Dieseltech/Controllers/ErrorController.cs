﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExceptionPage()
        {
            return View();
        }

    }
}