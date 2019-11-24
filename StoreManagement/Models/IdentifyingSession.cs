namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IdentifyingSession")]
    public partial class IdentifyingSession
    {
        public IdentifyingSession()
        {
        }

        public IdentifyingSession(string userID)
        {
            this.ExecutedDate = DateTime.Now;
            UserID = userID;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
