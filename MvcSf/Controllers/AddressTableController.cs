using System;
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
    public class AddressTableController : Controller
    {
        private sfdb db = new sfdb();

        // GET: AddressTable
        public ActionResult Index()
        {
            var addressTable = db.AddressTable.Include(a => a.StateTable).Include(a => a.UserTable);
            return View(addressTable.ToList());
        }

        // GET: AddressTable/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressTable addressTable = db.AddressTable.Find(id);
            if (addressTable == null)
            {
                return HttpNotFound();
            }
            return View(addressTable);
        }

        // GET: AddressTable/Create
        public ActionResult Create()
        {
            ViewBag.StateID = new SelectList(db.StateTable, "StateID", "StateAbbrev");
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName");
            return View();
        }

        // POST: AddressTable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Address1,Address2,Address3,City,StateID,ZipCode,IsBilling,IsShipping")] AddressTable addressTable)
        {
            if (ModelState.IsValid)
            {
                var userQuery = (from userLinq in db.UserTable.Where(u => String.Equals(u.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq).FirstOrDefault();
                addressTable.UserID = userQuery.UserID;
                addressTable.DateCreated = DateTime.Now;
                addressTable.CreatedBy = userQuery.UserName;
                addressTable.DateModified = DateTime.Now;
                addressTable.ModifiedBy = userQuery.UserName;
                db.AddressTable.Add(addressTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateID = new SelectList(db.StateTable, "StateID", "StateAbbrev", addressTable.StateID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", addressTable.UserID);
            return View(addressTable);
        }

        // GET: AddressTable/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressTable addressTable = db.AddressTable.Find(id);
            if (addressTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(db.StateTable, "StateID", "StateAbbrev", addressTable.StateID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", addressTable.UserID);
            return View(addressTable);
        }

        // POST: AddressTable/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AddressID,UserID,Address1,Address2,Address3,City,StateID,ZipCode,IsBilling,IsShipping,DateCreated,CreatedBy,DateModified,ModifiedBy")] AddressTable addressTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(addressTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(db.StateTable, "StateID", "StateAbbrev", addressTable.StateID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", addressTable.UserID);
            return View(addressTable);
        }

        // GET: AddressTable/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressTable addressTable = db.AddressTable.Find(id);
            if (addressTable == null)
            {
                return HttpNotFound();
            }
            return View(addressTable);
        }

        // POST: AddressTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AddressTable addressTable = db.AddressTable.Find(id);
            db.AddressTable.Remove(addressTable);
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
