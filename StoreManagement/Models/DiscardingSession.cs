namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiscardingSession")]
    public partial class DiscardingSession
    {
        public DiscardingSession()
        {
        }

        public DiscardingSession(string comment, bool isVerified, string userID)
        {
            ExecutedDate = DateTime.Now;
            Comment = comment;
            IsVerified = isVerified;
            UserID = userID;
        }

        [Key]
        public int DiscardingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string Comment { get; set; }

        public bool IsVerified { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
