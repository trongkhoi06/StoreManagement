namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public Order()
        {

        }

        public Order(string orderID, int supplierPK, string userID)
        {
            OrderID = orderID;
            SupplierPK = supplierPK;
            UserID = userID;
        }

        [Key]
        public int OrderPK { get; set; }

        [Required]
        [StringLength(100)]
        public string OrderID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int SupplierPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
