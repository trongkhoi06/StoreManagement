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
        public ConfirmingSession()
        {
        }

        [Key]
        public int ConfirmingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public int RequestPK { get; set; }
    }
}
