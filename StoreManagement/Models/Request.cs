namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Request")]
    public partial class Request
    {
        public Request()
        {
        }

        [Key]
        public int RequestPK { get; set; }

        [Required]
        [StringLength(100)]
        public string RequestID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ExpectedDate { get; set; }

        public bool IsIssued { get; set; }

        public bool IsConformed { get; set; }

        public string Comment { get; set; }

        public int DemandPK { get; set; }
    }
}
