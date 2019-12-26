namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReceivingSession")]
    public partial class ReceivingSession
    {
        public ReceivingSession()
        {
        }

        public ReceivingSession(string userID, int restorationPK)
        {
            ExecutedDate = DateTime.Now;
            UserID = userID;
            RestorationPK = restorationPK;
        }

        [Key]
        public int ReceivingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public int RestorationPK { get; set; }
        
    }
}
