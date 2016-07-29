using MvcSf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSf
{
    public class OrderRepository
    {
        sfdb db = new sfdb();

        public OrderTable GetOrder(int id)
        {
            return GetOrders().Where(o => o.OrderID == id).FirstOrDefault();
        }

        public /*System.Linq.IQueryable<MvcSf.Models.OrderTable>*/ List<OrderTable> GetOrders()
        {
            //var orders = (from p in db.OrderTable select p);
            var orders = db.OrderTable.Select(x => x).ToList();
            return orders;
        }

        //public int GetOrders()
        //{
        //    var orders = "sand";
        //    return orders;
        //}
    }
}