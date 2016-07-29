using MvcSf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSf.ViewModels
{
    public class ConfirmOrderViewModel
    {
        //public AddressTable AddressTable { get; set; }
        //public OrderProductTable OrderProductTable { get; set; }
        //public OrderTable OrderTable { get; set; }
        //public ProductTable ProductTable { get; set; }
        //public ShoppingCartProductTable ShoppingCartProductTable { get; set; }
        //public ShoppingCartTable ShoppingCartTable { get; set; }
        //public StateTable StateTable { get; set; }
        //public StatusTable StatusTable { get; set; }
        //public sysdiagrams sysdiagrams { get; set; }
        //public UserTable UserTable { get; set; }

        public int AddressID { get; set; }
        public int UserID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public String State { get; set; }
        public string Zipcode { get; set; }
        public DateTime DateTime { get; set; }


        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string ImageFile { get; set; }
        public byte[] ProductImage { get; set; }
        //public int ResultCount { get; set; }
        public int AlreadyInCart { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }
    }
}