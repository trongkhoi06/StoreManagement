namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConfirmingSession")]
    public partial class ConfirmingSession
    {
        public ConfirmingSession(string userID, int issuePK)
        {
            UserID = userID;
            IssuePK = issuePK;
            ExecutedDate = DateTime.Now;
        }

        [Key]
        public int ConfirmingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public int IssuePK { get; set; }
    }
}
