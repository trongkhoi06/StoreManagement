namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CountingSession")]
    public partial class CountingSession
    {
        public CountingSession()
        {
        }

        public CountingSession(double countedQuantity, int identifiedItemPK, string userID)
        {
            CountedQuantity = countedQuantity;
            IdentifiedItemPK = identifiedItemPK;
            UserID = userID;
            ExecutedDate = DateTime.Now;
        }

        [Key]
        public int CountingSessionPK { get; set; }

        public double CountedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int IdentifiedItemPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
