namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Supplier")]
    public partial class Supplier
    {
        public Supplier()
        {

        }

        public Supplier(string supplierCode, string supplierName, string supplierAddress, string supplierPhoneNumber)
        {
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            SupplierAddress = supplierAddress;
            SupplierPhoneNumber = supplierPhoneNumber;
            IsActive = true;
        }

        [Key]
        public int SupplierPK { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }

        public string SupplierAddress { get; set; }

        [StringLength(100)]
        public string SupplierPhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
