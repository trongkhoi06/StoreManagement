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

        public Request(string requestID, DateTime expectedDate, bool isIssued, bool isConfirmed, string comment, int demandPK, string userID)
        {
            RequestID = requestID;
            DateCreated = DateTime.Now;
            ExpectedDate = expectedDate;
            IsIssued = isIssued;
            IsConfirmed = isConfirmed;
            Comment = comment;
            DemandPK = demandPK;
            UserID = userID;
        }

        [Key]
        public int RequestPK { get; set; }

        [Required]
        [StringLength(100)]
        public string RequestID { get; set; }

        public DateTime DateCreated { get; set; }

        [Column(TypeName = "date")]
        public DateTime ExpectedDate { get; set; }

        public bool IsIssued { get; set; }

        public bool IsConfirmed { get; set; }

        public string Comment { get; set; }

        public int DemandPK { get; set; }

        public string UserID { get; set; }
    }
}
