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
        public Order(string OrderID, int SupplierPK,string EmployeeCode)
        {
            this.EmployeeCode = EmployeeCode;
            this.OrderID = OrderID;
            this.SupplierPK = SupplierPK;
            this.DateCreated = DateTime.Now;
            this.IsOpened = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderPK { get; set; }

        [Required]
        [StringLength(100)]
        public string OrderID { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public bool IsOpened { get; set; }

        public int SupplierPK { get; set; }

        [StringLength(50)]
        public string EmployeeCode { get; set; }
    }
}
