using MvcSf.Models;
using MvcSf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSf.Controllers
{
    public class OrdersController : Controller
    {
        private sfdb db = new sfdb();
        OrderRepository oRepo = new OrderRepository();

        // GET: OrdersAdmin
        public ActionResult OrdersAdminIndex()
        {
            return View(oRepo.GetOrders());
        }

        // GET: OrderAdminDetails
        public ActionResult OrderAdminDetailsIndex(int id)
        {
            //var userQuery = db.UserTable.Where(u => u.UserName == System.Web.HttpContext.Current.User.Identity.Name).Select(u => u.UserID).FirstOrDefault();
            //var cartQuery = db.ShoppingCartTable.Where(c => c.UserID == userQuery).Select(c => c.ShoppingCartID).FirstOrDefault();
            //var cartItemQuery = db.ShoppingCartProductTable.Where(ci => ci.ShoppingCartID == cartQuery).Select(cimple => cimple).ToList();
            ViewBag.OrderID = (int)id;
            return View();
        }

        [HttpGet]
        public PartialViewResult OrderAdminDetailsGeneral(int id)
        {
            return PartialView("OrderAdminDetailsGeneral", oRepo.GetOrder((int)id));
        }

        [HttpGet]
        public PartialViewResult OrderAdminDetailsAddress(int id)
        {
            //var addressQuery = db.AddressTable.Where(a => a.AddressID == oRepo.GetOrder(id).AddressID).Select(a => a).FirstOrDefault();
            var orderQuery = db.OrderTable.Where(o => o.OrderID == id).Select(o => o.AddressID).FirstOrDefault();
            var addressQuery = db.AddressTable.Where(a => a.AddressID == orderQuery).Select(a => a).FirstOrDefault();
            return PartialView("OrderAdminDetailsAddress", addressQuery);
        }

        [HttpGet]
        public PartialViewResult OrderAdminDetailsProducts(int id)
        {

            //var opQuery = db.OrderProductTable.Where(op => op.OrderID == (int)id).Select(op => op).ToList();
            //string itemName = (string)(db.ProductTable.Where(p => p.ProductID == opQuery[1].ProductID).Select(p => p.ProductName)).Single();
            //string itemName = from opQuery2 in db.OrderProductTable join itemQuery in db.ProductTable on opQuery2.OrderID equals id where itemQuery.ProductID == opQuery2.ProductID select itemQuery.ProductName.FirstOrDefault();

            var opQuery = db.OrderProductTable.Where(op => op.OrderID == (int)id).Select(op => op).ToList();
            string itemName = opQuery[0].ProductTable.ProductName;
            var opvm = new List<OrderProductViewModel>
            {
            };
            for (int i = 0; i < opQuery.Count(); i++)
            {
                itemName = opQuery[i].ProductTable.ProductName;
                //string itemName = (from opQuery2 in db.OrderProductTable join itemQuery in db.ProductTable on opQuery2.OrderID equals id where opQuery[i].ProductID == itemQuery.ProductID && itemQuery.ProductID == opQuery2.ProductID select itemQuery.ProductName).FirstOrDefault();

                //itemName = db.ProductTable.Where(p => p.ProductID == opQuery[i].ProductID).Select(p => p.ProductName).FirstOrDefault(); 
                opvm.Add(new OrderProductViewModel
                {
                    OrderProductID = opQuery[i].OrderProductID,
                    ProductID = opQuery[i].ProductID,
                    //ProductName = db.ProductTable.Where(p => p.ProductID == opQuery[i].ProductID).Select(p => p.ProductName).FirstOrDefault(),
                    ProductName = itemName,
                    Quantity = (int)opQuery[i].Quantity,
                    Price = (decimal)opQuery[i].Price
                });
            }
            return PartialView("OrderAdminDetailsProducts", opvm);
        }
    }
}