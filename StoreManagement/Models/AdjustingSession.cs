namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdjustingSession")]
    public partial class AdjustingSession
    {
        public AdjustingSession()
        {
        }

        public AdjustingSession(string comment, bool isVerified, string userID)
        {
            Comment = comment;
            IsVerified = isVerified;
            UserID = userID;
            ExecutedDate = DateTime.Now;
        }

        [Key]
        public int AdjustingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string Comment { get; set; }

        public bool IsVerified { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
