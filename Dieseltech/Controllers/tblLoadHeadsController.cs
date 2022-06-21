using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dieseltech.Models;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class tblLoadHeadsController : Controller
    {
        private DieseltechEntities db = new DieseltechEntities();

        // GET: tblLoadHeads
        [Customexception]
        public ActionResult Index()
        {
            return View(db.tblLoadHeads.ToList());
        }

        // GET: tblLoadHeads/Details/5
        [Customexception]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoadHead tblLoadHead = db.tblLoadHeads.Find(id);
            if (tblLoadHead == null)
            {
                return HttpNotFound();
            }
            return View(tblLoadHead);
        }

        // GET: tblLoadHeads/Create
        [Customexception]
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblLoadHeads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Create([Bind(Include = "LoaderHeadId,LoaderNumber,CarrierId,CarrierHelperId,NumberToText,IsSendText,TruckId,CarrierRate,LoadTypeId,Commodity,LoadSubTypeId,Available,Weight,QuantityTypeId,Quantity,DriverTypeId,CarrierInstructions,CompanyId,BrokerId,BrokerHelperId,BrokerRate,ContactName,ContactPhone,Extension,ContactEmail,BrokerReference,RegistrationDate,User_ID,AgentGross,AgentFlat,BranchFlat")] tblLoadHead tblLoadHead)
        {
            if (ModelState.IsValid)
            {
                db.tblLoadHeads.Add(tblLoadHead);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblLoadHead);
        }

        // GET: tblLoadHeads/Edit/5
        [Customexception]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoadHead tblLoadHead = db.tblLoadHeads.Find(id);
            if (tblLoadHead == null)
            {
                return HttpNotFound();
            }
            return View(tblLoadHead);
        }

        // POST: tblLoadHeads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Edit([Bind(Include = "LoaderHeadId,LoaderNumber,CarrierId,CarrierHelperId,NumberToText,IsSendText,TruckId,CarrierRate,LoadTypeId,Commodity,LoadSubTypeId,Available,Weight,QuantityTypeId,Quantity,DriverTypeId,CarrierInstructions,CompanyId,BrokerId,BrokerHelperId,BrokerRate,ContactName,ContactPhone,Extension,ContactEmail,BrokerReference,RegistrationDate,User_ID,AgentGross,AgentFlat,BranchFlat")] tblLoadHead tblLoadHead)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblLoadHead).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblLoadHead);
        }

        // GET: tblLoadHeads/Delete/5
        [Customexception]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoadHead tblLoadHead = db.tblLoadHeads.Find(id);
            if (tblLoadHead == null)
            {
                return HttpNotFound();
            }
            return View(tblLoadHead);
        }

        // POST: tblLoadHeads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult DeleteConfirmed(int id)
        {
            tblLoadHead tblLoadHead = db.tblLoadHeads.Find(id);
            db.tblLoadHeads.Remove(tblLoadHead);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Customexception]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
