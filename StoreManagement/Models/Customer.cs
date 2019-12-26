namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Customer")]
    public partial class Customer
    {
        public Customer()
        {
        }

        public Customer(string customerName, string customerCode, string address, string phoneNumber)
        {
            CustomerName = customerName;
            CustomerCode = customerCode;
            Address = address;
            PhoneNumber = phoneNumber;
            TaxID = "";
            IsActive = true;
        }

        [Key]
        public int CustomerPK { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerCode { get; set; }

        public string Address { get; set; }

        [StringLength(100)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string TaxID { get; set; }

        public bool IsActive { get; set; }
    }
}
