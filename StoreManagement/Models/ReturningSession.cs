namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReturningSession")]
    public partial class ReturningSession
    {
        public ReturningSession()
        {
        }

        public ReturningSession(int failedItemPK, string userID)
        {
            ExecutedDate = DateTime.Now;
            FailedItemPK = failedItemPK;
            UserID = userID;
        }

        [Key]
        public int ReturningSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int FailedItemPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
