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
            FailedItemPK = failedItemPK;
            UserID = userID;
            ExecutedDate = DateTime.Now;
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
