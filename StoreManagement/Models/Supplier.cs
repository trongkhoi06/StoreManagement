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

        public Supplier(string supplierName, string supplierAddress, string supplierPhoneNumber, string taxID, string supplierCode)
        {
            SupplierName = supplierName;
            SupplierAddress = supplierAddress;
            SupplierPhoneNumber = supplierPhoneNumber;
            TaxID = taxID;
            IsActive = true;
            SupplierCode = supplierCode;
        }

        [Key]
        public int SupplierPK { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }

        [StringLength(100)]
        public string SupplierAddress { get; set; }

        [StringLength(100)]
        public string SupplierPhoneNumber { get; set; }

        [StringLength(100)]
        public string TaxID { get; set; }

        public bool IsActive { get; set; }
    }
}
