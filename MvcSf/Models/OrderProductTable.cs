//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcSf.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderProductTable
    {
        public int OrderProductID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual OrderTable OrderTable { get; set; }
        public virtual ProductTable ProductTable { get; set; }
    }
}
