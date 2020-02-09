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

        public Customer(string customerCode, string customerName, string address, string phoneNumber)
        {
            CustomerCode = customerCode;
            CustomerName = customerName;
            Address = address;
            PhoneNumber = phoneNumber;
            IsActive = true;
        }

        [Key]
        public int CustomerPK { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerCode { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        public string Address { get; set; }

        [StringLength(100)]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
