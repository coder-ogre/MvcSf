﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcSf.Models;

namespace MvcSf.Controllers
{
    public class OrderAdminDetailsController : Controller
    {
        private sfdb db = new sfdb();
        OrderRepository oRepo = new OrderRepository();
        // GET: OrderAdminDetails
        //public ActionResult Index()
        //{
        //    var orderTable = db.OrderTable.Include(o => o.AddressTable).Include(o => o.StatusTable).Include(o => o.UserTable);
        //    return View(orderTable.ToList());
        //}

        // GET: OrderAdminDetails
        //public ActionResult Index(int id)
        //{
        //    return View(oRepo.GetOrder(id));
        //}

        public ActionResult Index(int id)
        {
            return View();
        }

        // GET: list of products
        [HttpGet]
        public PartialViewResult getOrderProducts(int id)
        {
            //var theGoods = (from stuff in db.OrderProductTable.Where(p => p.OrderID == id) select stuff).ToList();
            var theGoods = db.OrderProductTable.Where(p => p.OrderID == id).Select(x => x);
            return PartialView("getOrderProducts", theGoods);
        }

        // GET: 

        // GET: OrderAdminDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTable orderTable = db.OrderTable.Find(id);
            if (orderTable == null)
            {
                return HttpNotFound();
            }
            return View(orderTable);
        }

        // GET: OrderAdminDetails/Create
        public ActionResult Create()
        {
            ViewBag.AddressID = new SelectList(db.AddressTable, "AddressID", "Address1");
            ViewBag.StatusID = new SelectList(db.StatusTable, "StatusID", "StatusDescription");
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName");
            return View();
        }

        // POST: OrderAdminDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserID,AddressID,OrderDate,Total,StatusID,DateCreated,CreatedBy,DateModified,ModifiedBy")] OrderTable orderTable)
        {
            if (ModelState.IsValid)
            {
                db.OrderTable.Add(orderTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressID = new SelectList(db.AddressTable, "AddressID", "Address1", orderTable.AddressID);
            ViewBag.StatusID = new SelectList(db.StatusTable, "StatusID", "StatusDescription", orderTable.StatusID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", orderTable.UserID);
            return View(orderTable);
        }

        // GET: OrderAdminDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTable orderTable = db.OrderTable.Find(id);
            if (orderTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressID = new SelectList(db.AddressTable, "AddressID", "Address1", orderTable.AddressID);
            ViewBag.StatusID = new SelectList(db.StatusTable, "StatusID", "StatusDescription", orderTable.StatusID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", orderTable.UserID);
            return View(orderTable);
        }

        // POST: OrderAdminDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,UserID,AddressID,OrderDate,Total,StatusID,DateCreated,CreatedBy,DateModified,ModifiedBy")] OrderTable orderTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AddressID = new SelectList(db.AddressTable, "AddressID", "Address1", orderTable.AddressID);
            ViewBag.StatusID = new SelectList(db.StatusTable, "StatusID", "StatusDescription", orderTable.StatusID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", orderTable.UserID);
            return View(orderTable);
        }

        // GET: OrderAdminDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTable orderTable = db.OrderTable.Find(id);
            if (orderTable == null)
            {
                return HttpNotFound();
            }
            return View(orderTable);
        }

        // POST: OrderAdminDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderTable orderTable = db.OrderTable.Find(id);
            db.OrderTable.Remove(orderTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
