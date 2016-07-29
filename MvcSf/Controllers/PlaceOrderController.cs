using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcSf.Models;
using MvcSf.ViewModels;

namespace MvcSf.Controllers
{
    public class PlaceOrderController : Controller
    {
        private sfdb db = new sfdb();

        // GET: PlaceOrder
        public ActionResult Index()
        {
            var orderTable = db.OrderTable.Include(o => o.AddressTable).Include(o => o.StatusTable).Include(o => o.UserTable);
            return View(orderTable.ToList());
        }

        // GET: PlaceOrder/Details/5
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
        

        // GET: PlaceOrder/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateAddress(int id, [Bind(Include = "Address1, Address2, Address3, City, StateID, ZipCode, IsBilling, IsShipping, DateCreated, CreatedBy, DateModified, ModifiedBy")] AddressTable addressTable)
        //{
        //    ViewBag.AddressID = new SelectList(db.AddressTable, "AddressID", "Address1");
        //    ViewBag.StatusID = new SelectList(db.StatusTable, "StatusID", "StatusDescription");
        //    ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName");


        //    if (ModelState.IsValid)
        //    {
        //        addressTable.UserID = id;
        //        db.AddressTable.Add(addressTable);
        //        db.SaveChanges();
        //        return RedirectToAction("ChooseAddress");
        //    }

        //    return View(addressTable);
        //}

        // GET: AddressTable/Create
        public ActionResult CreateAddress()
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
        public ActionResult CreateAddress([Bind(Include = "Address1,Address2,Address3,City,StateID,ZipCode,IsBilling,IsShipping")] AddressTable addressTable)
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
                return RedirectToAction("ChooseAddress");
            }

            ViewBag.StateID = new SelectList(db.StateTable, "StateID", "StateAbbrev", addressTable.StateID);
            ViewBag.UserID = new SelectList(db.UserTable, "UserID", "UserName", addressTable.UserID);
            return View(addressTable);
        }

        public ActionResult ChooseAddress()
        {
            //var addressTable = db.AddressTable.Include(which => which.OrderTable).Include(which => which.UserTable);
            var userQuery = (from userLinq in db.UserTable.Where(userLinq => String.Equals(userLinq.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq).FirstOrDefault();
            ViewBag.UserID = userQuery.UserID;
            var addressQuery = from addressLinq in db.AddressTable.Where(addressLinq => addressLinq.UserID == userQuery.UserID) select addressLinq;
            return View(addressQuery.ToList());
        }

        public ActionResult ConfirmOrderAddress(int id)
        {
            var addressQuery = (from addressLinq in db.AddressTable.Where(addressLinq => addressLinq.AddressID == id) select addressLinq).FirstOrDefault();
            //var userQuery = (from userLinq in db.UserTable.Where(u => u.UserID == addressQuery.UserID) select userLinq).FirstOrDefault();
            //var cartQuery = (from cartLinq in db.ShoppingCartTable.Where(c => c.UserID == userQuery.UserID) select cartLinq).FirstOrDefault();
            //var itemQuery = (from itemLinq in db.ShoppingCartProductTable.Where(i => i.ShoppingCartID == cartQuery.ShoppingCartID) select itemLinq).ToList();
            return View(addressQuery);
        }

        [HttpGet]
        public ActionResult ConfirmOrderItems()
        {
            var userID = (from userLinq in db.UserTable.Where(userLinq => String.Equals(userLinq.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq.UserID).FirstOrDefault();
            var cartID = (from cartLinq in db.ShoppingCartTable.Where(c => c.UserID == userID) select cartLinq.ShoppingCartID).FirstOrDefault();
            var itemQuery = (from itemLinq in db.ShoppingCartProductTable.Where(i => i.ShoppingCartID == cartID) select itemLinq).ToList();
            
            return PartialView("ConfirmOrderItems", itemQuery);
        }


        public ActionResult CompleteOrder(int id)
        {
            var userQuery = (from userLinq in db.UserTable.Where(userLinq => String.Equals(userLinq.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq).FirstOrDefault();
            var matchCartToUserQuery = (from shoppingCartLinq in db.ShoppingCartTable.Where(s => s.UserID == userQuery.UserID) select shoppingCartLinq).FirstOrDefault();
            var matchCartItemToCart = (from cartItemLinq in db.ShoppingCartProductTable.Where(c => c.ShoppingCartID == matchCartToUserQuery.ShoppingCartID) select cartItemLinq);

            decimal totalPriceSum = 0;
            for(int k = 0; k < matchCartItemToCart.Count(); k++)
            {
                totalPriceSum += (decimal)(matchCartItemToCart.ToList()[k].Quantity * matchCartItemToCart.ToList()[k].Price);

            }
            OrderTable ot = new OrderTable
            {
                UserID = userQuery.UserID,
                AddressID = id,
                OrderDate = DateTime.Now,
                //Total = (decimal)matchCartItemToCart.ToList().Sum(x => x.Quantity * x.Price)   can't get this to work FIND OUT WHY. so instead for now I'll use totalPriceSum
                Total = totalPriceSum
            };
            db.OrderTable.Add(ot);



            foreach (var item in matchCartItemToCart)
            {

                OrderProductTable opt = new OrderProductTable
                {
                    OrderID = ot.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    DateCreated = DateTime.Now
                };
                db.OrderProductTable.Add(opt);

                var productTemp = (from qualityQualifier in db.ProductTable.Where(QQ => QQ.ProductID == item.ProductID) select qualityQualifier).FirstOrDefault();
                productTemp.Quantity = productTemp.Quantity - item.Quantity;
                if (productTemp.Quantity < 1)
                {
                    db.ProductTable.Remove(productTemp);
                }
                db.ShoppingCartProductTable.Remove(item);
            }

            db.ShoppingCartTable.Remove(matchCartToUserQuery);

            db.SaveChanges();
            return View();
        }


        //Finalizes the order, and then thanks the user for their business.
        /*public ActionResult Thanks()
        {
            var userQuery = (from userLinq in db.UserTable.Where(userLinq => String.Equals(userLinq.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq).FirstOrDefault();
            var matchCartToUserQuery = (from shoppingCartLinq in db.ShoppingCartTable.Where(s => s.UserID == userQuery.UserID) select shoppingCartLinq).FirstOrDefault();
            var matchCartItemToCart = (from cartItemLinq in db.ShoppingCartProductTable.Where(c => c.ShoppingCartID == matchCartToUserQuery.ShoppingCartID) select cartItemLinq);
            //decimal totalPriceSum = 0;
            //for (int kwant = 0; kwant < matchCartItemToCart.ToList().Count(); kwant++)
            //{
            //    totalPriceSum += (decimal)(matchCartItemToCart.ToList()[kwant].Price);
            //}

            OrderTable ot = new OrderTable
            {
                UserID = userQuery.UserID,
                OrderDate = DateTime.Now,
                StatusID = 1,   //enters first status phase
                Total = matchCartItemToCart.ToList().Sum(x => x.Quantity * x.Price)/*.ToString("$0.00")*/
                //Total = totalPriceSum
            /*};
            db.OrderTable.Add(ot);



            foreach (var item in matchCartItemToCart)
            {

                OrderProductTable opt = new OrderProductTable
                {
                    OrderID = ot.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    DateCreated = DateTime.Now
                };
                db.OrderProductTable.Add(opt);

                var productTemp = (from qualityQualifier in db.ProductTable.Where(QQ => QQ.ProductID == item.ProductID) select qualityQualifier).FirstOrDefault();
                productTemp.Quantity = productTemp.Quantity - item.Quantity;
                if (productTemp.Quantity < 1)
                {
                    db.ProductTable.Remove(productTemp);
                }
                db.ShoppingCartProductTable.Remove(item);
            }

            db.ShoppingCartTable.Remove(matchCartToUserQuery);

            db.SaveChanges();
            return View();
        }*/

        public ActionResult Thanks(int id)
        {
            var userQuery = (from userLinq in db.UserTable.Where(userLinq => String.Equals(userLinq.UserName, System.Web.HttpContext.Current.User.Identity.Name)) select userLinq).FirstOrDefault();
            var matchCartToUserQuery = (from shoppingCartLinq in db.ShoppingCartTable.Where(s => s.UserID == userQuery.UserID) select shoppingCartLinq).FirstOrDefault();
            var matchCartItemToCart = (from cartItemLinq in db.ShoppingCartProductTable.Where(c => c.ShoppingCartID == matchCartToUserQuery.ShoppingCartID) select cartItemLinq);

            decimal totalPriceSum = 0;
            for (int k = 0; k < matchCartItemToCart.Count(); k++)
            {
                totalPriceSum += (decimal)(matchCartItemToCart.ToList()[k].Quantity * matchCartItemToCart.ToList()[k].Price);

            }
            OrderTable ot = new OrderTable
            {
                UserID = userQuery.UserID,
                AddressID = id,
                OrderDate = DateTime.Now,
                StatusID = 1, // enters first status
                //Total = (decimal)matchCartItemToCart.ToList().Sum(x => x.Quantity * x.Price)   can't get this to work FIND OUT WHY. so instead for now I'll use totalPriceSum
                Total = totalPriceSum
            };
            db.OrderTable.Add(ot);



            foreach (var item in matchCartItemToCart)
            {

                OrderProductTable opt = new OrderProductTable
                {
                    OrderID = ot.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    DateCreated = DateTime.Now
                };
                db.OrderProductTable.Add(opt);

                var productTemp = (from qualityQualifier in db.ProductTable.Where(QQ => QQ.ProductID == item.ProductID) select qualityQualifier).FirstOrDefault();
                productTemp.Quantity = productTemp.Quantity - item.Quantity;
                if (productTemp.Quantity < 1)
                {
                    db.ProductTable.Remove(productTemp);
                }
                db.ShoppingCartProductTable.Remove(item);
            }

            db.ShoppingCartTable.Remove(matchCartToUserQuery);

            db.SaveChanges();
            return View();
        }



        // POST: PlaceOrder/Create
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

        // GET: PlaceOrder/Edit/5
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

        // POST: PlaceOrder/Edit/5
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

        // GET: PlaceOrder/Delete/5
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

        // POST: PlaceOrder/Delete/5
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
